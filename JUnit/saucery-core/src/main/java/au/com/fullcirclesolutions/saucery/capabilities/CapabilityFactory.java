package au.com.fullcirclesolutions.saucery.capabilities;

import org.openqa.selenium.remote.DesiredCapabilities;

public class CapabilityFactory {
    public static DesiredCapabilities CreateCapabilities(SaucePlatform platform, String testName) {
        String sanitisedTestName = Sanitiser.SanitiseTestName(testName, platform);
        if (platform.IsADesktopPlatform()) {
            return (new DesktopCreator()).Create(platform, sanitisedTestName).GetCaps();
        }
        //Mobile Platform
        return platform.CanUseAppium()
                //Mobile Platform
                ? platform.IsAnAppleDevice()
                        ? (new AppiumIOSCreator()).Create(platform, sanitisedTestName).GetCaps()
                        : (new AppiumAndroidCreator()).Create(platform, sanitisedTestName).GetCaps()
                //Devolve to WebDriver    
                : platform.IsAnAppleDevice()
                        ? (new WebDriverIOSCreator()).Create(platform, sanitisedTestName).GetCaps()
                        : (new WebDriverAndroidCreator()).Create(platform, sanitisedTestName).GetCaps();
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 10th August 2014
 */