package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.Enviro;
import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import org.openqa.selenium.remote.CapabilityType;
import org.openqa.selenium.remote.DesiredCapabilities;

class CapsBuilder {
    static DesiredCapabilities buildAppiumAndroidCaps(SaucePlatform platform){
        String browser = GetBrowser(Enviro.SauceNativeApp(), Enviro.SauceUseChromeOnAndroid());
        DesiredCapabilities caps = DesiredCapabilities.android();
        caps.setCapability(SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, browser);
        caps.setCapability(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, platform.LongVersion);
        //caps.setCapability(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, SauceryConstants.SAUCE_APPIUM_VERSION);
        caps.setCapability(SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.ANDROID);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.LongName);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);
        return caps;
    }
    
    static DesiredCapabilities buildAppiumIOSCaps(SaucePlatform platform){
        DesiredCapabilities caps = platform.IsAnIPhone() ? DesiredCapabilities.iphone() : DesiredCapabilities.ipad();
        //caps.setCapability(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, SauceryConstants.SAUCE_APPIUM_VERSION);
        caps.setCapability(SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, GetBrowser(Enviro.SauceNativeApp()));
        caps.setCapability(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, Sanitiser.SanitiseBrowserVersion(platform.BrowserVersion));
        caps.setCapability(SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.IOS_PLATFORM);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.IsAnIPhone() ? SauceryConstants.IPHONE_SIMULATOR : SauceryConstants.IPAD_SIMULATOR);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_CAPABILITY, platform.IsAnIPhone() ? SauceryConstants.IPHONE_DEVICE : SauceryConstants.IPAD_DEVICE);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);
        return caps;
    }
    
    static DesiredCapabilities buildDesktopCaps(SaucePlatform platform){
        DesiredCapabilities caps = new DesiredCapabilities();
        caps.setCapability(CapabilityType.PLATFORM, platform.Os);
        caps.setCapability(CapabilityType.VERSION, platform.BrowserVersion);
        caps.setCapability(CapabilityType.BROWSER_NAME, platform.Browser);
        return caps;
    }
    
    static DesiredCapabilities buildWebDriverAndroidCaps(SaucePlatform platform){
        DesiredCapabilities caps = DesiredCapabilities.android();
        caps.setCapability(SauceryConstants.SAUCE_PLATFORM_CAPABILITY, SauceryConstants.LINUX);
        caps.setCapability(SauceryConstants.SAUCE_VERSION_CAPABILITY, platform.BrowserVersion);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.LongName);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);
        return caps;
    }
    
    static DesiredCapabilities buildWebDriverIOSCaps(SaucePlatform platform){
        DesiredCapabilities caps = platform.IsAnIPhone() ? DesiredCapabilities.iphone() : DesiredCapabilities.ipad();
        caps.setCapability(CapabilityType.PLATFORM, SauceryConstants.IOS_PLATFORM);
        caps.setCapability(CapabilityType.VERSION, platform.BrowserVersion);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_CAPABILITY, platform.Device);
        caps.setCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);
        return caps;
    }
    
    private static String GetBrowser(String nativeApp, boolean useChromeOnAndroid) {
        return nativeApp != null
                ? ""
                : useChromeOnAndroid
                        ? SauceryConstants.CHROME_BROWSER
                        : SauceryConstants.DEFAULT_ANDROID_BROWSER;
    }
    
    private static String GetBrowser(String nativeApp) {
        return nativeApp != null ? "" : SauceryConstants.SAFARI_BROWSER;
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 */