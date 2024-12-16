using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;
using Shouldly;

namespace Merlin.TUnit;

public class DataDrivenTests : SauceryTBase
{
    [Test]
    [MethodDataSource(nameof(AllCombinations))]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());
    }

    public static IEnumerable<(BrowserVersion, int)> AllCombinations()
    {
        foreach (var browserVersion in SauceryTestData.Items)
        {
            foreach (var datum in new[] { 4, 5 })
            {
                yield return (browserVersion, datum);
            }
        }
    }


}

/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/