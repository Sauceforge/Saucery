using BenchmarkDotNet.Attributes;
using Saucery.Core.Dojo;

namespace Saucery.Benchmark;

[MemoryDiagnoser]
public class Benchmarks
{
    private PlatformConfigurator PlatformConfigurator { get; set; }

    [Benchmark(Baseline = true)]
    public void All()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.All);
    }

    [Benchmark]
    public void EmulatedOnly()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.Emulated);
    }

    [Benchmark]
    public void RealDeviceOnly()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.RealDevice);
    }
}