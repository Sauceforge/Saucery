using Shouldly;
using static Saucery.CoverageGapReporter.Tests.TestData;

namespace Saucery.CoverageGapReporter.Tests;

public sealed class FunctionalPlatformBuilderTests
{
    [Test]
    public void Simulator_rows_create_exact_versions_but_no_major_version()
    {
        var requirements = FunctionalPlatformBuilder.Build([IOSSimulator("14.3"), AndroidEmulator("9.0")], []);

        requirements.Select(r => r.DisplayName).Order().ShouldBe(["Android 9.0", "iOS 14.3"]);
        requirements.ShouldAllBe(r => r.Kind == PlatformRequirementKind.MobileExactVersion);
    }

    [Test]
    public void Real_devices_create_major_versions_but_no_exact_version()
    {
        var requirements = FunctionalPlatformBuilder.Build([], [RealIOS("15.7.1"), RealAndroid("10")]);

        requirements.Select(r => r.DisplayName).Order().ShouldBe(["Android 10.x", "iOS 15.x"]);
        requirements.ShouldAllBe(r => r.Kind == PlatformRequirementKind.MobileMajorWildcard);
    }

    [Test]
    [Arguments("Windows 2012", "Windows 8", "8")]
    [Arguments("Windows 2012 R2", "Windows 8.1", "8.1")]
    [Arguments("Windows 10", "Windows 10", "10")]
    public void Windows_server_hosts_map_to_the_windows_version_saucery_declares(string sauceOs, string displayName, string version)
    {
        var requirement = FunctionalPlatformBuilder.Build([Desktop(sauceOs, "chrome")], []).ShouldHaveSingleItem();

        requirement.DisplayName.ShouldBe(displayName);
        requirement.Version.ShouldBe(version);
        requirement.FamilyKey.ShouldBe("desktop:Windows");
    }

    [Test]
    public void Desktop_requirements_list_the_browsers_sauce_offers_on_that_os()
    {
        var requirements = FunctionalPlatformBuilder.Build(
            [
                Desktop("Windows 2012", "chrome"),
                Desktop("Windows 2012", "firefox"),
                Desktop("Windows 2012", "internet explorer"),
                Desktop("Windows 2012", "chrome"),
                Desktop("Windows 2012 R2", "MicrosoftEdge")
            ],
            []);

        requirements.Single(r => r.DisplayName == "Windows 8").SauceBrowsers.ShouldBe(["chrome", "firefox", "internet explorer"]);
        requirements.Single(r => r.DisplayName == "Windows 8.1").SauceBrowsers.ShouldBe(["MicrosoftEdge"]);
    }
}
