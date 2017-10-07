using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Saucery.Driver {
    public class SauceryRemoteWebDriver : RemoteWebDriver {
        public SauceryRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities) {
        }

        public SauceryRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities, TimeSpan.FromSeconds(400)) {
            //var threadLocalDriver = new ThreadLocal<RemoteWebDriver>();
            //threadLocalDriver.Value(ThreadGuard.protect(new RemoteWebDriver(remoteAddress, desiredCapabilities)));
            //return threadLocalDriver.Value;
        }

        public String GetSessionId() {
            return SessionId != null ? SessionId.ToString() : "BLANK";
            //return SessionId.ToString();
        } 
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */