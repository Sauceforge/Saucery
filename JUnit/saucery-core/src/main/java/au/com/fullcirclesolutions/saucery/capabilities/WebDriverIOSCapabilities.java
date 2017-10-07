package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import au.com.fullcirclesolutions.saucery.utils.SyncedPrinter;

class WebDriverIOSCapabilities extends BaseCapabilities {
    WebDriverIOSCapabilities(SaucePlatform platform, String testName) {
        super(testName);
        SyncedPrinter.println(String.format(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_WEBDRIVER));
        Caps = CapsBuilder.buildWebDriverIOSCaps(platform);
        AddSauceLabsCapabilities();
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 */