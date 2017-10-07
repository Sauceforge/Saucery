using System;
using OpenQA.Selenium.Remote;
using SauceryX.Capabilities.Base;
using SauceryX.Extensions;
using SauceryX.OnDemand;
using SauceryX.Util;

namespace SauceryX.Capabilities.ConcreteProducts {
    internal class WebDriverIOSCapabilities : BaseCapabilities {
        public WebDriverIOSCapabilities(SaucePlatform platform, string testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_WEBDRIVER);
            Caps = platform.IsAnIPhone() ? DesiredCapabilities.IPhone() : DesiredCapabilities.IPad();
            Caps.SetCapability(CapabilityType.Platform, "OS X 10.10");
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