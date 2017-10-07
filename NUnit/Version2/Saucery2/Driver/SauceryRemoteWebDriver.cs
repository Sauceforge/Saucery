using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Saucery2.Driver {
    public class SauceryRemoteWebDriver : RemoteWebDriver {
        public SauceryRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities) {
        }

        public SauceryRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities, TimeSpan.FromSeconds(400)) {
        }

        public string GetSessionId() {
            return SessionId.ToString();
        } 
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */