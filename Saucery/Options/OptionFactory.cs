﻿using OpenQA.Selenium;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options.ConcreteCreators;
using Saucery.Util;

namespace Saucery.Options
{
    public class OptionFactory
    {
        private BrowserVersion BrowserVersion { get; set; }

        public OptionFactory(BrowserVersion browserVersion)
        {
            BrowserVersion = browserVersion;
        }

        public DriverOptions CreateOptions(string testName) {

            //TODO: Determine platform type upfront with an enum attribute on the platform.

            if (!BrowserVersion.IsAMobileDevice()) {
                DebugMessages.PrintHaveDesktopPlatform();
                return GetDesktopOptions(testName);
            }
            //Mobile Platform
            if (BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Apple))
            {
                DebugMessages.PrintHaveApplePlatform();
                return new AppiumIOSCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType);
            }
            else
            {
                DebugMessages.PrintHaveAndroidPlatform();
                return new AppiumAndroidCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType);
            }
            //return platform.CanUseAppium()
            //    //Mobile Platform
            //    ? platform.IsAnAppleDevice()
            //        ? new AppiumIOSCreator().Create(platform, testName).GetOpts()
            //        : new AppiumAndroidCreator().Create(platform, testName).GetOpts()
            //    //Devolve to WebDriver    
            //    : platform.IsAnAppleDevice()
            //        ? new WebDriverIOSCreator().Create(platform, testName).GetCaps()
            //        : new WebDriverAndroidCreator().Create(platform, testName).GetCaps();
        }

        private DriverOptions GetDesktopOptions(string testName)
        {
            return BrowserVersion.BrowserName.ToLower() switch
            {
                "firefox" => new FirefoxCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
                "internet explorer" => new IECreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
                "microsoftedge" => new EdgeCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
                "chrome" => new ChromeCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
                "safari" => new SafariCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
                _ => new ChromeCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
            };
        }

        //public bool IsSupportedPlatform()
        //{
        //    if (BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Android) || BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Apple)) {
        //        return true;
        //    }

        //    return BrowserVersion.BrowserName.ToLower() switch
        //    {
        //        "firefox" => BrowserVersion.FirefoxVersionIsSupported(),
        //        "internet explorer" => BrowserVersion.IEVersionIsSupported(),
        //        "microsoftedge" => BrowserVersion.EdgeVersionIsSupported(),
        //        "chrome" => BrowserVersion.ChromeVersionIsSupported(),
        //        "safari" => BrowserVersion.SafariVersionIsSupported(),
        //        _ => false,
        //    };
        //}
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */