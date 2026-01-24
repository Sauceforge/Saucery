using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;

namespace Merlin.TUnit;

public class GuineaPigTests : SauceryTBase {
    [Test]
    [MethodDataSource(typeof(RequestedPlatformData), nameof(RequestedPlatformData.AllPlatforms))]
    public async Task ClickLinkTest(BrowserVersion requestedPlatform) {
        await InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.ClickLink(SauceryDriver());

        // verify the browser was navigated to the correct page
        await Assert.That(Driver!.Url).Contains("saucelabs.com/test-guinea-pig2.html");
    }

    [Test]
    [MethodDataSource(nameof(AllCombinations), Arguments = [new int[] { 4, 5 }])]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data) {
        await InitialiseDriver(requestedPlatform);

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
            _ => data,
            (browserVersionFunc, datum) => new Func<(BrowserVersion, int)>(() => (browserVersionFunc(), datum))
        );
}