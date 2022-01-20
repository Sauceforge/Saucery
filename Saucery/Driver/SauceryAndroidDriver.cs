using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;
using Saucery.Util;

namespace Saucery.Driver {
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

        //protected override RemoteWebElementFactory CreateElementFactory()
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Swipe(int startx, int starty, int endx, int endy, int duration) {
        //    throw new NotImplementedException();
        //}

        //public override AppiumWebElement ScrollTo(string text) {
        //    throw new NotImplementedException();
        //}

        //public override AppiumWebElement ScrollToExact(string text) {
        //    throw new NotImplementedException();
        //}
    }
}

/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */