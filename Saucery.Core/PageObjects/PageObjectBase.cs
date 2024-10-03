using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Shouldly;

namespace Saucery.Core.PageObjects;

public class PageObjectBase(string url, string name, string title)
{
    internal string Name = name;
    private WebDriverWait? Wait;

    protected void GetPage(WebDriver driver)
    {
        driver.Navigate().GoToUrl(url);

        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1800));
        Wait.Until(ExpectedConditions.TitleIs(title));
        driver.Title.ShouldBe(title);
        //CheckTitle(driver);
    }

    //private void CheckTitle(WebDriver driver) {
    //    Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1800));
    //    while (!Wait.Until(ExpectedConditions.TitleIs(Title)))
    //    {
    //        GetPage(driver);
    //    }
    //    driver.Title.ShouldBe(Title);
    //    //Assert.AreEqual(Title, driver.Title);
    //}
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 17th July 2014
* 
*/