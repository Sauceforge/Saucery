using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Shouldly;

namespace Saucery.Core.PageObjects;

public class PageObjectBase {
    internal string PageUrl;
    internal string Name;
    internal string Title;
    internal WebDriverWait? Wait;

    public PageObjectBase(string url, string name, string title)
    {
        PageUrl = url;
        Name = name;
        Title = title;
    }

    public void GetPage(WebDriver driver) {
        driver.Navigate().GoToUrl(PageUrl);
        CheckTitle(driver);
    }

    public void CheckTitle(WebDriver driver) {
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        while (!Wait.Until(ExpectedConditions.TitleIs(Title)))
        {
            GetPage(driver);
        }
        driver.Title.ShouldBe(Title);
        //Assert.AreEqual(Title, driver.Title);
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
/*
* Copyright Andrew Gray, SauceForge
* Date: 17th July 2014
* 
*/