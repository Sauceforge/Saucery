using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using NUnitLite;

namespace Saucery.FrameworkOnly.Microbench;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 3, iterationCount: 10)]
public class FrameworkOnlyBenchmarks
{
    private string _nunitAssemblyPath = "";
    private string _xunit2AssemblyPath = "";
    private string _xunit3AssemblyPath = "";

    private XunitRunner _xunitRunner = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _nunitAssemblyPath = typeof(Tests.NUnit.NUnitWorkloadTests).Assembly.Location;
        _xunit2AssemblyPath = typeof(Tests.XUnit2.XUnit2WorkloadTests).Assembly.Location;
        _xunit3AssemblyPath = typeof(Tests.XUnit3.XUnit3WorkloadTests).Assembly.Location;

        _xunitRunner = new XunitRunner();
    }

    [Benchmark(Description = "NUnit (NUnitLite AutoRun, in-process)")]
    public int NUnit_InProcess()
    {
        var args = new[]
        {
            _nunitAssemblyPath,
            "--labels=Off",
            "--trace=Off",
            "--workers=0",
            "--timeout=0"
        };

        var exit = new AutoRun().Execute(args);
        if (exit != 0)
            throw new InvalidOperationException($"NUnitLite returned exit code {exit}.");
        return exit;
    }

    [Benchmark(Baseline = true)]
    [Description("xUnit v2 (xunit.v3.runner.utility, in-process)")]
    public int XUnit2_InProcess()
        => _xunitRunner.RunAssembly(_xunit2AssemblyPath);

    [Benchmark]
    [Description("xUnit v3 (xunit.v3.runner.utility, in-process)")]
    public int XUnit3_InProcess()
        => _xunitRunner.RunAssembly(_xunit3AssemblyPath);
}
