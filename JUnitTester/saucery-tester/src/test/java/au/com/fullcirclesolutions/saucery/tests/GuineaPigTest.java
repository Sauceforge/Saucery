package au.com.fullcirclesolutions.saucery.tests;

import org.junit.Assert;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class GuineaPigTest extends SauceryBase {

    public GuineaPigTest(String os, String platform, String browser, String browserVersion, String longName,
            String longVersion, String url, String device, String deviceOrientation) {
        super(os, platform, browser, browserVersion, longName, longVersion, url, device, deviceOrientation);
    }
    
    @Test
    public void PageTitle() {
        Driver.navigate().to("https://saucelabs.com/test/guinea-pig");

        // verify the page title is correct
        Assert.assertTrue(Driver.getTitle().contains("I am a page title - Sauce Labs"));
    }

    @Test
    public void LinkWorks() {
        Driver.navigate().to("https://saucelabs.com/test/guinea-pig");
        // find and click the link on the page
        WebElement link = Driver.findElement(By.id("i am a link"));
        link.click();

        // wait for the page to change
        WebDriverWait wait = new WebDriverWait(Driver, 10);
        wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("i_am_an_id")));

        // verify the browser was navigated to the correct page
        Assert.assertTrue(Driver.getCurrentUrl().contains("saucelabs.com/test-guinea-pig2.html"));
    }

    @Test
    public void UserAgentPresent() {
        Driver.navigate().to("https://saucelabs.com/test/guinea-pig");

        // read the useragent string off the page
        String useragent = Driver.findElement(By.id("useragent")).toString();

        Assert.assertNotNull(useragent);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */