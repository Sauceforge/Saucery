using Merlin.TUnit.RealDevices;
using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;
using Shouldly;

[assembly: ParallelLimiter<MyParallelLimit>]

namespace Merlin.TUnit.RealDevices;

public class ClickLinkTests : SauceryTBase
{
    [Test]
    [MethodDataSource(typeof(RequestedPlatformData), nameof(RequestedPlatformData.AllPlatforms))]
    public async Task ClickLinkTest(BrowserVersion requestedPlatform)
    {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.ClickLink(SauceryDriver());

        // verify the browser was navigated to the correct page
        Driver!.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/