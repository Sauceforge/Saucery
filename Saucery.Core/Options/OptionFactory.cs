using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.Options.ConcreteCreators;
using Saucery.Core.Util;

namespace Saucery.Core.Options;

public class OptionFactory
{
    private BrowserVersion BrowserVersion { get; set; }

    public OptionFactory(BrowserVersion browserVersion)
    {
        BrowserVersion = browserVersion;
    }

    public DriverOptions? CreateOptions(string testName) {
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
    }

    private DriverOptions? GetDesktopOptions(string testName) => BrowserVersion.BrowserName.ToLower() switch
    {
        SauceryConstants.BROWSER_FIREFOX => new FirefoxCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
        SauceryConstants.BROWSER_IE => new IECreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
        SauceryConstants.BROWSER_EDGE_LOWER => new EdgeCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
        SauceryConstants.BROWSER_CHROME => new ChromeCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
        SauceryConstants.BROWSER_SAFARI => new SafariCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
        _ => new ChromeCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType),
    };

    public bool IsApple()
    {
        return BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Apple);
    }

    public bool IsAndroid()
    {
        return BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Android);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/