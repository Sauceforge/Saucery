using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Saucery2.Driver {
    internal class TouchCapableRemoteWebDriver : RemoteWebDriver, IHasTouchScreen {
        public TouchCapableRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities) {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public TouchCapableRemoteWebDriver(ICapabilities desiredCapabilities)
            : base(desiredCapabilities) {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public TouchCapableRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities) {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public TouchCapableRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout)
            : base(remoteAddress, desiredCapabilities, commandTimeout) {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public ITouchScreen TouchScreen { get; private set; }

        public bool HasCapability(string capability) {
            throw new NotImplementedException();
        }

        public object GetCapability(string capability) {
            throw new NotImplementedException();
        }

        public string BrowserName { get; private set; }
        public Platform Platform { get; private set; }
        public string Version { get; private set; }
        public bool IsJavaScriptEnabled { get; private set; }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 1st August 2014
 * 
 */