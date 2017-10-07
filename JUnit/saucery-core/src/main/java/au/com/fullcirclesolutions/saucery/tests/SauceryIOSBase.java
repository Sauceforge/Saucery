package au.com.fullcirclesolutions.saucery.tests;

import au.com.fullcirclesolutions.saucery.driver.SauceryIOSDriver;
import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import com.saucelabs.common.SauceOnDemandSessionIdProvider;
import com.saucelabs.junit.ConcurrentParameterized;
import java.net.URL;
import java.util.concurrent.TimeUnit;
import org.junit.After;
import org.junit.runner.RunWith;
import org.openqa.selenium.remote.DesiredCapabilities;

@RunWith(ConcurrentParameterized.class)
public class SauceryIOSBase extends SauceryRoot implements SauceOnDemandSessionIdProvider {
    protected SauceryIOSDriver Driver;

    public SauceryIOSBase(String os, String platformType, String browser, String browserVersion, String longName, String longVersion, String url, String device, String deviceOrientation) {
        super(os, platformType, browser, browserVersion, longName, longVersion, url, device, deviceOrientation);
    }

    @Override
    public void InitialiseDriver(DesiredCapabilities caps, int waitSecs) {
        SauceLabsFlowController.ControlFlow();
        try {
            String fullUrl = String.format(SauceryConstants.SAUCELABS_HUB, Authentication.getUsername(), Authentication.getAccessKey());
            Driver = new SauceryIOSDriver(new URL(fullUrl), caps);
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