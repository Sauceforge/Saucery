using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Shouldly;

namespace Saucery.Core.PageObjects;

public class PageObjectBase(string url, string name, string title)
{
    internal string Name = name;

    protected void GetPage(WebDriver driver)
    {
        driver.Navigate().GoToUrl(url);

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(1800)); 
        wait.Until(ExpectedConditions.TitleIs(title));
        driver.Title.ShouldBe(title);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 17th July 2014
* 
*/