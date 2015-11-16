using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Saucery3.OnDemand;
using Saucery3.Tests;

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
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");

            // verify the page title is correct
            Assert.IsTrue(Driver.Title.Contains("I am a page title - Sauce Labs"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void Test2() {
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");
            // find and click the link on the page
            var link = Driver.FindElement(By.Id("i am a link"));
            link.Click();

            // wait for the page to change
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.Url.Contains("guinea-pig2"));

            // verify the browser was navigated to the correct page
            Assert.IsTrue(Driver.Url.Contains("saucelabs.com/test-guinea-pig2.html"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void Test3() {
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");

            // read the useragent string off the page
            var useragent = Driver.FindElement(By.Id("useragent")).Text;

            Assert.IsNotNull(useragent);
        }

        #endregion
    }
}