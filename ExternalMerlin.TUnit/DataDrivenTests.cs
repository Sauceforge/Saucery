using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;

namespace ExternalMerlin.TUnit;

public class DataDrivenTests : SauceryTBase
{
    [Test]
    [MethodDataSource(nameof(AllCombinations), Arguments = [new[] { 4, 5 }])]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());
    }

    public static IEnumerable<(BrowserVersion, int)> AllCombinations(int[] data)
    {
        foreach (var browserVersion in RequestedPlatformData.AllPlatformsAsList())
        {
            foreach (var datum in data)
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