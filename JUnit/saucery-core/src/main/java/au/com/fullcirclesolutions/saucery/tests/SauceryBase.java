package au.com.fullcirclesolutions.saucery.tests;

import java.net.URL;
import java.util.concurrent.TimeUnit;

import org.junit.After;
import org.junit.runner.RunWith;
import org.openqa.selenium.remote.DesiredCapabilities;

import au.com.fullcirclesolutions.saucery.driver.SauceryRemoteWebDriver;
import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
//See http://repository-saucelabs.forge.cloudbees.com/release/com/saucelabs/sauce_junit for latest version
//See http://repository-saucelabs.forge.cloudbees.com/release/com/saucelabs/sauce_java_common for latest version
import com.saucelabs.common.SauceOnDemandSessionIdProvider;
import com.saucelabs.junit.ConcurrentParameterized;

@RunWith(ConcurrentParameterized.class)
public class SauceryBase extends SauceryRoot implements SauceOnDemandSessionIdProvider {
    protected SauceryRemoteWebDriver Driver;

    public SauceryBase(String os, String platformType, String browser, String browserVersion, String longName, String longVersion, String url, String device, String deviceOrientation) {
        super(os, platformType, browser, browserVersion, longName, longVersion, url, device, deviceOrientation);
    }

    @Override
    public void InitialiseDriver(DesiredCapabilities caps, int waitSecs) {
        SauceLabsFlowController.ControlFlow();
        try {
            String fullUrl = String.format(SauceryConstants.SAUCELABS_HUB, Authentication.getUsername(), Authentication.getAccessKey());
            Driver = new SauceryRemoteWebDriver(new URL(fullUrl), caps);
            sessionId = Driver.getSessionId().toString();
            Driver.manage().timeouts().implicitlyWait(waitSecs, TimeUnit.SECONDS);
        } catch (Exception ex) {
            System.out.println(ex.getMessage());
        }
    }

    @After
    public void tearDown() {
        if (Driver != null) {
            // terminate the remote webdriver session
            Driver.quit();
        }
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014 
 */