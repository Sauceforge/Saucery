using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using Saucery.Util;
using System;

namespace Saucery.Driver
{
    public class SauceryAndroidDriver : AndroidDriver {
        public SauceryAndroidDriver(DriverOptions options)
            : base(new Uri(SauceryConstants.SAUCELABS_HUB), options) {
        }

        public SauceryAndroidDriver(Uri remoteAddress, DriverOptions options)
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