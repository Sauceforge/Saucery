using System;
using OpenQA.Selenium.Remote;
using SauceryX.Capabilities.Base;
using SauceryX.OnDemand;
using SauceryX.Util;

namespace SauceryX.Capabilities.ConcreteProducts {
    internal class AppiumAndroidCapabilities : BaseCapabilities {
        public AppiumAndroidCapabilities(SaucePlatform platform, string testName)
            : base(testName)
        {
            var nativeApp = Enviro.SauceNativeApp;
            var useChromeOnAndroid = Enviro.SauceUseChromeOnAndroid;
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_APPIUM);
            Caps = DesiredCapabilities.Android();
            Caps.SetCapability(SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, GetBrowser(nativeApp, useChromeOnAndroid));
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, platform.LongVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, SauceryConstants.SAUCE_APPIUM_VERSION);
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.ANDROID);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.LongName);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);

            AddSauceLabsCapabilities(nativeApp);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 * 
 */