using System;
using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SauceryX.OnDemand;
using SauceryX.Tests;
using Xunit;

namespace SauceryXTester.Tests {
    public class SecondFixture : SauceryBase {
        public SecondFixture(SaucePlatform platform) : base(platform) {
        }

        #region Tests
        [MethodImpl(MethodImplOptions.Synchronized)]
        [Fact]
        public void Test1() {
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");

            // verify the page title is correct
            Assert.True(Driver.Title.Contains("I am a page title - Sauce Labs"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Fact]
        public void Test2() {
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");
            // find and click the link on the page
            var link = Driver.FindElement(By.Id("i am a link"));
            link.Click();

            // wait for the page to change
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.Url.Contains("guinea-pig2"));

            // verify the browser was navigated to the correct page
            Assert.True(Driver.Url.Contains("saucelabs.com/test-guinea-pig2.html"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [Fact]
        public void Test3() {
            Driver.Navigate().GoToUrl("https://saucelabs.com/test/guinea-pig");

            // read the useragent string off the page
            var useragent = Driver.FindElement(By.Id("useragent")).Text;

            Assert.NotNull(useragent);
        }

        #endregion
    }
}