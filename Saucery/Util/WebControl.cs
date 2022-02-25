using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.ObjectModel;

namespace Saucery.Util
{
    public class WebControl {
        private readonly By _by;

        public WebControl(By by){
            _by = by;
        }

        public IWebElement Find(RemoteWebDriver driver){
            return driver.FindElement(_by);
        }

        public ReadOnlyCollection<IWebElement> FindAll(RemoteWebDriver driver){
            return driver.FindElements(_by);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 14th August 2014
 * 
 */