﻿using System;
using OpenQA.Selenium.Remote;
using Saucery3.Capabilities.Base;
using Saucery3.OnDemand;
using Saucery3.Util;

namespace Saucery3.Capabilities.ConcreteProducts {
    internal class AppiumIOSCapabilities : BaseCapabilities {
        public AppiumIOSCapabilities(SaucePlatform platform, string testName) : base(testName) {
            var nativeApp = Enviro.SauceNativeApp;
            //Console.WriteLine(SauceryConstants.SETTING_UP_APPIUM, testName, SauceryConstants.IOS_ON_APPIUM, Enviro.RecommendedAppiumVersion);
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_APPIUM);
            Caps = platform.IsAnIPhone() ? DesiredCapabilities.IPhone() : DesiredCapabilities.IPad();

            //See https://github.com/appium/appium-dotnet-driver/wiki/Android-Sample
            //IOSDriver<AppiumWebElement> iosd = new IOSDriver<AppiumWebElement>(Caps);

            //Caps.SetCapability(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, Enviro.RecommendedAppiumVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, GetBrowser(nativeApp));
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, platform.BrowserVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.IOS_PLATFORM);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.IsAnIPhone() ? SauceryConstants.IPHONE_SIMULATOR : SauceryConstants.IPAD_SIMULATOR);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_CAPABILITY, platform.IsAnIPhone() ? SauceryConstants.IPHONE_DEVICE : SauceryConstants.IPAD_DEVICE);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);

            AddSauceLabsCapabilities(nativeApp);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 10th September 2014
 * 
 */