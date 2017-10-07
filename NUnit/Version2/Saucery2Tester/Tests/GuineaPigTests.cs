using System.Runtime.CompilerServices;
using NUnit.Framework;
using Saucery2.OnDemand;
using Saucery2.Tests;
using Saucery2Tester.PageObjects;
using Shouldly;

namespace Saucery2Tester.Tests {
    /// <summary>tests for the sauce labs guinea pig page</summary>
    public class GuineaPigTests : SauceryBase {
        #region Tests
        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void PageTitle([ValueSource(typeof(SaucePlatform), "GetPlatforms")] SaucePlatform platform) {
            Setup(platform);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // verify the page title is correct - this is actually checked as part of the constructor above.
            Driver.Title.ShouldContain("I am a page title - Sauce Labs");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void LinkWorks([ValueSource(typeof(SaucePlatform), "GetPlatforms")] SaucePlatform platform) {
            Setup(platform);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
            // find and click the link on the page
            guineaPigPage.ClickLink();

            // verify the browser was navigated to the correct page
            Assert.IsTrue(Driver.Url.Contains("saucelabs.com/test-guinea-pig2.html"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void UserAgentPresent([ValueSource(typeof(SaucePlatform), "GetPlatforms")] SaucePlatform platform) {
            Setup(platform);
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // read the useragent string off the page
            var useragent = guineaPigPage.GetUserAgent();
            
            Assert.IsNotNull(useragent);
        }

        #endregion
    }
}