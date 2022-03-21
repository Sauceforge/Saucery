using Saucery.Dojo;
using Saucery.Util;
using System;
using System.Text;

namespace Saucery.OnDemand
{
    internal static class PlatformExtensions {
        //public static bool CanUseAppium(this SaucePlatform platform) {
        //    if (platform.IsAnAndroidDevice()) { Console.WriteLine("CanUseAppium: Platform {0}; BrowserVersion: {1}", platform.Platform, platform.ParseBrowserVersion()); }
        //    return (IsAnAppleDevice(platform) &&
        //            platform.ParseBrowserVersion() >= SauceryConstants.APPIUM_IOS_MINIMUM_VERSION) ||
        //           (IsAnAndroidDevice(platform) &&
        //            platform.ParseBrowserVersion() >= SauceryConstants.APPIUM_ANDROID_MINIMUM_VERSION);
        //}

        public static bool IsAMobileDevice(this BrowserVersion browserVersion)
        {
            return IsAnAndroidDevice(browserVersion) || IsAnAppleDevice(browserVersion);
        }

        public static bool IsAnAndroidDevice(this BrowserVersion browserVersion)
        {
            return browserVersion.Os != null && browserVersion.Os.ToUpper().Contains(SauceryConstants.LINUX.ToUpper());
        }

        public static bool IsAnAppleDevice(this BrowserVersion browserVersion)
        {
            return IsAnIPhone(browserVersion) || IsAnIPad(browserVersion);
        }

        public static bool IsAnIPhone(this BrowserVersion browserVersion)
        {
            return browserVersion.DeviceName != null && browserVersion.DeviceName.ToLower().Contains(SauceryConstants.APPLE_IPHONE);
        }

        public static bool IsAnIPad(this BrowserVersion browserVersion)
        {
            return browserVersion.DeviceName != null && browserVersion.DeviceName.ToLower().Contains(SauceryConstants.APPLE_IPAD);
        }

        //public static bool IsAMobileDevice(this SaucePlatform platform)
        //{
        //    return IsAnAndroidDevice(platform) || IsAnAppleDevice(platform);
        //}

        public static bool IsAnAppleDevice(this SaucePlatform platform)
        {
            return IsAnIPhone(platform) || IsAnIPad(platform);
        }

        public static bool IsAnIPhone(this SaucePlatform platform) {
            return platform.Device != null && platform.Device.ToLower().Contains(SauceryConstants.APPLE_IPHONE);
        }

        public static bool IsAnIPad(this SaucePlatform platform) {
            return platform.Device != null && platform.Device.ToLower().Contains(SauceryConstants.APPLE_IPAD);
        }

        public static bool IsAnAndroidDevice(this SaucePlatform platform) {
            return platform.Device != null && platform.Device.ToUpper().Contains(SauceryConstants.ANDROID_PLATFORM);
        }

        //public static void SetTestName(this SaucePlatform platform, string testName) {
        //    var shortTestName = new StringBuilder();
        //    shortTestName.Append(testName.Contains(SauceryConstants.LEFT_SQUARE_BRACKET) 
        //                            ? testName.Substring(0, testName.IndexOf(SauceryConstants.LEFT_SQUARE_BRACKET, StringComparison.Ordinal)) 
        //                            : testName);
        //    platform.TestName = platform.IsAMobileDevice()
        //        ? AppendPlatformField(AppendPlatformField(AppendPlatformField(shortTestName, platform.LongName), platform.BrowserVersion), platform.DeviceOrientation).ToString()
        //        : AppendPlatformField(AppendPlatformField(AppendPlatformField(shortTestName, platform.Os),       platform.Browser),        platform.BrowserVersion).ToString();
        //}

        public static void SetTestName(this BrowserVersion browserVersion, string testName)
        {
            var shortTestName = new StringBuilder();
            shortTestName.Append(testName.Contains(SauceryConstants.LEFT_SQUARE_BRACKET)
                                    ? testName.Substring(0, testName.IndexOf(SauceryConstants.LEFT_SQUARE_BRACKET, StringComparison.Ordinal))
                                    : testName);
            browserVersion.TestName = browserVersion.IsAMobileDevice()
                ? AppendPlatformField(AppendPlatformField(AppendPlatformField(shortTestName, browserVersion.DeviceName), browserVersion.Name), browserVersion.DeviceOrientation).ToString()
                : AppendPlatformField(AppendPlatformField(AppendPlatformField(shortTestName, browserVersion.Os), browserVersion.BrowserName), browserVersion.Name).ToString();
        }

        //public static string SanitisedLongVersion(this SaucePlatform platform)
        //{
        //    var result = platform.LongVersion.EndsWith(SauceryConstants.DOT)
        //                    ? platform.LongVersion.Trim().Remove(platform.LongVersion.Length - 1)
        //                    : platform.LongVersion.Trim();
        //    Console.WriteLine("SanitisedLongVersion returning string '{0}'", result);
        //    return result;
        //}

        private static StringBuilder AppendPlatformField(this StringBuilder testName, string fieldToAdd) {
            return testName.Append(SauceryConstants.UNDERSCORE).Append(fieldToAdd);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */