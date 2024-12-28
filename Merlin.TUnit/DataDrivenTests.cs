using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;

namespace Merlin.TUnit;

public class DataDrivenTests : SauceryTBase
{
    [Test]
    [MethodDataSource(nameof(AllCombinations), Arguments = [new[] { 4, 5 }])]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());

        var commentField = guineaPigPage.GetField(SauceryDriver(), "comments");
        await Assert.That(commentField).IsNotNull();

        var commentText = commentField.GetDomProperty("value");
        await Assert.That(commentText).Contains(data.ToString());
    }

    public static IEnumerable<Func<(BrowserVersion, int)>> AllCombinations(int[] data) =>
        RequestedPlatformData
        .AllPlatforms()
        .SelectMany(
            browserVersionFunc => data,
            (browserVersionFunc, datum) => new Func<(BrowserVersion, int)>(() => (browserVersionFunc(), datum))
        );
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/