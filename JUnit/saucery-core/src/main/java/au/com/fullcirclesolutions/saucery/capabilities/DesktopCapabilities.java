package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import au.com.fullcirclesolutions.saucery.utils.SyncedPrinter;

class DesktopCapabilities extends BaseCapabilities {
    DesktopCapabilities(SaucePlatform platform, String testName) {
        super(testName);
        SyncedPrinter.println(String.format(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER));
        Caps = CapsBuilder.buildDesktopCaps(platform);
        AddSauceLabsCapabilities();
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 */