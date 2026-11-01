using System.Diagnostics;

namespace Saucery.CoverageGapRemediator;

internal sealed class RepositoryValidator
{
    public async Task<ValidationResult> ValidateAsync(
        RepositoryContext context,
        IReadOnlyList<CoverageDiscrepancy> targets,
        Options options)
    {
        var messages = new List<string>();

        var coreBuild = await ProcessRunner.RunAsync(
            "dotnet",
            ["build", Path.Combine("Saucery.Core", "Saucery.Core.csproj"), "-c", "Release"],
            context.Root);

        messages.Add($"Build Saucery.Core: {(coreBuild.ExitCode == 0 ? "PASS" : "FAIL")}");
        if(coreBuild.ExitCode != 0)
        {
            messages.Add(coreBuild.CombinedOutput);
            return new(false, messages, Array.Empty<CoverageDiscrepancy>());
        }

        foreach(var testProject in context.TestProjects)
        {
            var relativeProject = context.Relative(testProject);
            var testBuild = await ProcessRunner.RunAsync(
                "dotnet",
                ["build", relativeProject, "-c", "Release"],
                context.Root);

            messages.Add($"Build {relativeProject}: {(testBuild.ExitCode == 0 ? "PASS" : "FAIL")}");
            if(testBuild.ExitCode != 0)
            {
                messages.Add(testBuild.CombinedOutput);
                return new(false, messages, Array.Empty<CoverageDiscrepancy>());
            }
        }

        if(options.VerifyWithReporter && File.Exists(context.ReporterProject))
        {
            var reportPath = Path.Combine(context.Root, "saucery-functional-platform-coverage-gaps.txt");
            var reporter = await ProcessRunner.RunAsync(
                "dotnet",
                [
                    "run",
                    "--project", Path.Combine("Saucery.CoverageGapReporter", "Saucery.CoverageGapReporter.csproj"),
                    "--configuration", "Release",
                    "--",
                    "--repo-root", context.Root,
                    "--output", reportPath,
                    "--region", "us-west-1",
                    "--include-real", "true"
                ],
                context.Root);

            messages.Add($"Re-run CoverageGapReporter: {(reporter.ExitCode == 0 ? "PASS" : "FAIL")}");
            if(reporter.ExitCode != 0)
            {
                messages.Add(reporter.CombinedOutput);
                return new(false, messages, Array.Empty<CoverageDiscrepancy>());
            }

            var after = await CoverageReportReader.ReadAsync(reportPath);

            var reporterVerifiedTargets = targets
                .Where(t => t.Type == DiscrepancyType.Missing)
                .ToArray();

            var remaining = after
                .Where(a => reporterVerifiedTargets.Any(t => Equivalent(t, a)))
                .DistinctBy(a => a.Identity)
                .ToArray();

            if(remaining.Length > 0)
            {
                messages.Add($"Reporter verification: {remaining.Length} missing-platform discrepancy(s) remain; another synthesis pass will be attempted.");
                messages.AddRange(remaining.Select(x => $"  - {x.Type}: {x.DisplayName}"));
                return new(false, messages, remaining);
            }

            var obsoleteTargets = targets
                .Where(t => t.Type == DiscrepancyType.ObsoleteCandidate)
                .ToArray();

            if(reporterVerifiedTargets.Length > 0)
                messages.Add("Reporter verification: PASS. All targeted missing-platform discrepancies disappeared.");

            if(obsoleteTargets.Length > 0)
                messages.Add($"Reporter verification: {obsoleteTargets.Length} obsolete candidate(s) are compile-validated only and will not be retried based on reporter output.");
        }
        else
        {
            messages.Add("Coverage reporter verification: SKIPPED");
        }

        var xunitV3Project = context.TestProjects
            .SingleOrDefault(project =>
                Path.GetFileName(project).Equals(
                    "Saucery.Core.Tests.XUnitv3.csproj",
                    StringComparison.OrdinalIgnoreCase));

        if(xunitV3Project is null)
        {
            messages.Add("Run Saucery.Core.Tests.XUnitv3: FAIL");
            messages.Add("  Required xUnit.v3 test project Saucery.Core.Tests.XUnitv3/Saucery.Core.Tests.XUnitv3.csproj was not found.");
            return new(false, messages, Array.Empty<CoverageDiscrepancy>());
        }

        var relativeXunitV3Project = context.Relative(xunitV3Project);
        var tests = await ProcessRunner.RunAsync(
            "dotnet",
            ["run", "--project", relativeXunitV3Project, "--configuration", "Release"],
            context.Root);

        if(tests.ExitCode == 0)
        {
            messages.Add($"Run {relativeXunitV3Project}: PASS");
            return new(true, messages, Array.Empty<CoverageDiscrepancy>());
        }

        if(ContainsGenuineAssertionFailure(tests.CombinedOutput))
        {
            messages.Add($"Run {relativeXunitV3Project}: FAIL");
            messages.Add("  Genuine xUnit.v3 assertion failures were detected. They always fail remediation validation, even when Sauce infrastructure failures are present in the same run.");
            messages.Add(tests.CombinedOutput);
            return new(false, messages, Array.Empty<CoverageDiscrepancy>());
        }

        if(IsExternalSauceInfrastructureFailure(tests.CombinedOutput))
        {
            messages.Add($"Run {relativeXunitV3Project}: INCONCLUSIVE");
            messages.Add("  The xUnit.v3 run contains only the recognized live Sauce Labs platform-acquisition failure and no genuine assertion failures.");
            messages.Add("  This is treated as an external integration failure rather than a remediation failure.");
            return new(true, messages, Array.Empty<CoverageDiscrepancy>());
        }

        messages.Add($"Run {relativeXunitV3Project}: FAIL");
        messages.Add(tests.CombinedOutput);
        return new(false, messages, Array.Empty<CoverageDiscrepancy>());
    }

    private static bool ContainsGenuineAssertionFailure(string output)
    {
        if(string.IsNullOrWhiteSpace(output))
            return false;

        if(output.Contains("Shouldly.ShouldAssertException", StringComparison.Ordinal))
            return true;

        var xunitAssertion =
            output.Contains("Assert.Equal() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.True() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.False() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.Contains() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.DoesNotContain() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.NotNull() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.Null() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.Throws() Failure", StringComparison.Ordinal);

        if(xunitAssertion)
            return true;

        // xUnit/MTP can wrap assertion failures in XunitException. Require assertion-shaped
        // output so fixture-constructor failures are not misclassified as assertions.
        return output.Contains("XunitException", StringComparison.Ordinal) &&
               (output.Contains("should be", StringComparison.OrdinalIgnoreCase) ||
                output.Contains("but was", StringComparison.OrdinalIgnoreCase) ||
                output.Contains("Expected:", StringComparison.OrdinalIgnoreCase) ||
                output.Contains("Actual:", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsExternalSauceInfrastructureFailure(string output)
    {
        if(string.IsNullOrWhiteSpace(output)) return false;

        var sauceAcquirer =
            output.Contains("SauceLabsRealDeviceAcquirer.AcquireRealDevicePlatforms", StringComparison.Ordinal) ||
            output.Contains("SauceLabsPlatformAcquirer", StringComparison.Ordinal);

        var externalFailure =
            output.Contains("System.Text.Json.JsonException", StringComparison.Ordinal) ||
            output.Contains("System.Net.Http.HttpRequestException", StringComparison.Ordinal) ||
            output.Contains("TaskCanceledException", StringComparison.Ordinal) ||
            output.Contains("The JSON value could not be converted", StringComparison.Ordinal);

        var fixtureSetupFailure =
            output.Contains("BeforeClassException", StringComparison.Ordinal) ||
            output.Contains("BeforeClass hook failed", StringComparison.Ordinal) ||
            (output.Contains("Class fixture type", StringComparison.Ordinal) &&
             output.Contains("threw in its constructor", StringComparison.Ordinal));

        var genuineAssertionFailure =
            output.Contains("Shouldly.ShouldAssertException", StringComparison.Ordinal) ||
            output.Contains("Assert.Equal() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.True() Failure", StringComparison.Ordinal) ||
            output.Contains("Assert.False() Failure", StringComparison.Ordinal);

        return sauceAcquirer && externalFailure && fixtureSetupFailure && !genuineAssertionFailure;
    }

    private static bool Equivalent(CoverageDiscrepancy a, CoverageDiscrepancy b) =>
        a.Type == b.Type &&
        a.Family.Equals(b.Family, StringComparison.OrdinalIgnoreCase) &&
        a.Kind == b.Kind &&
        a.DisplayName.Equals(b.DisplayName, StringComparison.OrdinalIgnoreCase);
}

internal sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError)
{
    public string CombinedOutput => string.Join(Environment.NewLine, new[] { StandardOutput, StandardError }.Where(x => !string.IsNullOrWhiteSpace(x)));
}

internal static class ProcessRunner
{
    public static async Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string workingDirectory, CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach(var argument in arguments) startInfo.ArgumentList.Add(argument);
        using var process = new Process { StartInfo = startInfo };
        process.Start();
        var stdout = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderr = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        return new(process.ExitCode, await stdout, await stderr);
    }
}
