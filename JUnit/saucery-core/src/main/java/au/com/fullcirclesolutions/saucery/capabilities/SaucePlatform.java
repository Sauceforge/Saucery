package au.com.fullcirclesolutions.saucery.capabilities;

import au.com.fullcirclesolutions.saucery.utils.SauceryConstants;
import com.fasterxml.jackson.annotation.JsonProperty;

public class SaucePlatform {
    @JsonProperty("os")
    String Os;
    @JsonProperty("platform")
    String Platform;
    @JsonProperty("browser")
    String Browser;
    @JsonProperty("browser-version")
    String BrowserVersion;
    @JsonProperty("long-name")
    String LongName;
    @JsonProperty("long-version")
    String LongVersion;
    @JsonProperty("url")
    String Url;
    @JsonProperty("device")
    String Device;
    @JsonProperty("device-orientation")
    String DeviceOrientation;
    
    private SaucePlatform(){
    }

    public SaucePlatform(String os, String platform, String browser, String browserVersion, String longName, String longVersion, String url, String device, String deviceOrientation) {
        Os = os;
        Platform = platform;
        Browser = browser;
        BrowserVersion = browserVersion;
        LongName = longName;
        LongVersion = longVersion;
        Url = url;
        Device = device;
        DeviceOrientation = deviceOrientation;
    }

    public String[] toArray() {
        return new String[]{Sanitiser.SanitisePlatformField(Os),
            Sanitiser.SanitisePlatformField(Platform),
            Sanitiser.SanitisePlatformField(Browser),
            Sanitiser.SanitisePlatformField(BrowserVersion),
            Sanitiser.SanitisePlatformField(LongName),
            Sanitiser.SanitisePlatformField(LongVersion),
            Sanitiser.SanitisePlatformField(Url),
            Device != null ? Device : SauceryConstants.NULL_STRING,
            DeviceOrientation != null ? DeviceOrientation : SauceryConstants.NULL_STRING
        };
    }

    double parseBrowserVersion() {
        return Double.parseDouble(Sanitiser.SanitiseBrowserVersion(BrowserVersion));
    }

    boolean CanUseAppium() {
        double browserVersion = parseBrowserVersion();
        return (IsAnAppleDevice() && browserVersion >= SauceryConstants.APPIUM_IOS_MINIMUM_VERSION)
                || (IsAnAndroidDevice() && browserVersion >= SauceryConstants.APPIUM_ANDROID_MINIMUM_VERSION);
    }

    boolean IsAMobileDevice() {
        return IsAnAndroidDevice() || IsAnAppleDevice();
    }

    boolean IsADesktopPlatform() {
        return !IsAMobileDevice();
    }

    boolean IsAnIOS7Device() {
        return IsAnAppleDevice() && BrowserVersion.contains("7");
    }

    boolean IsAnAppleDevice() {
        return IsAnIPhone() || IsAnIPad();
    }

    boolean IsAnIPhone() {
        return Device.toLowerCase().contains(SauceryConstants.APPLE_IPHONE);
    }

    boolean IsAnIPad() {
        return Device.toLowerCase().contains(SauceryConstants.APPLE_IPAD);
    }

    boolean IsAnAndroidDevice() {
        return Platform.equals(SauceryConstants.ANDROID_PLATFORM);
    }
}
/*
 * Copyright: Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014 
 */