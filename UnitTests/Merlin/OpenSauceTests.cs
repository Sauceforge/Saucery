using Merlin.PageObjects;
using NUnit.Framework;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Tests;
using Saucery.Util;
using Shouldly;
using System;
using System.Collections.Generic;

namespace Merlin
{
    public class OpenSauceTests : SauceryBase {
        public OpenSauceTests(BrowserVersion browserVersion) : base(browserVersion) {
            SetRequestedPlatforms(new List<SaucePlatform>
            {
                //Desktop Platforms
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "latest"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "87"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78"),
                new SaucePlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "13"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),

                //Mobile Platforms
                new SaucePlatform("Linux", "Chrome", "89", "Android", "Google Pixel 6 Pro GoogleAPI Emulator", "12.0", "", "Android", "1.22.1", "portrait"),
                //https://github.com/SeleniumHQ/selenium/issues/10460 
                //new SaucePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_11, "iPhone 13 Pro Max Simulator", "15.0", "", "iphone", "1.22.0", "portrait")
            });
        }

        [Test]
        [TestCase(5)]
        [TestCase(4)]
        //[Ignore("Need OpenSauce")]
        public void DataDrivenTitleTest(int data) {
            Console.WriteLine(@"My data is: " + data);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // verify the page title is correct - this is actually checked as part of the constructor above.
            Driver.Title.ShouldContain("I am a page title - Sauce Labs");
        }

        [Test]
        //[Ignore("Need OpenSauce")]
        public void ClickLinkTest() {
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
            
            // find and click the link on the page
            guineaPigPage.ClickLink(Driver);

            // verify the browser was navigated to the correct page
            Driver.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
        }

        [Test]
        //[Ignore("Need OpenSauce")]
        public void UserAgentTest() {
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // read the useragent string off the page
            var useragent = guineaPigPage.GetUserAgent(Driver);

            useragent.ShouldNotBeNull();
        }
    }
}