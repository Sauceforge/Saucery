﻿using System;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;
using Saucery3.Capabilities.Base;
using Saucery3.OnDemand;
using Saucery3.Util;

namespace Saucery3.Capabilities.ConcreteProducts {
    internal class WebDriverAndroidCapabilities : BaseCapabilities {
        public WebDriverAndroidCapabilities(SaucePlatform platform, string testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_WEBDRIVER);
            Caps = DesiredCapabilities.Android();

            //See https://github.com/appium/appium-dotnet-driver/wiki/Android-Sample
            //AndroidDriver<AppiumWebElement> iosd = new AndroidDriver<AppiumWebElement>(Caps);

            Caps.SetCapability(SauceryConstants.SAUCE_PLATFORM_CAPABILITY, SauceryConstants.LINUX);
            Caps.SetCapability(SauceryConstants.SAUCE_VERSION_CAPABILITY, platform.BrowserVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.LongName);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);

            AddSauceLabsCapabilities();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 10th September 2014
 * 
 */