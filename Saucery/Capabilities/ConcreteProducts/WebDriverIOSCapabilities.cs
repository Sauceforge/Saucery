using System;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Remote;
using Saucery3.Capabilities.Base;
using Saucery3.OnDemand;
using Saucery3.Util;

namespace Saucery3.Capabilities.ConcreteProducts {
    internal class WebDriverIOSCapabilities : BaseCapabilities {
        public WebDriverIOSCapabilities(SaucePlatform platform, string testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_WEBDRIVER);
            Caps = platform.IsAnIPhone() ? DesiredCapabilities.IPhone() : DesiredCapabilities.IPad();

            //See https://github.com/appium/appium-dotnet-driver/wiki/Android-Sample
            //IOSDriver<AppiumWebElement> iosd = new IOSDriver<AppiumWebElement>(Caps);

            Caps.SetCapability(CapabilityType.Platform, SauceryConstants.IOS_PLATFORM);
            Caps.SetCapability(CapabilityType.Version, platform.BrowserVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_CAPABILITY, platform.Device);
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