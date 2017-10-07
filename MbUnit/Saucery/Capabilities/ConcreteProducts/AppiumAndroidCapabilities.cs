using System;
using OpenQA.Selenium.Remote;
using Saucery.Capabilities.Base;
using Saucery.Convertors;
using Saucery.TestDataSources;
using Saucery.Util;

namespace Saucery.Capabilities.ConcreteProducts {
    internal class AppiumAndroidCapabilities : BaseCapabilities {
        public AppiumAndroidCapabilities(PlatformTestData platform, String testName, bool? useChromeOnAndroid, string nativeApp) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_APPIUM);
            var browser = GetBrowser(nativeApp, useChromeOnAndroid.GetValue<bool>());
            Caps = DesiredCapabilities.Android();
            Caps.SetCapability(SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, browser);
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, platform.LongVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, SauceryConstants.SAUCE_APPIUM_VERSION);
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.ANDROID);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.LongName);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);

            //Native Mobile Application
            if(nativeApp != null) {
                Caps.SetCapability(SauceryConstants.SAUCE_NATIVE_APP_CAPABILITY, nativeApp);
            }
            AddSauceLabsCapabilities();
        }

        private static String GetBrowser(String nativeApp, bool useChromeOnAndroid) {
            return nativeApp != null
                    ? ""
                    : useChromeOnAndroid
                       ? SauceryConstants.CHROME_BROWSER
                       : SauceryConstants.DEFAULT_ANDROID_BROWSER;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 * 
 */