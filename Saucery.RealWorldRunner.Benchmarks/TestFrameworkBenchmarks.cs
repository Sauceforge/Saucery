using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Saucery.RealWorldRunner.Benchmarks;

/// <summary>
/// Benchmarks "full test-suite execution" time for the four Saucery test-framework projects.
/// 
/// IMPORTANT:
/// - This benchmarks end-to-end execution (process start + test discovery + execution + teardown).
/// - Setup builds everything once; benchmark iterations run with --no-build.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 1, iterationCount: 5)]
public class TestFrameworkBenchmarks
{
    private RepoLayout _layout = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _layout = RepoLayout.DiscoverFromCurrentDirectory();
        _layout.BuildAllReleaseOnce();
    }

    [Benchmark(Baseline = true)]
    [Description("TUnit (dotnet run)")]
    public int TUnit_DotnetRun() =>
        DotnetCli.RunAndAssertSuccess(
            args: $"run -c Release --no-build --project \"{_layout.TUnitProject}\"",
            workingDirectory: _layout.RepoRoot);

    [Benchmark]
    [Description("NUnit (dotnet test)")]
    public int NUnit_DotnetTest() =>
        DotnetCli.RunAndAssertSuccess(
            args: $"test \"{_layout.NUnitProject}\" -c Release --no-build --nologo",
            workingDirectory: _layout.RepoRoot);

    [Benchmark]
    [Description("xUnit (dotnet test)")]
    public int XUnit_DotnetTest() =>
        DotnetCli.RunAndAssertSuccess(
            args: $"test \"{_layout.XUnitProject}\" -c Release --no-build --nologo",
            workingDirectory: _layout.RepoRoot);

    [Benchmark]
    [Description("xUnit.v3 (dotnet test)")]
    public int XUnitV3_DotnetTest() =>
        DotnetCli.RunAndAssertSuccess(
            args: $"test \"{_layout.XUnitV3Project}\" -c Release --no-build --nologo",
            workingDirectory: _layout.RepoRoot);
}
