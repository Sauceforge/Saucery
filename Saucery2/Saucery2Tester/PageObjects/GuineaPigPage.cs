using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using Saucery2.Driver;
using Saucery2.PageObjects;

namespace Saucery2Tester.PageObjects {
    public class GuineaPigPage : PageObjectBase {
        public GuineaPigPage(SauceryRemoteWebDriver driver, string urlRoot)
            : base(driver, urlRoot + "test/guinea-pig", "GuineaPig", "I am a page title - Sauce Labs") {
            GetPage();
            CheckTitle();
            PageFactory.InitElements(Driver, this);
        }

        public GuineaPigPage ClickLink() {
            //Could also use a "Selectors" class here.
            var link = Driver.FindElement(By.Id("i am a link"));
            link.Click();
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.Url.Contains("guinea-pig2"));
            return this;
        }

        public string GetUserAgent() {
            //Could also use a "Selectors" class here.
            return Driver.FindElement(By.Id("useragent")).Text;
        }

        public GuineaPigPage TypeField(IWebElement field, string data) {
            field.Clear();
            field.SendKeys(data);
            return this;
        }
    }
}