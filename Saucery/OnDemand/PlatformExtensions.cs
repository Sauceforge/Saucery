using Saucery.Dojo;
using Saucery.OnDemand.Base;
using Saucery.Util;
using System;
using System.Text;

namespace Saucery.OnDemand;

internal static class PlatformExtensions {
    public static bool IsAMobileDevice(this BrowserVersion browserVersion) => IsAnAndroidDevice(browserVersion) || IsAnAppleDevice(browserVersion);

    public static bool IsAnAndroidDevice(this BrowserVersion browserVersion) => browserVersion.Os != null && browserVersion.Os.ToUpper().Contains(SauceryConstants.PLATFORM_LINUX.ToUpper());

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

    public static void SetTestName(this BrowserVersion browserVersion, string testName)
    {
        StringBuilder shortTestName = new();
        shortTestName.Append(testName.Contains(SauceryConstants.LEFT_SQUARE_BRACKET)
                                ? testName[..testName.IndexOf(SauceryConstants.LEFT_SQUARE_BRACKET, StringComparison.Ordinal)]
                                : testName);

        browserVersion.TestName = browserVersion.IsAMobileDevice()
            ? string.IsNullOrEmpty(browserVersion.DeviceOrientation)
                ? AppendPlatformField(AppendPlatformField(shortTestName, browserVersion.DeviceName), browserVersion.Name).ToString()
                : AppendPlatformField(AppendPlatformField(AppendPlatformField(shortTestName, browserVersion.DeviceName), browserVersion.Name), browserVersion.DeviceOrientation).ToString()
            : string.IsNullOrEmpty(browserVersion.ScreenResolution)
                ? AppendPlatformField(AppendPlatformField(AppendPlatformField(shortTestName, browserVersion.Os), browserVersion.BrowserName), browserVersion.Name).ToString()
                : AppendPlatformField(AppendPlatformField(AppendPlatformField(AppendPlatformField(shortTestName, browserVersion.Os), browserVersion.BrowserName), browserVersion.Name), browserVersion.ScreenResolution).ToString();
    }

    private static StringBuilder AppendPlatformField(this StringBuilder testName, string fieldToAdd) => testName.Append(SauceryConstants.UNDERSCORE).Append(fieldToAdd);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/