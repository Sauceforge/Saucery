using MyTestProject.PageObjects;
using Saucery.Core.Dojo;
using Saucery.XUnit3;
using Shouldly;

namespace MyTestProject;

public class ClickLinkTests(ITestContextAccessor output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
{
    [Theory]
    [MemberData(nameof(RequestedPlatformData.AllPlatforms), MemberType = typeof(RequestedPlatformData))]
    public void ClickLinkTest(BrowserVersion requestedPlatform)
    {
        InitialiseDriver(requestedPlatform, _testContextAccessor.Current?.Test!);

        var guineaPigPage = new GuineaPigPage(_baseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.ClickLink(_baseFixture.SauceryDriver());

        // verify the browser was navigated to the correct page
        _baseFixture.SauceryDriver().Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 28th June 2025
* 
*/