using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Saucery.Driver;
using Saucery.Util;
using System;
using System.Linq;
using System.Threading;

namespace Saucery.PageObjects
{
    public class PageObjectBase {
        internal string PageUrl;
        internal string Name;
        internal string Title;

        public PageObjectBase(string url, string name, string title)
        {
            PageUrl = url;
            Name = name;
            Title = title;
        }

        public void GetPage(SauceryRemoteWebDriver driver) {
            driver.Navigate().GoToUrl(PageUrl);
            WaitForPageLoad(driver, 10);
        }

        public void CheckTitle(SauceryRemoteWebDriver driver) {
            while(driver.Title.Equals(SauceryConstants.APPLE_TITLE) || driver.PageSource.Contains("Let's browse!")) {
                GetPage(driver);
            }
            Assert.AreEqual(Title, driver.Title);
        }

        protected static void ScrollIntoView(SauceryRemoteWebDriver driver, IWebElement element) {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            Thread.Sleep(500);
        }
        
        protected static void WaitUntilElementClickable(SauceryRemoteWebDriver driver, IWebElement element) {
            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, 10));
            wait.Until(ElementIsClickable(element));
        }

        private static Func<IWebDriver, IWebElement> ElementIsClickable(IWebElement element) {
            return driver => (element != null && element.Displayed && element.Enabled) ? element : null;
        }

        protected static void ScrollToTop(SauceryRemoteWebDriver driver) {
            var actions = new Actions(driver);
            actions.KeyDown(Keys.Control).SendKeys(Keys.Home).Perform();
            Console.WriteLine(@"Scrolled to top");
        }

        protected static void ScrollToBottom(SauceryRemoteWebDriver driver) {
            var actions = new Actions(driver);
            actions.KeyDown(Keys.Control).SendKeys(Keys.End).Perform();
        }

        public static void WaitForPageLoad(SauceryRemoteWebDriver driver, int maxWaitTimeInSeconds) {
            var state = string.Empty;
            try {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));
                //Checks every 500 ms whether predicate returns true if returns exit otherwise keep trying till it returns true
                wait.Until(d => {
                    try {
                        state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();
                    } catch(InvalidOperationException) {
                        //Ignore
                    } catch(NoSuchWindowException) {
                        //when popup is closed, switch to last windows
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                    }
                    //In IE7 there are chances we may get state as loaded instead of complete
                    return (state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase));
                });
            } catch(TimeoutException) {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls 
                if(!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase)) {
                    throw;
                }
            } catch(NullReferenceException) {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls 
                if(!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase)) {
                    throw;
                }
            } catch(WebDriverException) {
                if(driver.WindowHandles.Count == 1) {
                    driver.SwitchTo().Window(driver.WindowHandles[0]);
                }
                state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();

                if(!(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) ||
                     state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase))) {
                    throw;
                }
            }
        }

        protected static void WaitUntilOptionsLoad(ISearchContext dropdown) {
            while(true) {
                Thread.Sleep(1000);
                var options = dropdown.FindElements(By.TagName("option"));
                if(options.Count > 0) {
                    //System.out.println("More than one option tag found; therefore options have loaded");
                    break;
                }
            }
        }

        public static bool IsElementVisible(IWebElement element) {
            return element.Displayed && element.Enabled;
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 17th July 2014
 * 
 */