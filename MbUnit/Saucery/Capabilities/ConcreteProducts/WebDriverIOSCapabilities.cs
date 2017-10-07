using System;
using OpenQA.Selenium.Remote;
using Saucery.Capabilities.Base;
using Saucery.Extensions;
using Saucery.TestDataSources;
using Saucery.Util;

namespace Saucery.Capabilities.ConcreteProducts {
    internal class WebDriverIOSCapabilities : BaseCapabilities {
        public WebDriverIOSCapabilities(PlatformTestData platform, String testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_WEBDRIVER);
            Caps = platform.IsAnIPhone() ? DesiredCapabilities.IPhone() : DesiredCapabilities.IPad();
            //Caps.SetCapability(CapabilityType.Platform, platform.GetSanitisedOs());
            Caps.SetCapability(CapabilityType.Platform, platform.Os);
            Caps.SetCapability(CapabilityType.Version, platform.BrowserVersion);
            //SauceLabs lies yet again: Adding DeviceName here means IOS 6.0 tests fail to start
            //caps.SetCapability(Constants.SAUCE_DEVICE_NAME_CAPABILITY, platform.IsAnIPhone() ? Constants.IPHONE_DEVICE_NAME : Constants.IPAD_DEVICE_NAME);
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