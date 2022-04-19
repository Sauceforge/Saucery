using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using Saucery.Driver;
using SeleniumExtras.WaitHelpers;
using System;

namespace Saucery.PageObjects
{
    public class PageObjectBase {
        internal string PageUrl;
        internal string Name;
        internal string Title;
        internal WebDriverWait wait;

        public PageObjectBase(string url, string name, string title)
        {
            PageUrl = url;
            Name = name;
            Title = title;
        }

        public void GetPage(SauceryRemoteWebDriver driver) {
            driver.Navigate().GoToUrl(PageUrl);
            CheckTitle(driver);
        }

        public void CheckTitle(SauceryRemoteWebDriver driver) {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            while (!wait.Until(ExpectedConditions.TitleIs(Title)))
            {
                GetPage(driver);
            }
            Assert.AreEqual(Title, driver.Title);
        }

        //protected static void ScrollIntoView(SauceryRemoteWebDriver driver, IWebElement element) {
        //    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        //    Thread.Sleep(500);
        //}
        
        //protected static void WaitUntilElementClickable(SauceryRemoteWebDriver driver, IWebElement element) {
        //    var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, 10));
        //    wait.Until(ElementIsClickable(element));
        //}

        //private static Func<IWebDriver, IWebElement> ElementIsClickable(IWebElement element) {
        //    return driver => (element != null && element.Displayed && element.Enabled) ? element : null;
        //}

        //protected static void ScrollToTop(SauceryRemoteWebDriver driver) {
        //    var actions = new Actions(driver);
        //    actions.KeyDown(Keys.Control).SendKeys(Keys.Home).Perform();
        //    Console.WriteLine(@"Scrolled to top");
        //}

        //protected static void ScrollToBottom(SauceryRemoteWebDriver driver) {
        //    var actions = new Actions(driver);
        //    actions.KeyDown(Keys.Control).SendKeys(Keys.End).Perform();
        //}

        //protected static void WaitUntilOptionsLoad(ISearchContext dropdown)
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(1000);
        //        var options = dropdown.FindElements(By.TagName("option"));
        //        if (options.Count > 0)
        //        {
        //            //System.out.println("More than one option tag found; therefore options have loaded");
        //            break;
        //        }
        //    }
        //}

        //public static bool IsElementVisible(IWebElement element) {
        //    return element.Displayed && element.Enabled;
        //}
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 17th July 2014
 * 
 */