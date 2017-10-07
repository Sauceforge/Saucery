using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

//TODO: Do extension methods attached to RemoteWebDriver extend to subclasses of RemoteWebDriver?

namespace Saucery2.Selenium {
    internal static class SeleniumExtensions {
        public static bool JQueryLoaded(this RemoteWebDriver driver) {
            var result = false;
            try {
                result = (bool)driver.ExecuteScript("return typeof jQuery == 'function'");
            } catch(WebDriverException) {
            }
            return result;
        }

        public static void LoadJQuery(this RemoteWebDriver driver, string version = "any", TimeSpan? timeout = null) {
            //Get the url to load jQuery from
            var jQueryUrl = string.IsNullOrEmpty(version) || version.ToLower() == "latest"
                ? "http://code.jquery.com/jquery-latest.min.js"
                : "https://ajax.googleapis.com/ajax/libs/jquery/" + version + "/jquery.min.js";

            //Script to load jQuery from external site
            var versionEnforceScript = version != null && version.ToLower() != "any"
                ? string.Format("if (typeof jQuery == 'function' && jQuery.fn.jquery != '{0}') jQuery.noConflict(true);", version)
                : string.Empty;
            var loadingScript =
                @"if (typeof jQuery != 'function')
                  {
                      headID.appendChild(newScript);
                      var headID = document.getElementsByTagName('head')[0];
                      var newScript = document.createElement('script');
                      newScript.type = 'text/javascript';
                      newScript.src = '" + jQueryUrl + @"';
                  }
                  return (typeof jQuery == 'function');";

            var loaded = (bool)driver.ExecuteScript(versionEnforceScript + loadingScript);

            if(!loaded) {
                //Wait for the script to load
                //Verify library loaded
                if(!timeout.HasValue) {
                    timeout = new TimeSpan(0, 0, 30);
                }

                var timePassed = 0;
                while(!driver.JQueryLoaded()) {
                    Thread.Sleep(500);
                    timePassed += 500;

                    if(timePassed > timeout.Value.TotalMilliseconds) {
                        throw new Exception("Could not load jQuery");
                    }
                }
            }

            var s = driver.ExecuteScript("return jQuery.fn.jquery").ToString();
        }

        public static IWebElement FindElement(this RemoteWebDriver driver, By.JQueryBy by) {
            //First make sure we can use jQuery functions
            driver.LoadJQuery();

            //Execute the jQuery selector as a script
            var element = driver.ExecuteScript("return $(\"" + by.Selector + "\").get(0)") as IWebElement;

            if(element != null) {
                return element;
            }
            throw new NoSuchElementException("No element found with jQuery command: jQuery" + by.Selector);
        }

        public static ReadOnlyCollection<IWebElement> FindElements(this RemoteWebDriver driver, By.JQueryBy by) {
            //First make sure we can use jQuery functions
            driver.LoadJQuery();

            //Execute the jQuery selector as a script
            var collection =
                driver.ExecuteScript("return $(\"" + by.Selector + "\").get()") as ReadOnlyCollection<IWebElement> ??
                new ReadOnlyCollection<IWebElement>(new List<IWebElement>());

            //Unlike FindElement, FindElements does not throw an exception if no elements are found
            //and instead returns an empty list
            return collection;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 27th July 2014
 * 
 */