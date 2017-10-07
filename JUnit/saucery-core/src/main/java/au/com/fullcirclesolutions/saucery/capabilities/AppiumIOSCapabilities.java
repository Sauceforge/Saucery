package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.Enviro;
import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import au.com.fullcirclesolutions.saucery.utils.SyncedPrinter;

class AppiumIOSCapabilities extends BaseCapabilities {
    AppiumIOSCapabilities(SaucePlatform platform, String testName) {
        super(testName);
        SyncedPrinter.println(String.format(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_APPIUM));
        Caps = CapsBuilder.buildAppiumIOSCaps(platform);
        AddSauceLabsCapabilities(Enviro.SauceNativeApp());
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 */