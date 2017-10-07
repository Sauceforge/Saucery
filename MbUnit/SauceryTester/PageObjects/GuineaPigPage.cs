using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Saucery.Driver;
using Saucery.PageObjects;

namespace SauceryTester.PageObjects
{
    public class GuineaPigPage : PageObjectBase {
#pragma warning disable 0649
        [FindsBy(How = How.Id, Using = "//*[@id='i am a link']")] private IWebElement _link;
        [FindsBy(How = How.Id, Using = "//*[@id='useragent']")] private IWebElement _userAgent;
#pragma warning restore 0649

        public GuineaPigPage(SauceryRemoteWebDriver driver, String urlRoot)
            : base(driver, urlRoot, "Guinea Pig", "I am a page title - Sauce Labs") {
            GetPage();
            CheckTitle();
            PageFactory.InitElements(Driver, this);
        }

        public GuineaPigPage ClickLink() {
            _link.Click();
            WaitForPageLoad(5);
            return this;
        }

        public IWebElement GetUserAgent() {
            return _userAgent;
        }
    }
}
