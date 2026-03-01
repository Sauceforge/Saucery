using System.Diagnostics;

namespace Saucery.TestFrameworkBenchmarks;

internal static class DotnetCli
{
    /// <summary>
    /// Executes: dotnet {args}. Returns exit code; throws if non-zero.
    /// Standard output/error are drained to avoid deadlocks, but discarded to keep memory stable.
    /// </summary>
    public static int RunAndAssertSuccess(string args, string workingDirectory)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = new Process { StartInfo = psi };

        if (!proc.Start())
            throw new InvalidOperationException("Failed to start dotnet process.");

        proc.OutputDataReceived += (_, __) => { };
        proc.ErrorDataReceived += (_, __) => { };
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        proc.WaitForExit();

        if (proc.ExitCode != 0)
            throw new InvalidOperationException($"dotnet {args} failed with exit code {proc.ExitCode}.");

        return proc.ExitCode;
    }
}
