using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;
using Saucery.Util;

namespace Saucery.Driver {
    public class SauceryIOSDriver : IOSDriver {
        public SauceryIOSDriver(DriverOptions options)
            : base(new Uri(SauceryConstants.SAUCELABS_HUB), options, TimeSpan.FromSeconds(400)) {
        }

        public SauceryIOSDriver(Uri remoteAddress, DriverOptions options)
            : base(remoteAddress, options, TimeSpan.FromSeconds(400)) {
        }

        public string GetSessionId() {
            return SessionId.ToString();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */