using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Shouldly;
using Xunit.Abstractions;

namespace ExternalMerlin.XUnit;

public class ClickLinkTests : SauceryXBase
{
    public ClickLinkTests(ITestOutputHelper output, BaseFixture baseFixture) : base(output, baseFixture)
    {
    }

    [Theory] 
    [MemberData(nameof(RequestedPlatformData.Platforms), MemberType = typeof(RequestedPlatformData))]
    public void ClickLinkTest(string os,
                              string platformNameForOption,
                              string browserName,
                              string name,
                              string automationBackend,
                              string deviceName,
                              string recommendedAppiumVersion,
                              List<string> supportedBackendVersions,
                              List<string> deprecatedBackendVersions,
                              string testName,
                              string deviceOrientation,
                              string screenResolution,
                              PlatformType platformType,
                              List<string> screenResolutions)
    {
        InitialiseDriver(new BrowserVersion(os, platformNameForOption, browserName, name, automationBackend, deviceName, recommendedAppiumVersion,
                                            supportedBackendVersions, deprecatedBackendVersions, testName, deviceOrientation, screenResolution,
                                            platformType, screenResolutions));

        var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.ClickLink(BaseFixture.SauceryDriver());

        // verify the browser was navigated to the correct page
        BaseFixture.Driver!.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/