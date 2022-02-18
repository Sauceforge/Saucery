using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Saucery.Driver;
using Saucery.PageObjects;
using SeleniumExtras.PageObjects;

namespace Merlin.PageObjects {
    public class GuineaPigPage : PageObjectBase {
        public GuineaPigPage(SauceryRemoteWebDriver driver, string urlRoot)
            : base(urlRoot + "test/guinea-pig", "GuineaPig", "I am a page title - Sauce Labs") {
            GetPage(driver);
            CheckTitle(driver);
            PageFactory.InitElements(driver, this);
        }

        public GuineaPigPage ClickLink(SauceryRemoteWebDriver driver) {
            //Could also use a "Selectors" class here.
            var link = driver.FindElement(By.Id("i am a link"));
            link.Click();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.Url.Contains("guinea-pig2"));
            return this;
        }

        public string GetUserAgent(SauceryRemoteWebDriver driver) {
            //Could also use a "Selectors" class here.
            return driver.FindElement(By.Id("useragent")).Text;
        }

        public GuineaPigPage TypeField(IWebElement field, string data) {
            field.Clear();
            field.SendKeys(data);
            return this;
        }
    }
}