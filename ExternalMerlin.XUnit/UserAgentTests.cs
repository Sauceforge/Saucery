using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Shouldly;
using Xunit.Abstractions;

//[assembly: CollectionBehavior(MaxParallelThreads = 4)]

namespace ExternalMerlin.XUnit;

public class UserAgentTests : SauceryXBase
{
    public UserAgentTests(ITestOutputHelper output, BaseFixture baseFixture) : base(output, baseFixture)
    {
    }

    //[Theory, ClassData(typeof(RequestedPlatformData))]

    //[Theory, MemberData(nameof(RequestedPlatformData.Platforms), MemberType = typeof(RequestedPlatformData))]
    //public void UserAgentTest(string os,
    //                          string platformNameForOption,
    //                          string browserName,
    //                          string name,
    //                          string automationBackend,
    //                          string deviceName,
    //                          string recommendedAppiumVersion,
    //                          List<string> supportedBackendVersions,
    //                          List<string> deprecatedBackendVersions,
    //                          string testName,
    //                          string deviceOrientation,
    //                          string screenResolution,
    //                          PlatformType platformType,
    //                          List<string> screenResolutions)
    //{
    //    InitialiseDriver(new BrowserVersion(os, platformNameForOption, browserName, name, automationBackend, deviceName, recommendedAppiumVersion,
    //                                        supportedBackendVersions, deprecatedBackendVersions, testName, deviceOrientation, screenResolution,
    //                                        platformType, screenResolutions));

    //    var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

    //    // read the useragent string off the page
    //    var useragent = guineaPigPage.GetUserAgent(BaseFixture.SauceryDriver());

    //    useragent.ShouldNotBeNull();
    //}
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/