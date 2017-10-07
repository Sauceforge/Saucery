using System;
using OpenQA.Selenium.Remote;
using Saucery.Capabilities.ConcreteCreators;
using Saucery.Extensions;
using Saucery.TestDataSources;

namespace Saucery.Capabilities {
    internal class CapabilityFactory {
        //public static DesiredCapabilities CreateCapabilities(PlatformTestData platform, String testName, string nativeApp, bool useChromeOnAndroid) {
        public static DesiredCapabilities CreateCapabilities(PlatformTestData platform, String testName)
        {
            if (platform.IsADesktopPlatform()) {
                return (new DesktopCreator()).Create(platform, testName).GetCaps();
            }
            //Mobile Platform
            return platform.CanUseAppium()
                //Mobile Platform
                ? platform.IsAnAppleDevice()
                    //? (new AppiumIOSCreator()).Create(platform, testName, nativeApp).GetCaps()
                    //: (new AppiumAndroidCreator()).Create(platform, testName, nativeApp, useChromeOnAndroid).GetCaps()
                    ? (new AppiumIOSCreator()).Create(platform, testName).GetCaps()
                    : (new AppiumAndroidCreator()).Create(platform, testName).GetCaps()
                //Devolve to WebDriver    
                : platform.IsAnAppleDevice()
                    ? (new WebDriverIOSCreator()).Create(platform, testName).GetCaps()
                    : (new WebDriverAndroidCreator()).Create(platform, testName).GetCaps();
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 10th August 2014
 * 
 */