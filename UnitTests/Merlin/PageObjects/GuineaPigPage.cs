using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Saucery.Driver;
using Saucery.PageObjects;
using SeleniumExtras.PageObjects;
using Shouldly;
using System;

namespace Merlin.PageObjects
{
    public class GuineaPigPage : PageObjectBase {
        public GuineaPigPage(SauceryRemoteWebDriver driver, string urlRoot)
            : base(urlRoot + "test/guinea-pig", "GuineaPig", "I am a page title - Sauce Labs") {
            GetPage(driver);
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

        public GuineaPigPage TypeField(SauceryRemoteWebDriver driver, string fieldId, string data) {
            var element = driver.FindElement(By.Id(fieldId));
            element.Clear();
            element.SendKeys(data);
            var val = element.GetAttribute("value");
            val.ShouldBeEquivalentTo(data);
            return this;
        }
    }
}