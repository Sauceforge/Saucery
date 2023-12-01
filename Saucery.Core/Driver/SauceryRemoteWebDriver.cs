using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Saucery.Core.Driver;

public class SauceryRemoteWebDriver(Uri remoteAddress, DriverOptions options, int secs) : RemoteWebDriver(remoteAddress, options.ToCapabilities(), TimeSpan.FromSeconds(secs)) {
    public string GetSessionId() => SessionId.ToString();
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/