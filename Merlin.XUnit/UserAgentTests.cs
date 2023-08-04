using Saucery.Core.Dojo;
using Saucery.Core.Driver;
using Saucery.Core.OnDemand;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Shouldly;
using Xunit.Abstractions;

//[assembly: CollectionBehavior(MaxParallelThreads = 4)]

namespace Merlin.XUnit;

public class UserAgentTests : SauceryXBase
{
    public UserAgentTests(ITestOutputHelper output, BaseFixture baseFixture) : base(output, baseFixture)
    {
    }

    //[Theory, ClassData(typeof(RequestedPlatformData))]
    [Theory, MemberData(nameof(RequestedPlatformData.Platforms), MemberType = typeof(RequestedPlatformData))]
    public void UserAgentTest(string os,
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

        var guineaPigPage = new GuineaPigPage((SauceryRemoteWebDriver)BaseFixture.Driver, "https://saucelabs.com/");

        // read the useragent string off the page
        var useragent = guineaPigPage.GetUserAgent((SauceryRemoteWebDriver)BaseFixture.Driver);

        useragent.ShouldNotBeNull();
    }


    //[Test]
    //[TestCase(5)]
    //[TestCase(4)]
    //public void DataDrivenTitleTest(int data)
    //{
    //    var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

    //    guineaPigPage.TypeField(Driver, "comments", data.ToString());
    //    // verify the page title is correct - this is actually checked as part of the constructor above.
    //    Driver.Title.ShouldContain("I am a page title - Sauce Labs");
    //}
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/