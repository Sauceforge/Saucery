using NUnit.Framework;
using Saucery;
using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Shouldly;

[assembly: LevelOfParallelism(3)]

namespace ExternalMerlin.NUnit;

//[Parallelizable(ParallelScope.All)]
[TestFixtureSource(typeof(RequestedPlatformData))]
public class NuGetIntegrationTests : SauceryBase {
    public NuGetIntegrationTests(BrowserVersion browserVersion) : base(browserVersion) {
    }

    [Test]
    [TestCase(5)]
    [TestCase(4)]
    public void DataDrivenTitleTest(int data) {
        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());
        // verify the page title is correct - this is actually checked as part of the constructor above.
        Driver.Title.ShouldContain("I am a page title - Sauce Labs");
    }

    [Test]
    public void ClickLinkTest() {
        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");
        
        // find and click the link on the page
        guineaPigPage.ClickLink(SauceryDriver());

        // verify the browser was navigated to the correct page
        Driver.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }

    [Test]
    [Ignore("Ignore")]
    public void UserAgentTest() {
        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        // read the useragent string off the page
        var useragent = guineaPigPage.GetUserAgent(SauceryDriver());

        useragent.ShouldNotBeNull();
    }
}