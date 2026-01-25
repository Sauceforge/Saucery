using MyTestProject.PageObjects;
using Saucery.Core.Dojo;
using Saucery.XUnit;
using Shouldly;
using Xunit.Abstractions;

namespace MyTestProject;

public class ClickLinkTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
{
    [Theory]
    [MemberData(nameof(RequestedPlatformData.AllPlatforms), MemberType = typeof(RequestedPlatformData))]
    public async Task ClickLinkTest(BrowserVersion requestedPlatform)
    {
        await InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.ClickLink(BaseFixture.SauceryDriver());

        // verify the browser was navigated to the correct page
        BaseFixture.SauceryDriver().Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/