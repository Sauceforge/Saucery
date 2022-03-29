using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace Saucery.Driver
{
    public class SauceryRemoteWebDriver : RemoteWebDriver {
        public SauceryRemoteWebDriver(Uri remoteAddress, DriverOptions options)
            : base(remoteAddress, options.ToCapabilities(), TimeSpan.FromSeconds(180)) {
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