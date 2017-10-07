package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import au.com.fullcirclesolutions.saucery.utils.SyncedPrinter;

class WebDriverAndroidCapabilities extends BaseCapabilities {
    WebDriverAndroidCapabilities(SaucePlatform platform, String testName) {
        super(testName);
        SyncedPrinter.println(String.format(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_WEBDRIVER));
        Caps = CapsBuilder.buildWebDriverAndroidCaps(platform);
        AddSauceLabsCapabilities();
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014 
 */