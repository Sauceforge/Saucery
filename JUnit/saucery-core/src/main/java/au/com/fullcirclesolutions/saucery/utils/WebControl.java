package au.com.fullcirclesolutions.saucery.utils;

import java.util.List;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.remote.RemoteWebDriver;

public class WebControl {
    private final By _by;

    public WebControl(By by) {
        _by = by;
    }

    public WebElement Find(RemoteWebDriver driver) {
        return driver.findElement(_by);
    }

    public List<WebElement> FindAll(RemoteWebDriver driver) {
        return driver.findElements(_by);
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 14th August 2014
 */
