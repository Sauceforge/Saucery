using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Saucery3.OnDemand;
using Saucery3.Tests;
using Saucery3Tests.PageObjects;

//
// Saucery3 can be downloaded from http://nuget.org/packages/Saucery3
// 30-day trial and enterprise-wide activation keys are available from http://fullcirclesolutions.com.au
//

//
// This is a simple class library C# file.
// Class is subclassed from SauceryBase (other options are SauceryAndroidBase and SauceryIOSBase for Appium testing)
// Class requires constructor as shown below
// Other than this there is nothing else required to use Saucery!
// More How To at http://fullcirclesolutions.com.au/howto.html
//
namespace Saucery3Tests {
	/// <summary>tests for the sauce labs guinea pig page</summary>
    public class SecondFixture : SauceryBase {
        public SecondFixture(SaucePlatform platform) : base(platform) {
        }

        #region Tests
        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void Test1() {
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // verify the page title is correct
            Assert.IsTrue(Driver.Title.Contains("I am a page title - Sauce Labs"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void Test2() {
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");
            // find and click the link on the page
            guineaPigPage.ClickLink();

            // verify the browser was navigated to the correct page
            Assert.IsTrue(Driver.Url.Contains("saucelabs.com/test-guinea-pig2.html"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void Test3() {
            var guineaPigPage = new GuineaPigPage(Driver, "https://saucelabs.com/");

            // read the useragent string off the page
            var useragent = guineaPigPage.GetUserAgent();

            Assert.IsNotNull(useragent);
        }

        #endregion
    }
}