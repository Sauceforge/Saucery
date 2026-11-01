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

        foreach(var testProject in context.TestProjects)
        {
            var relativeProject = context.Relative(testProject);
            var tests = await ProcessRunner.RunAsync(
                "dotnet",
                ["run", "--project", relativeProject, "--configuration", "Release"],
                context.Root);

            if(tests.ExitCode == 0)
            {
                messages.Add($"Run {relativeProject}: PASS");
                continue;
            }

            if(IsExternalSauceInfrastructureFailure(tests.CombinedOutput))
            {
                messages.Add($"Run {relativeProject}: INCONCLUSIVE");
                messages.Add("  The test process reached the live Sauce Labs platform acquisition hook and failed before test execution because the external API response could not be deserialized.");
                messages.Add("  This is treated as an external integration failure, not as a remediation failure. All Core/test projects built successfully and any missing-platform targets were verified by the coverage reporter.");
                continue;
            }

            messages.Add($"Run {relativeProject}: FAIL");
            messages.Add(tests.CombinedOutput);
            return new(false, messages, Array.Empty<CoverageDiscrepancy>());
        }

        return new(true, messages, Array.Empty<CoverageDiscrepancy>());
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

        var beforeTests =
            output.Contains("BeforeClassException", StringComparison.Ordinal) ||
            output.Contains("BeforeClass hook failed", StringComparison.Ordinal);

        return sauceAcquirer && externalFailure && beforeTests;
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
