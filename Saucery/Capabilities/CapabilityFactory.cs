using OpenQA.Selenium.Remote;
using Saucery3.Capabilities.ConcreteCreators;
using Saucery3.OnDemand;

namespace Saucery3.Capabilities {
    internal class CapabilityFactory {
        public static DesiredCapabilities CreateCapabilities(SaucePlatform platform, string testName) {
            if (platform.IsADesktopPlatform()) {
                return new DesktopCreator().Create(platform, testName).GetCaps();
            }
            //Mobile Platform
            return platform.CanUseAppium()
                //Mobile Platform
                ? platform.IsAnAppleDevice()
                    ? new AppiumIOSCreator().Create(platform, testName).GetCaps()
                    : new AppiumAndroidCreator().Create(platform, testName).GetCaps()
                //Devolve to WebDriver    
                : platform.IsAnAppleDevice()
                    ? new WebDriverIOSCreator().Create(platform, testName).GetCaps()
                    : new WebDriverAndroidCreator().Create(platform, testName).GetCaps();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 10th August 2014
 * 
 */