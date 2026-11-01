using System.Diagnostics;

namespace Saucery.CoverageGapRemediator;

internal sealed class RepositoryValidator
{
    public async Task<ValidationResult> ValidateAsync(RepositoryContext context, IReadOnlyList<CoverageDiscrepancy> targets, Options options)
    {
        var messages = new List<string>();

        var build = await ProcessRunner.RunAsync("dotnet", ["build", Path.Combine("Saucery.Core", "Saucery.Core.csproj"), "-c", "Release"], context.Root);
        messages.Add($"Build Saucery.Core: {(build.ExitCode == 0 ? "PASS" : "FAIL")}");
        if(build.ExitCode != 0)
        {
            messages.Add(build.CombinedOutput);
            return new(false, messages, Array.Empty<CoverageDiscrepancy>());
        }

        var tests = await ProcessRunner.RunAsync("dotnet", ["test", Path.Combine("Saucery.Core.Tests", "Saucery.Core.Tests.csproj"), "-c", "Release"], context.Root);
        messages.Add($"Test Saucery.Core.Tests: {(tests.ExitCode == 0 ? "PASS" : "FAIL")}");
        if(tests.ExitCode != 0)
        {
            messages.Add(tests.CombinedOutput);
            return new(false, messages, Array.Empty<CoverageDiscrepancy>());
        }

        if(!options.VerifyWithReporter || !File.Exists(context.ReporterProject))
        {
            messages.Add("Coverage reporter verification: SKIPPED");
            return new(true, messages, Array.Empty<CoverageDiscrepancy>());
        }

        var reportPath = Path.Combine(context.Root, "saucery-functional-platform-coverage-gaps.txt");
        var reporter = await ProcessRunner.RunAsync("dotnet",
            ["run", "--project", Path.Combine("Saucery.CoverageGapReporter", "Saucery.CoverageGapReporter.csproj"), "--configuration", "Release", "--",
             "--repo-root", context.Root, "--output", reportPath, "--region", "us-west-1", "--include-real", "true"], context.Root);
        messages.Add($"Re-run CoverageGapReporter: {(reporter.ExitCode == 0 ? "PASS" : "FAIL")}");
        if(reporter.ExitCode != 0)
        {
            messages.Add(reporter.CombinedOutput);
            return new(false, messages, Array.Empty<CoverageDiscrepancy>());
        }

        var after = await CoverageReportReader.ReadAsync(reportPath);
        var remaining = targets.Where(t => after.Any(a => Equivalent(t, a))).ToArray();
        if(remaining.Length > 0)
        {
            messages.Add($"Reporter verification: {remaining.Length} targeted discrepancy(s) remain; another synthesis pass will be attempted.");
            messages.AddRange(remaining.Select(x => $"  - {x.Type}: {x.DisplayName}"));
            return new(false, messages, remaining);
        }

        messages.Add("Reporter verification: PASS. All targeted discrepancies disappeared.");
        return new(true, messages, Array.Empty<CoverageDiscrepancy>());
    }

    private static bool Equivalent(CoverageDiscrepancy a, CoverageDiscrepancy b) =>
        a.Type == b.Type && a.Family.Equals(b.Family, StringComparison.OrdinalIgnoreCase) && a.Kind == b.Kind &&
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
