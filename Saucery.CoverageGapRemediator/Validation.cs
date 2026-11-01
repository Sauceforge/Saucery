using System.Diagnostics;

namespace Saucery.CoverageGapRemediator;

internal sealed record ValidationResult(bool Success, IReadOnlyList<string> Messages);

internal sealed class RepositoryValidator
{
    public async Task<ValidationResult> ValidateAsync(
        RepositoryContext context,
        IReadOnlyList<CoverageDiscrepancy> originalDiscrepancies,
        Options options)
    {
        var messages = new List<string>();

        var build = await ProcessRunner.RunAsync(
            "dotnet",
            ["build", Path.Combine("Saucery.Core", "Saucery.Core.csproj"), "-c", "Release"],
            context.Root);
        messages.Add($"Build Saucery.Core: {(build.ExitCode == 0 ? "PASS" : "FAIL")}");
        if (build.ExitCode != 0)
        {
            messages.Add(build.CombinedOutput);
            return new ValidationResult(false, messages);
        }

        var testsProject = Path.Combine(context.Tests, "Saucery.Core.Tests.csproj");
        if (!File.Exists(testsProject))
        {
            messages.Add("Saucery.Core.Tests.csproj was not found; validation cannot continue.");
            return new ValidationResult(false, messages);
        }

        var tests = await ProcessRunner.RunAsync(
            "dotnet",
            ["test", Path.Combine("Saucery.Core.Tests", "Saucery.Core.Tests.csproj"), "-c", "Release"],
            context.Root);
        messages.Add($"Test Saucery.Core.Tests: {(tests.ExitCode == 0 ? "PASS" : "FAIL")}");
        if (tests.ExitCode != 0)
        {
            messages.Add(tests.CombinedOutput);
            return new ValidationResult(false, messages);
        }

        if (!options.VerifyWithReporter || !File.Exists(context.ReporterProject))
        {
            messages.Add("Coverage reporter verification: SKIPPED");
            return new ValidationResult(true, messages);
        }

        var reporter = await ProcessRunner.RunAsync(
            "dotnet",
            [
                "run",
                "--project", Path.Combine("Saucery.CoverageGapReporter", "Saucery.CoverageGapReporter.csproj"),
                "-c", "Release"
            ],
            context.Root);

        messages.Add($"Re-run CoverageGapReporter: {(reporter.ExitCode == 0 ? "PASS" : "FAIL")}");
        if (reporter.ExitCode != 0)
        {
            messages.Add(reporter.CombinedOutput);
            return new ValidationResult(false, messages);
        }

        var reportPath = Path.Combine(context.Root, "saucery-functional-platform-coverage-gaps.txt");
        if (!File.Exists(reportPath))
        {
            messages.Add("Reporter completed but the expected report file was not produced.");
            return new ValidationResult(false, messages);
        }

        var after = await CoverageReportReader.ReadAsync(reportPath);
        var unresolved = originalDiscrepancies
            .Where(original => after.Any(current => Equivalent(original, current)))
            .ToArray();

        if (unresolved.Length > 0)
        {
            messages.Add("Reporter verification: FAIL. These targeted discrepancies remain:");
            messages.AddRange(unresolved.Select(x => $"  - {x.Type}: {x.DisplayName}"));
            return new ValidationResult(false, messages);
        }

        messages.Add("Reporter verification: PASS. All targeted discrepancies disappeared.");
        return new ValidationResult(true, messages);
    }

    private static bool Equivalent(CoverageDiscrepancy a, CoverageDiscrepancy b) =>
        a.Type == b.Type &&
        a.Family.Equals(b.Family, StringComparison.OrdinalIgnoreCase) &&
        a.Kind == b.Kind &&
        a.DisplayName.Equals(b.DisplayName, StringComparison.OrdinalIgnoreCase);
}

internal sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError)
{
    public string CombinedOutput => string.Join(
        Environment.NewLine,
        new[] { StandardOutput, StandardError }.Where(x => !string.IsNullOrWhiteSpace(x)));
}

internal static class ProcessRunner
{
    public static async Task<ProcessResult> RunAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        string workingDirectory,
        CancellationToken cancellationToken = default)
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

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        return new ProcessResult(
            process.ExitCode,
            await stdoutTask,
            await stderrTask);
    }
}
