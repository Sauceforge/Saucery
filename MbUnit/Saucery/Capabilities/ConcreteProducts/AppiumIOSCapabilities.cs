using System;
using OpenQA.Selenium.Remote;
using Saucery.Capabilities.Base;
using Saucery.Extensions;
using Saucery.TestDataSources;
using Saucery.Util;

namespace Saucery.Capabilities.ConcreteProducts {
    internal class AppiumIOSCapabilities : BaseCapabilities {
        public AppiumIOSCapabilities(PlatformTestData platform, String testName, string nativeApp) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_APPIUM);
            Caps = platform.IsAnIPhone() ? DesiredCapabilities.IPhone() : DesiredCapabilities.IPad();
            Caps.SetCapability(SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, nativeApp == null ? SauceryConstants.SAFARI_BROWSER : "");
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, platform.BrowserVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, SauceryConstants.SAUCE_APPIUM_VERSION);
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.IOS_PLATFORM);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.IsAnIPhone() ? SauceryConstants.IPHONE_SIMULATOR : SauceryConstants.IPAD_SIMULATOR);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);

            //Native Mobile Application
            if (nativeApp != null) {
                Caps.SetCapability(SauceryConstants.SAUCE_NATIVE_APP_CAPABILITY, nativeApp);
            }
            AddSauceLabsCapabilities();
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 * 
 */