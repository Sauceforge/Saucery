using Saucery.Core.Dojo;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

internal static class PlatformExtensions {
    public static bool IsAMobileDevice(this BrowserVersion browserVersion) => IsAnAndroidDevice(browserVersion) || IsAnAppleDevice(browserVersion);

    public static bool IsAnAndroidDevice(this BrowserVersion browserVersion) => browserVersion.Os != null && browserVersion.Os.ToUpper().Contains(SauceryConstants.PLATFORM_LINUX.ToUpper()) && browserVersion.RecommendedAppiumVersion != null;

    public static bool IsAnAppleDevice(this BrowserVersion browserVersion) => IsAnIPhone(browserVersion) || IsAnIPad(browserVersion);

    public static bool IsAnIPhone(this BrowserVersion browserVersion) => browserVersion.DeviceName != null && browserVersion.DeviceName.ToLower().Contains(SauceryConstants.APPLE_IPHONE);

    public static bool IsAnIPad(this BrowserVersion browserVersion) => browserVersion.DeviceName != null && browserVersion.DeviceName.ToLower().Contains(SauceryConstants.APPLE_IPAD);

    public static bool IsAnAppleDevice(this SaucePlatform platform) => IsAnIPhone(platform) || IsAnIPad(platform);

    public static bool IsAnIPhone(this SaucePlatform platform) => platform.LongName != null && platform.LongName.ToLower().Contains(SauceryConstants.APPLE_IPHONE);

    public static bool IsAnIPad(this SaucePlatform platform) => platform.LongName != null && platform.LongName.ToLower().Contains(SauceryConstants.APPLE_IPAD);

    public static bool IsAnAndroidDevice(this SaucePlatform platform) => platform.LongName != null &&
               (platform.LongName.ToLower().Contains(SauceryConstants.GOOGLE_LOWER) ||
                platform.LongName.ToLower().Contains(SauceryConstants.SAMSUNG_LOWER) ||
                platform.LongName.ToLower().Contains(SauceryConstants.ANDROID_LOWER));
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/