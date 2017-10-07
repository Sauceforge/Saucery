package au.com.fullcirclesolutions.saucery.driver;

import io.appium.java_client.ios.IOSDriver;
import java.net.URL;
import org.openqa.selenium.remote.DesiredCapabilities;

public class SauceryIOSDriver extends IOSDriver {
    public SauceryIOSDriver(URL remoteAddress, DesiredCapabilities desiredCapabilities) {
        super(remoteAddress, desiredCapabilities);
    }

    public String GetSessionId() {
        return getSessionId().toString();
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 */