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

            //TODO: Determine platform type upfront with an enum attribute on the platform.

            if (!Platform.IsAMobileDevice()) {
                DebugMessages.PrintHaveDesktopPlatform();
                return GetDesktopOptions(testName);
            }
            //Mobile Platform
            if (Platform.PlatformType.Equals(OnDemand.PlatformType.Apple))
            {
                DebugMessages.PrintHaveApplePlatform();
                return new AppiumIOSCreator().Create(Platform, testName).GetOpts(Platform.PlatformType);
            }
            else
            {
                DebugMessages.PrintHaveAndroidPlatform();
                return new AppiumAndroidCreator().Create(Platform, testName).GetOpts(Platform.PlatformType);
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
            return Platform.Browser.ToLower() switch
            {
                "firefox" => new FirefoxCreator().Create(Platform, testName).GetOpts(Platform.PlatformType),
                "internet explorer" => new IECreator().Create(Platform, testName).GetOpts(Platform.PlatformType),
                "microsoftedge" => new EdgeCreator().Create(Platform, testName).GetOpts(Platform.PlatformType),
                "chrome" => new ChromeCreator().Create(Platform, testName).GetOpts(Platform.PlatformType),
                "safari" => new SafariCreator().Create(Platform, testName).GetOpts(Platform.PlatformType),
                _ => new ChromeCreator().Create(Platform, testName).GetOpts(Platform.PlatformType),
            };
        }

        public bool IsSupportedPlatform()
        {
            if (Platform.PlatformType.Equals(OnDemand.PlatformType.Android) || Platform.PlatformType.Equals(OnDemand.PlatformType.Apple)) {
                return true;
            }

            return Platform.Browser.ToLower() switch
            {
                "firefox" => Platform.FirefoxVersionIsSupported(),
                "internet explorer" => Platform.IEVersionIsSupported(),
                "microsoftedge" => Platform.EdgeVersionIsSupported(),
                "chrome" => Platform.ChromeVersionIsSupported(),
                "safari" => Platform.SafariVersionIsSupported(),
                _ => false,
            };
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */