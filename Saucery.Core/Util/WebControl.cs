using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.ObjectModel;

namespace Saucery.Core.Util;

public class WebControl(By by)
{
    private readonly By _by = by;

    public IWebElement Find(RemoteWebDriver driver) => 
        driver.FindElement(_by);

    public ReadOnlyCollection<IWebElement> FindAll(RemoteWebDriver driver) => 
        driver.FindElements(_by);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 14th August 2014
* 
*/