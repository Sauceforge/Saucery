using ExternalMerlin.PageObjects;
using NUnit.Framework;
using Saucery.Dojo;
using Saucery.Tests;
using Shouldly;

//[assembly: LevelOfParallelism(3)]

namespace ExternalMerlin;

//[Parallelizable(ParallelScope.All)]
[TestFixtureSource(typeof(RequestedPlatformData))]
public class NuGetIntegrationTests : SauceryBase {
    public NuGetIntegrationTests(BrowserVersion browserVersion) : base(browserVersion) {
    }

    [Test]
    [TestCase(5)]
    [TestCase(4)]
    public void DataDrivenTitleTest(int data) {
        var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

        guineaPigPage.TypeField(Driver, "comments", data.ToString());
        // verify the page title is correct - this is actually checked as part of the constructor above.
        Driver.Title.ShouldContain("I am a page title - Sauce Labs");
    }

    [Test]
    public void ClickLinkTest() {
        var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
        
        // find and click the link on the page
        guineaPigPage.ClickLink(Driver);

        // verify the browser was navigated to the correct page
        Driver.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }

    [Test]
    public void UserAgentTest() {
        var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

        // read the useragent string off the page
        var useragent = guineaPigPage.GetUserAgent(Driver);

        useragent.ShouldNotBeNull();
    }
}