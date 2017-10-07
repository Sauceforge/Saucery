using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using Saucery2.Util;

namespace Saucery2.Driver {
    public class SauceryAndroidDriver : AppiumDriver<AppiumWebElement> {
        public SauceryAndroidDriver(ICapabilities desiredCapabilities)
            : base(new Uri(SauceryConstants.SAUCELABS_HUB), desiredCapabilities) {
        }

        public SauceryAndroidDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities, TimeSpan.FromSeconds(400)) {
        }

        public string GetSessionId() {
            return SessionId.ToString();
        }

        public override void Swipe(int startx, int starty, int endx, int endy, int duration) {
            throw new NotImplementedException();
        }

        //public override AppiumWebElement ScrollTo(string text) {
        //    throw new NotImplementedException();
        //}

        //public override AppiumWebElement ScrollToExact(string text) {
        //    throw new NotImplementedException();
        //}
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */