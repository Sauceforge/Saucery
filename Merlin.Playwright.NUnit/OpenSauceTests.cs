using Microsoft.Playwright;
using Saucery.Core.Dojo;
using Saucery.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace Merlin.Playwright.NUnit;

[Parallelizable(ParallelScope.Self)]
[TestFixtureSource(typeof(RequestedPlatformData))]
public partial class OpenSauceTests(BrowserVersion browserVersion) : SauceryBase(browserVersion)
{
    [Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingToTheIntroPage()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(PlayWrightRegEx());

        // create a locator
        var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(IntroRegex());
    }

    [Test]
    [TestCase(5)]
    [TestCase(4)]
    public async Task DataDrivenTest(int data)
    {
        await Page.GotoAsync("https://saucelabs.com/test/guinea-pig");

        var comments = Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "comments" });

        await comments.FillAsync(data.ToString());

        // verify the page title is correct - this is actually checked as part of the constructor above.
        await Expect(Page).ToHaveTitleAsync("I am a page title - Sauce Labs");
    }

    [GeneratedRegex("Playwright")]
    private static partial Regex PlayWrightRegEx();
    [GeneratedRegex(".*intro")]
    private static partial Regex IntroRegex();

    //Needs: Meziantou.Xunit.ParallelTestFramework NuGet package
    //private override BrowserNewContextOptions ContextOptions()
    //{
    //    return new BrowserNewContextOptions()
    //    {
    //        ColorScheme = ColorScheme.Dark,
    //        //ViewportSize = new()
    //        //{
    //        //    Width = 1920,
    //        //    Height = 1080
    //        //},
    //        //BaseURL = "https://github.com",

    //    };
    //}
}