package au.com.fullcirclesolutions.saucery.driver;

import java.net.URL;

import org.openqa.selenium.remote.CommandExecutor;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.remote.RemoteWebDriver;

public class SauceryRemoteWebDriver extends RemoteWebDriver {

    public SauceryRemoteWebDriver(CommandExecutor commandExecutor, DesiredCapabilities desiredCapabilities) {
        super(commandExecutor, desiredCapabilities);
    }

    public SauceryRemoteWebDriver(URL remoteAddress, DesiredCapabilities desiredCapabilities) {
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
