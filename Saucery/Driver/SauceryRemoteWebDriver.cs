using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;

namespace Saucery.Driver;

public class SauceryRemoteWebDriver : RemoteWebDriver {
    public SauceryRemoteWebDriver(Uri remoteAddress, DriverOptions options, int secs)
        : base(remoteAddress, options.ToCapabilities(), TimeSpan.FromSeconds(secs)) {
    }

    public string GetSessionId() {
        return SessionId.ToString();
    } 
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/