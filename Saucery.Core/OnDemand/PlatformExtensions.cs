using Saucery.Core.Dojo;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

internal static class PlatformExtensions {
    public static bool IsAMobileDevice(this BrowserVersion browserVersion) => 
        IsAnAndroidDevice(browserVersion) || 
        IsAnAppleDevice(browserVersion);

    public static bool IsARealDevice(this BrowserVersion browserVersion) => 
        !browserVersion.DeviceName.Contains("Simulator") && 
        !browserVersion.DeviceName.Contains("Emulator");

    public static bool IsARealDevice(this SaucePlatform platform) => 
        !platform.LongName.Contains("Simulator") && 
        !platform.LongName.Contains("Emulator");

    public static bool IsAnAndroidDevice(this BrowserVersion browserVersion) => 
        browserVersion.Os != null && 
        browserVersion.Os.Contains(SauceryConstants.PLATFORM_LINUX, StringComparison.CurrentCultureIgnoreCase) && 
        browserVersion.RecommendedAppiumVersion != null;

    public static bool IsAnAppleDevice(this BrowserVersion browserVersion) => 
        IsAnIPhone(browserVersion) || 
        IsAnIPad(browserVersion);

    public static bool IsAnIPhone(this BrowserVersion browserVersion) => 
        browserVersion.DeviceName != null && 
        browserVersion.DeviceName.Contains(SauceryConstants.APPLE_IPHONE, StringComparison.CurrentCultureIgnoreCase);

    public static bool IsAnIPad(this BrowserVersion browserVersion) => 
        browserVersion.DeviceName != null && 
        browserVersion.DeviceName.Contains(SauceryConstants.APPLE_IPAD, StringComparison.CurrentCultureIgnoreCase);

    public static bool IsAnAppleDevice(this SaucePlatform platform) => 
        IsAnIPhone(platform) || 
        IsAnIPad(platform);

    public static bool IsAnIPhone(this SaucePlatform platform) => 
        platform.LongName != null && 
        platform.LongName.Contains(SauceryConstants.APPLE_IPHONE, StringComparison.CurrentCultureIgnoreCase);

    public static bool IsAnIPad(this SaucePlatform platform) => 
        platform.LongName != null && 
        platform.LongName.Contains(SauceryConstants.APPLE_IPAD, StringComparison.CurrentCultureIgnoreCase);

    public static bool IsAnAndroidDevice(this SaucePlatform platform) => 
        platform.LongName != null &&
       (platform.LongName.Contains(SauceryConstants.GOOGLE_LOWER, StringComparison.CurrentCultureIgnoreCase) ||
        platform.LongName.Contains(SauceryConstants.SAMSUNG_LOWER, StringComparison.CurrentCultureIgnoreCase) ||
        platform.LongName.Contains(SauceryConstants.ANDROID_LOWER, StringComparison.CurrentCultureIgnoreCase));
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/