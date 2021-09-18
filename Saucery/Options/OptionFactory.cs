using OpenQA.Selenium;
using Saucery.OnDemand;
using Saucery.Options.ConcreteCreators;
using Saucery.Util;

namespace Saucery.Options
{
    public class OptionFactory
    {
        private SaucePlatform Platform { get; set; }

        public OptionFactory(SaucePlatform platform)
        {
            Platform = platform;
        }

        public DriverOptions CreateOptions(string testName) {
            if (Platform.IsADesktopPlatform()) {
                DebugMessages.PrintHaveDesktopPlatform();
                return GetDesktopOptions(Platform, testName);
            }
            //Mobile Platform
            if (Platform.IsAnAppleDevice())
            {
                DebugMessages.PrintHaveApplePlatform();
                return new AppiumIOSCreator().Create(Platform, testName).GetOpts();
            }
            else
            {
                DebugMessages.PrintHaveAndroidPlatform();
                return new AppiumAndroidCreator().Create(Platform, testName).GetOpts();
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

        private static DriverOptions GetDesktopOptions(SaucePlatform platform, string testName)
        {
            switch (platform.Browser.ToLower())
            {
                case "firefox":
                    return new FirefoxCreator().Create(platform, testName).GetOpts();
                case "internet explorer":
                    return new IECreator().Create(platform, testName).GetOpts();
                case "microsoftedge":
                    return new EdgeCreator().Create(platform, testName).GetOpts();
                case "chrome":
                    return new ChromeCreator().Create(platform, testName).GetOpts();
                case "safari":
                    return new SafariCreator().Create(platform, testName).GetOpts();
                default:
                    return new ChromeCreator().Create(platform, testName).GetOpts();
            }
        }

        public bool IsSupportedPlatform()
        {
            if (Platform.IsAnAndroidDevice())
            {
                return true;
            } 
            else
            {
                if (Platform.IsAnAppleDevice())
                {
                    return true;
                }
            }

            switch (Platform.Browser.ToLower())
            {
                case "firefox":
                    return Platform.FirefoxVersionIsSupported();
                case "internet explorer":
                    return Platform.IEVersionIsSupported();
                case "microsoftedge":
                    return true;
                case "chrome":
                    return Platform.ChromeVersionIsSupported();
                case "safari":
                    return Platform.SafariVersionIsSupported();
                default:
                    return false;
            }
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */