using System;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Remote;
using Saucery2.Util;

namespace Saucery2.Driver {
    public class SauceryIOSDriver : IOSDriver<IOSElement> {
        public SauceryIOSDriver(DesiredCapabilities desiredCapabilities)
            : base(new Uri(SauceryConstants.SAUCELABS_HUB), desiredCapabilities, TimeSpan.FromSeconds(400)) {
        }

        public SauceryIOSDriver(Uri remoteAddress, DesiredCapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities, TimeSpan.FromSeconds(400)) {
        }

        public string GetSessionId() {
            return SessionId.ToString();
        }

        //public override IOSElement ScrollTo(string text) {
        //    throw new NotImplementedException();
        //}

        //public override IOSElement ScrollToExact(string text) {
        //    throw new NotImplementedException();
        //}
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */