using System;
using OpenQA.Selenium.Remote;
using Saucery2.Capabilities.Base;
using Saucery2.OnDemand;
using Saucery2.Util;

namespace Saucery2.Capabilities.ConcreteProducts {
    internal class DesktopCapabilities : BaseCapabilities {
        public DesktopCapabilities(SaucePlatform platform, string testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);
            Caps = new DesiredCapabilities();
            Caps.SetCapability(CapabilityType.Platform, platform.Os);
            Caps.SetCapability(CapabilityType.Version, platform.BrowserVersion);
            Caps.SetCapability(CapabilityType.BrowserName, platform.Browser);

            AddSauceLabsCapabilities();
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */