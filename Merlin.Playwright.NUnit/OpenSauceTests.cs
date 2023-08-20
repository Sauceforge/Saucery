using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Saucery.Core.Dojo;
using Saucery.Playwright;
using System.Text.RegularExpressions;

namespace Merlin.Playwright.NUnit;

[Parallelizable(ParallelScope.Self)]
//[Parallelizable(ParallelScope.All)]
[TestFixtureSource(typeof(RequestedPlatformData))]
//public class OpenSauceTests : PageTest
public class OpenSauceTests : SauceryBase
{
    //public OpenSauceTests()
    //{

    //}

    public OpenSauceTests(BrowserVersion browserVersion) : base(browserVersion)
    {
    }

    [Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        // create a locator
        var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }

    [Test]
    [TestCase(5)]
    [TestCase(4)]
    public async Task DataDrivenTitleTest(int data)
    {
        await Page.GotoAsync("https://saucelabs.com/test/guinea-pig");

        var comments = Page.GetByRole(AriaRole.Textbox, new() { Name = "comments" });

        await comments.TypeAsync(data.ToString());

        // verify the page title is correct - this is actually checked as part of the constructor above.
        await Expect(Page).ToHaveTitleAsync("I am a page title - Sauce Labs");
    }
}