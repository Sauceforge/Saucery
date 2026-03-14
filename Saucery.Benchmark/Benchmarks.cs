using System;
using System.Linq;
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
        int totalBrowserVersions = PlatformConfigurator.AvailablePlatforms
            .SelectMany(p => p.Browsers)
            .SelectMany(b => b.BrowserVersions)
            .Count();
        int totalRealDevices = PlatformConfigurator.AvailableRealDevices.Count;
        Console.WriteLine("All Platform count: {0}", totalBrowserVersions + totalRealDevices);
    }

    [Benchmark]
    public void EmulatedOnly()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.Emulated);
        int totalBrowserVersions = PlatformConfigurator.AvailablePlatforms
            .SelectMany(p => p.Browsers)
            .SelectMany(b => b.BrowserVersions)
            .Count();
        Console.WriteLine("Emulated Platform count: {0}", totalBrowserVersions);
    }

    [Benchmark]
    public void RealDeviceOnly()
    {
        PlatformConfigurator = new PlatformConfigurator(PlatformFilter.RealDevice);
        int totalRealDevices = PlatformConfigurator.AvailableRealDevices.Count;
        Console.WriteLine("Real Device Platform count: {0}", totalRealDevices);
    }
}