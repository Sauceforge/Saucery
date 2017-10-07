using System;
using OpenQA.Selenium.Remote;
using Saucery2.Capabilities.Base;
using Saucery2.OnDemand;
using Saucery2.Util;

namespace Saucery2.Capabilities.ConcreteProducts {
    internal class WebDriverIOSCapabilities : BaseCapabilities {
        public WebDriverIOSCapabilities(SaucePlatform platform, string testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_WEBDRIVER);
            Caps = platform.IsAnIPhone() ? DesiredCapabilities.IPhone() : DesiredCapabilities.IPad();
            Caps.SetCapability(CapabilityType.Platform, SauceryConstants.IOS_PLATFORM);
            Caps.SetCapability(CapabilityType.Version, platform.BrowserVersion);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_CAPABILITY, platform.Device);
            Caps.SetCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);

            AddSauceLabsCapabilities();
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 10th September 2014
 * 
 */