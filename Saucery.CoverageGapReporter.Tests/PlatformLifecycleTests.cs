using Shouldly;
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

    [Test]
    public void Real_device_major_kept_alive_only_by_simulators_is_reported_obsolete()
    {
        // Sauce still offers iOS 14 simulators, but its real-device inventory starts at iOS 15.
        // Before real devices and simulators were separated, the iOS 14.0 simulator kept the
        // "iOS 14" real-device declaration looking supported and it was never reported.
        var lifecycle = Analyze(
            [IOSSimulator("14.0"), IOSSimulator("15.0")],
            [RealIOS("15.8")],
            realDeviceInventoryRead: true);

        var obsolete = lifecycle.ObsoleteCandidates.ShouldHaveSingleItem();
        obsolete.DisplayName.ShouldBe("iOS 14.x");
        obsolete.Kind.ShouldBe(PlatformRequirementKind.MobileMajorWildcard);
    }

    [Test]
    public void Simulator_version_sauce_no_longer_offers_is_reported_obsolete()
    {
        var lifecycle = Analyze(
            [IOSSimulator("15.0")],
            [RealIOS("14.8"), RealIOS("15.8")],
            realDeviceInventoryRead: true);

        lifecycle.ObsoleteCandidates.Select(x => x.DisplayName).ShouldBe(["iOS 14.0"]);
    }

    [Test]
    public void Without_the_real_device_inventory_no_major_version_is_reported_obsolete()
    {
        // Every real-device declaration would look obsolete if the inventory could not be read.
        var lifecycle = Analyze(
            [IOSSimulator("14.0"), IOSSimulator("15.0")],
            [],
            realDeviceInventoryRead: false);

        lifecycle.ObsoleteCandidates.ShouldBeEmpty();
    }

    [Test]
    public void Platform_sauce_offers_in_both_inventories_is_not_obsolete()
    {
        var lifecycle = Analyze(
            [IOSSimulator("14.0"), IOSSimulator("15.0")],
            [RealIOS("14.8"), RealIOS("15.8")],
            realDeviceInventoryRead: true);

        lifecycle.ObsoleteCandidates.ShouldBeEmpty();
    }

    private static PlatformLifecycleResult Analyze(
        SauceVirtualRow[] virtualRows,
        SauceRealDevice[] realDevices,
        bool realDeviceInventoryRead)
    {
        using var core = new SourceFolder(("Dojo/DojoExtensions.cs", DojoExtensions));

        return PlatformLifecycleAnalyzer.Analyze(
            FunctionalPlatformBuilder.Build(virtualRows, realDevices),
            SauceryPlatformExtractor.Extract(core.Root),
            realDeviceInventoryRead);
    }
}
