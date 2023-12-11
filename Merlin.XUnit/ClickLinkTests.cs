using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Shouldly;
using Xunit.Abstractions;

namespace Merlin.XUnit;

public class ClickLinkTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture) //, IClassFixture<ConcurrencyFixture>
{
    [Theory]
    [MemberData(nameof(AllPlatforms))]
    public void ClickLinkTest(BrowserVersion requestedPlatform) {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.ClickLink(BaseFixture.SauceryDriver());

        // verify the browser was navigated to the correct page
        BaseFixture.Driver!.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }

    public static IEnumerable<object[]> AllPlatforms
    {
        get
        {
            List<object[]> allPlatforms = [];

            foreach(var platform in RequestedPlatformData.Items) {
                allPlatforms.Add([platform]);
            }

            return from c in allPlatforms
                   select c;
        }
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/