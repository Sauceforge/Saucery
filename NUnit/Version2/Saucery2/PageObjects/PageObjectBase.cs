using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Saucery2.Driver;
using Saucery2.Util;

namespace Saucery2.PageObjects {
    public class PageObjectBase {
        public SauceryRemoteWebDriver Driver;
        //internal TouchCapableRemoteWebDriver TouchCapableDriver;
        internal string PageUrl;
        internal string Name;
        internal string Title;

        public PageObjectBase(SauceryRemoteWebDriver driver, string url, string name, string title) {
            Driver = driver;
            PageUrl = url;
            Name = name;
            Title = title;
        }

        public void GetPage() {
            Driver.Navigate().GoToUrl(PageUrl);
            WaitForPageLoad(10);
        }
        
        public void GetPage(int secondsToWait) {
            Driver.Navigate().GoToUrl(PageUrl);
            WaitForPageLoad(secondsToWait);
        }

        public void CheckTitle() {
            while(Driver.Title.Equals(SauceryConstants.APPLE_TITLE)) {
                GetPage();
            }
            Assert.AreEqual(Title, Driver.Title);
        }

        public int GetSelectedCount() {
            return new WebControl(By.CssSelector("selected")).FindAll(Driver).Count;
        }

        public void CheckTitle(string title1, string title2, string title3) {
            Assert.IsTrue(string.IsNullOrEmpty(Driver.Title) || //Apple Devices have an empty Title on the PDF Page
                          //Driver.Title.Contains(Constants.TEST_URL) || 
                          Driver.Title.Contains(title1) || 
                          Driver.Title.Contains(title2) || 
                          Driver.Title.Contains(title3));
        }

        protected void ScrollIntoView(IWebElement element) {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            Thread.Sleep(500);
        }
        
        protected void WaitUntilElementClickable(IWebElement element) {
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 0, 10));
            wait.Until(ElementIsClickable(element));
        }

        private static Func<IWebDriver, IWebElement> ElementIsClickable(IWebElement element) {
            return driver => (element != null && element.Displayed && element.Enabled) ? element : null;
        }

        protected void ScrollToTop() {
            var actions = new Actions(Driver);
            actions.KeyDown(Keys.Control).SendKeys(Keys.Home).Perform();
            Console.WriteLine(@"Scrolled to top");
        }

        protected void ScrollToBottom() {
            var actions = new Actions(Driver);
            actions.KeyDown(Keys.Control).SendKeys(Keys.End).Perform();
        }

        public void WaitForPageLoad(int maxWaitTimeInSeconds) {
            var state = string.Empty;
            try {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));
                //Checks every 500 ms whether predicate returns true if returns exit otherwise keep trying till it returns true
                wait.Until(d => {
                    try {
                        state = ((IJavaScriptExecutor)Driver).ExecuteScript(@"return document.readyState").ToString();
                    } catch(InvalidOperationException) {
                        //Ignore
                    } catch(NoSuchWindowException) {
                        //when popup is closed, switch to last windows
                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
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
                if(Driver.WindowHandles.Count == 1) {
                    Driver.SwitchTo().Window(Driver.WindowHandles[0]);
                }
                state = ((IJavaScriptExecutor)Driver).ExecuteScript(@"return document.readyState").ToString();

                if(!(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) ||
                     state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase))) {
                    throw;
                }
            }
        }

        protected void WaitUntilOptionsLoad(ISearchContext dropdown) {
            while(true) {
                Thread.Sleep(1000);
                var options = dropdown.FindElements(By.TagName("option"));
                if(options.Count > 0) {
                    //System.out.println("More than one option tag found; therefore options have loaded");
                    break;
                }
            }
        }

        public bool IsElementVisible(IWebElement element) {
            return element.Displayed && element.Enabled;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 17th July 2014
 * 
 */