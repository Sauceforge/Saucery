using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Saucery.Core.PageObjects;
using SeleniumExtras.PageObjects;

namespace MyTestProject.PageObjects;

public class GuineaPigPage : PageObjectBase {
    public GuineaPigPage(WebDriver driver, string urlRoot)
        : base(urlRoot + "test/guinea-pig", "GuineaPig", "I am a page title - Sauce Labs") {
        GetPage(driver);
        PageFactory.InitElements(driver, this);
    }

    public GuineaPigPage ClickLink(WebDriver driver) {
        //Could also use a "Selectors" class here.
        var link = driver.FindElement(By.Id("i am a link"));
        link.Click();
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        wait.Until(d => d.Url.Contains("guinea-pig2"));
        return this;
    }

    public IWebElement GetField(WebDriver driver, string fieldId) => driver.FindElement(By.Id(fieldId));

    public string GetUserAgent(WebDriver driver) =>
        //Could also use a "Selectors" class here.
        driver.FindElement(By.Id("useragent")).Text;

    public GuineaPigPage TypeField(WebDriver driver, string fieldId, string data) {
        var element = GetField(driver, fieldId);
        element.Clear();
        element.SendKeys(data);
        return this;
    }
}