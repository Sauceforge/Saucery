using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Saucery2.Tests;
using Saucery2.ValueSources;
using Saucery2Tests.PageObjects;

//
// Saucery2 can be downloaded from http://nuget.org/packages/Saucery2
// 30-day trial and enterprise-wide activation keys are available from http://fullcirclesolutions.com.au
//

//
// This is a simple class library C# file.
// Class is subclassed from SauceryBase (other options are SauceryAndroidBase and SauceryIOSBase for Appium testing)
// One of the parameters to each test needs to be the ValueSource calling GetPlatforms.
// This platform parameter needs to be passed to the Setup method 
// Other than this there is nothing else required to use Saucery
// More How To at http://fullcirclesolutions.com.au/howto.html
//
namespace Saucery2Tests {
    /// <summary>tests for the sauce labs guinea pig page</summary>
    public class GuineaPigTests : SauceryBase {
        #region Tests
        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void PageTitle([ValueSource(typeof(PlatformTestData), "GetPlatforms")] PlatformTestData platform) {
            Setup(platform);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
            
            // verify the page title is correct - this is actually checked as part of the constructor above.
            Driver.Title.ShouldContain("I am a page title - Sauce Labs");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void LinkWorks([ValueSource(typeof(PlatformTestData), "GetPlatforms")] PlatformTestData platform) {
            Setup(platform);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
            // find and click the link on the page
            guineaPigPage.ClickLink();

            // verify the browser was navigated to the correct page
            Assert.IsTrue(Driver.Url.Contains("saucelabs.com/test-guinea-pig2.html"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void UserAgentPresent([ValueSource(typeof(PlatformTestData), "GetPlatforms")] PlatformTestData platform) {
            Setup(platform);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // read the useragent string off the page
            var useragent = guineaPigPage.GetUserAgent();
			
            Assert.IsNotNull(useragent);
        }

        #endregion
    }
}