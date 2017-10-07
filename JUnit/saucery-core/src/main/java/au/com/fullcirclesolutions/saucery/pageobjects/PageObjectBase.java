package au.com.fullcirclesolutions.saucery.pageobjects;

import java.util.concurrent.TimeUnit;

import org.junit.Assert;
import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.SearchContext;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import au.com.fullcirclesolutions.saucery.driver.SauceryRemoteWebDriver;
import au.com.fullcirclesolutions.saucery.utils.WebControl;

public class PageObjectBase {

    protected SauceryRemoteWebDriver Driver;
    protected String PageURL;
    protected String Name;
    protected String Title;

    protected PageObjectBase(SauceryRemoteWebDriver driver, String url, String name, String title) {
        Driver = driver;
        PageURL = url;
        Name = name;
        Title = title;
    }

    public void GetPage() {
        Driver.navigate().to(PageURL);
        WaitForPageLoad(10);
    }

    public void GetPage(int secondsToWait) {
        Driver.navigate().to(PageURL);
        WaitForPageLoad(secondsToWait);
    }

    public void CheckTitle() {
        // Check that we're on the right page.
        if (!Title.equals(Driver.getTitle())) {
            // Alternatively, we could navigate to the login page, perhaps logging out first
            throw new IllegalStateException("This is not the " + Name + " page");
        }
    }

    public int GetSelectedCount() {
        return new WebControl(By.cssSelector("selected")).FindAll(Driver).size();
    }

    public void CheckTitle(String title1, String title2, String title3) {
        Assert.assertTrue(Driver.getTitle() == null || Driver.getTitle().equals("") || //Apple Devices have an empty Title on the PDF Page
                //Driver.Title.Contains(Constants.TEST_URL) || 
                Driver.getTitle().contains(title1)
                || Driver.getTitle().contains(title2)
                || Driver.getTitle().contains(title3));
    }

    protected void ScrollToTop() {
        Actions actions = new Actions(Driver);
        actions.keyDown(Keys.CONTROL).sendKeys(Keys.HOME).perform();
        System.out.println("Scrolled to top");
    }

    protected void ScrollToBottom() {
        Actions actions = new Actions(Driver);
        actions.keyDown(Keys.CONTROL).sendKeys(Keys.END).perform();
    }

    public boolean IsElementVisible(WebElement element) {
        return element.isDisplayed() && element.isEnabled();
    }

    public void WaitForPageLoad(int maxWaitTimeInSeconds) {
        Driver.manage().timeouts().pageLoadTimeout(maxWaitTimeInSeconds, TimeUnit.SECONDS);
    }

    protected void WaitUntilOptionsLoad(SearchContext dropdown) {
        WebDriverWait wait = new WebDriverWait(Driver, 10);
        wait.until(ExpectedConditions.elementToBeClickable(By.tagName("option")));
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 11th December 2013 
 */