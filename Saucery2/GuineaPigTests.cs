using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Saucery2.Tests;
using Saucery2.ValueSources;

//
// Saucery2 can be downloaded from http://nuget.org/packages/Saucery2
// An activation key is available from http://fullcirclesolutions.com.au
//
namespace Saucery2Tests {
    /// <summary>tests for the sauce labs guinea pig page</summary>
    public class GuineaPigTests : SauceryBase {
        #region Tests
        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void PageTitle([ValueSource(typeof(PlatformTestData), "GetPlatforms")] PlatformTestData platform) {
            Setup(platform);
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");

            // verify the page title is correct
            Assert.IsTrue(Driver.Title.Contains("I am a page title - Sauce Labs"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Test]
        public void LinkWorks([ValueSource(typeof(PlatformTestData), "GetPlatforms")] PlatformTestData platform) {
            Setup(platform);
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
        public void UserAgentPresent([ValueSource(typeof(PlatformTestData), "GetPlatforms")] PlatformTestData platform) {
            Setup(platform);
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");

            // read the useragent string off the page
            var useragent = Driver.FindElement(By.Id("useragent")).Text;

            Assert.IsNotNull(useragent);
        }

        #endregion
    }
}