using Xunit;
using static Saucery.CoverageGapReporter.Tests.TestData;

namespace Saucery.CoverageGapReporter.Tests;

public sealed class PlatformLifecycleTests
{
    // Saucery.Core maps simulators by exact version and real devices by major version only.
    private const string DojoExtensions = """
        public static class DojoExtensions
        {
            public static object? FindIOSBrowser(string label) => label switch
            {
                "iOS 15.0" => platforms.GetPlatform<IOS15Platform>().FirstOrDefault(),
                "iOS 14.0" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
                _ => null
            };

            public static object? FindIOSPlatform(string label) => label switch
            {
                "iOS 15" => platforms.GetPlatform<IOS15Platform>().FirstOrDefault(),
                "iOS 14" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
                _ => null
            };
        }
        """;

    [Fact]
    public void Real_device_major_kept_alive_only_by_simulators_is_reported_obsolete()
    {
        // Sauce still offers iOS 14 simulators, but its real-device inventory starts at iOS 15.
        // Before real devices and simulators were separated, the iOS 14.0 simulator kept the
        // "iOS 14" real-device declaration looking supported and it was never reported.
        using var core = new SourceFolder(("Dojo/DojoExtensions.cs", DojoExtensions));
        var requirements = FunctionalPlatformBuilder.Build(
            [IOSSimulator("14.0"), IOSSimulator("15.0")],
            [RealIOS("15.8")]);

        var lifecycle = PlatformLifecycleAnalyzer.Analyze(
            requirements,
            SauceryPlatformExtractor.Extract(core.Root),
            realDeviceInventoryRead: true);

        var obsolete = Assert.Single(lifecycle.ObsoleteCandidates);
        Assert.Equal("iOS 14.x", obsolete.DisplayName);
        Assert.Equal(PlatformRequirementKind.MobileMajorWildcard, obsolete.Kind);
    }

    [Fact]
    public void Simulator_version_sauce_no_longer_offers_is_reported_obsolete()
    {
        using var core = new SourceFolder(("Dojo/DojoExtensions.cs", DojoExtensions));
        var requirements = FunctionalPlatformBuilder.Build(
            [IOSSimulator("15.0")],
            [RealIOS("14.8"), RealIOS("15.8")]);

        var lifecycle = PlatformLifecycleAnalyzer.Analyze(
            requirements,
            SauceryPlatformExtractor.Extract(core.Root),
            realDeviceInventoryRead: true);

        Assert.Equal(["iOS 14.0"], lifecycle.ObsoleteCandidates.Select(x => x.DisplayName));
    }

    [Fact]
    public void Without_the_real_device_inventory_no_major_version_is_reported_obsolete()
    {
        // Every real-device declaration would look obsolete if the inventory could not be read.
        using var core = new SourceFolder(("Dojo/DojoExtensions.cs", DojoExtensions));
        var requirements = FunctionalPlatformBuilder.Build([IOSSimulator("14.0"), IOSSimulator("15.0")], []);

        var lifecycle = PlatformLifecycleAnalyzer.Analyze(
            requirements,
            SauceryPlatformExtractor.Extract(core.Root),
            realDeviceInventoryRead: false);

        Assert.Empty(lifecycle.ObsoleteCandidates);
    }

    [Fact]
    public void Platform_sauce_offers_in_both_inventories_is_not_obsolete()
    {
        using var core = new SourceFolder(("Dojo/DojoExtensions.cs", DojoExtensions));
        var requirements = FunctionalPlatformBuilder.Build(
            [IOSSimulator("14.0"), IOSSimulator("15.0")],
            [RealIOS("14.8"), RealIOS("15.8")]);

        var lifecycle = PlatformLifecycleAnalyzer.Analyze(
            requirements,
            SauceryPlatformExtractor.Extract(core.Root),
            realDeviceInventoryRead: true);

        Assert.Empty(lifecycle.ObsoleteCandidates);
    }
}
