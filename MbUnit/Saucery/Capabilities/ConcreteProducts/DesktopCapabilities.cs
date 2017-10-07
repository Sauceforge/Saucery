using System;
using OpenQA.Selenium.Remote;
using Saucery.Capabilities.Base;
using Saucery.TestDataSources;
using Saucery.Util;

namespace Saucery.Capabilities.ConcreteProducts {
    internal class DesktopCapabilities : BaseCapabilities {
        public DesktopCapabilities(PlatformTestData platform, String testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);
            Caps = new DesiredCapabilities();
            Caps.SetCapability(CapabilityType.Platform, platform.Os);
            Caps.SetCapability(CapabilityType.Version, platform.BrowserVersion);
            Caps.SetCapability(CapabilityType.BrowserName, platform.BrowserName);

            AddSauceLabsCapabilities();
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */