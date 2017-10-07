package au.com.fullcirclesolutions.saucery.driver;

import io.appium.java_client.android.AndroidDriver;
import java.net.URL;
import org.openqa.selenium.remote.DesiredCapabilities;

public class SauceryAndroidDriver extends AndroidDriver {
    public SauceryAndroidDriver(URL remoteAddress, DesiredCapabilities desiredCapabilities) {
        super(remoteAddress, desiredCapabilities);
    }

    public String GetSessionId() {
        return GetSessionId().toString();
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 */