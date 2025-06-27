using NUnit.Framework;
using Saucery;
using Saucery.Core.Dojo;
using Shouldly;
using sourceName.PageObjects;

[assembly: LevelOfParallelism(4)]

namespace sourceName;

[TestFixture]
[Parallelizable]
[TestFixtureSource(typeof(RequestedPlatformData))]
public class NuGetIntegrationTests(BrowserVersion browserVersion) : SauceryBase(browserVersion) {

    [Test]
    [TestCase(5)]
    [TestCase(4)]
    public void DataDrivenTest(int data) {
        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());
        // verify the page title is correct - this is actually checked as part of the constructor above.
        Driver?.Title.ShouldContain("I am a page title - Sauce Labs");
    }

    [Test]
    public void ClickLinkTest() {
        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");
        
        // find and click the link on the page
        guineaPigPage.ClickLink(SauceryDriver());

        // verify the browser was navigated to the correct page
        Driver?.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }
}