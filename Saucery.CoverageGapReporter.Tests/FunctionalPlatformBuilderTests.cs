using Xunit;
using static Saucery.CoverageGapReporter.Tests.TestData;

namespace Saucery.CoverageGapReporter.Tests;

public sealed class FunctionalPlatformBuilderTests
{
    [Fact]
    public void Simulator_rows_create_exact_versions_but_no_major_version()
    {
        var requirements = FunctionalPlatformBuilder.Build([IOSSimulator("14.3"), AndroidEmulator("9.0")], []);

        Assert.Equal(["Android 9.0", "iOS 14.3"], requirements.Select(r => r.DisplayName).Order());
        Assert.All(requirements, r => Assert.Equal(PlatformRequirementKind.MobileExactVersion, r.Kind));
    }

    [Fact]
    public void Real_devices_create_major_versions_but_no_exact_version()
    {
        var requirements = FunctionalPlatformBuilder.Build([], [RealIOS("15.7.1"), RealAndroid("10")]);

        Assert.Equal(["Android 10.x", "iOS 15.x"], requirements.Select(r => r.DisplayName).Order());
        Assert.All(requirements, r => Assert.Equal(PlatformRequirementKind.MobileMajorWildcard, r.Kind));
    }

    [Theory]
    [InlineData("Windows 2012", "Windows 8", "8")]
    [InlineData("Windows 2012 R2", "Windows 8.1", "8.1")]
    [InlineData("Windows 10", "Windows 10", "10")]
    public void Windows_server_hosts_map_to_the_windows_version_saucery_declares(string sauceOs, string displayName, string version)
    {
        var requirement = Assert.Single(FunctionalPlatformBuilder.Build([Desktop(sauceOs, "chrome")], []));

        Assert.Equal(displayName, requirement.DisplayName);
        Assert.Equal(version, requirement.Version);
        Assert.Equal("desktop:Windows", requirement.FamilyKey);
    }

    [Fact]
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

        var windows8 = requirements.Single(r => r.DisplayName == "Windows 8");
        var windows81 = requirements.Single(r => r.DisplayName == "Windows 8.1");

        Assert.Equal(["chrome", "firefox", "internet explorer"], windows8.SauceBrowsers);
        Assert.Equal(["MicrosoftEdge"], windows81.SauceBrowsers);
    }
}
