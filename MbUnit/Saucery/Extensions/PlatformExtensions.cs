using System;
using System.Text;
using Saucery.TestDataSources;
using Saucery.Util;

namespace Saucery.Extensions {
    internal static class PlatformExtensions {
        public static Boolean CanUseAppium(this PlatformTestData platform) {
            if (UserChecker.ItIsMe()) {
                Console.WriteLine("IsAnAppleDevice=" + IsAnAppleDevice(platform));
            }

            return (IsAnAppleDevice(platform) && platform.ParseBrowserVersion() >= SauceryConstants.APPIUM_IOS_MINIMUM_VERSION) ||
                   (IsAnAndroidDevice(platform) && platform.ParseBrowserVersion() >= SauceryConstants.APPIUM_ANDROID_MINIMUM_VERSION);
        }

        public static Boolean IsAMobileDevice(this PlatformTestData platform) {
            return !platform.Device.Equals(SauceryConstants.NULL_STRING) && 
                   (IsAnAndroidDevice(platform) || IsAnAppleDevice(platform));
        }

        public static Boolean IsADesktopPlatform(this PlatformTestData platform) {
            return !IsAMobileDevice(platform);
        }

        public static Boolean IsAnIOS7Device(this PlatformTestData platform) {
            return IsAnAppleDevice(platform) && platform.BrowserVersion.Contains("7");
        }

        public static Boolean IsAnAppleDevice(this PlatformTestData platform) {
            return IsAnIPhone(platform) || IsAnIPad(platform);
        }

        public static Boolean IsAnIPhone(this PlatformTestData platform) {
            return platform.Device.Contains(SauceryConstants.APPLE_IPHONE) ||
                   platform.Device.Contains(SauceryConstants.IPHONE_DEVICE_NAME);
        }

        public static Boolean IsAnIPad(this PlatformTestData platform) {
            return platform.Device.Contains(SauceryConstants.APPLE_IPAD) ||
                   platform.Device.Contains(SauceryConstants.IPAD_DEVICE_NAME);
        }

        public static Boolean IsAnAndroidDevice(this PlatformTestData platform) {
            return platform.LongName.Contains(SauceryConstants.ANDROID_PLATFORM) ||
                   platform.LongName.Contains(SauceryConstants.SAMSUNG) ||
                   platform.LongName.Contains(SauceryConstants.GOOGLE) ||
                   platform.LongName.Contains(SauceryConstants.LG);
        }

        public static string GetTestName(this PlatformTestData platform, string testName) {
            var shortTestName = new StringBuilder();
            var startIndex = testName.IndexOf(SauceryConstants.LEFT_BRACKET, StringComparison.Ordinal);
            shortTestName.Append(startIndex < 0 ? testName : testName.Remove(startIndex));
            //shortTestName.Append(testName.Remove(testName.IndexOf(SauceryConstants.LEFT_BRACKET, StringComparison.Ordinal)));
            var sanitised = shortTestName.Append(SauceryConstants.UNDERSCORE)
                                         .Append(platform.Os)
                                         .Append(SauceryConstants.UNDERSCORE)
                                         .Append(platform.BrowserName)
                                         .Append(SauceryConstants.UNDERSCORE)
                                         .Append(platform.BrowserVersion);
            return IsADesktopPlatform(platform)
                    ? sanitised.ToString()
                    : IsAnAppleDevice(platform)
                        ? sanitised.Append(SauceryConstants.UNDERSCORE)
                                   .Append(platform.DeviceOrientation).ToString()
                        : sanitised.Append(SauceryConstants.UNDERSCORE)
                                   .Append(platform.LongName)
                                   .Append(SauceryConstants.UNDERSCORE)
                                   .Append(platform.DeviceOrientation).ToString(); //Android
        }

        private static string AppendWith(this string testName, string fieldToAdd) {
            return fieldToAdd == SauceryConstants.NULL_STRING
                         ? testName
                         : string.Format("{0}_{1}", testName, fieldToAdd.Replace(" ", ""));
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */