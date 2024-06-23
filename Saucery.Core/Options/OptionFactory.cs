using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.Options.ConcreteCreators;
using Saucery.Core.Util;

namespace Saucery.Core.Options;

public class OptionFactory(BrowserVersion browserVersion) : IDisposable
{
    private BrowserVersion BrowserVersion { get; set; } = browserVersion;

    public DriverOptions? CreateOptions(string testName)
    {
        if (!BrowserVersion.IsAMobileDevice())
        {
            DebugMessages.PrintHaveDesktopPlatform();
            return GetDesktopOptions(testName);
        }

        //Mobile Device
        if(browserVersion.IsARealDevice()) {
            if(BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Apple)) {
                DebugMessages.PrintHaveApplePlatform(true);
                return new RealDeviceIOSCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType);
            } else {
                DebugMessages.PrintHaveAndroidPlatform(true);
                return new RealDeviceAndroidCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType);
            }
        }
        else {
            //Emulated Mobile Platform
            if(BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Apple)) {
                DebugMessages.PrintHaveApplePlatform(false);
                return new EmulatedIOSCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType);
            } else {
                DebugMessages.PrintHaveAndroidPlatform(false);
                return new EmulatedAndroidCreator().Create(BrowserVersion, testName).GetOpts(BrowserVersion.PlatformType);
            }
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

    public bool IsApple() => BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Apple);

    public bool IsAndroid() => BrowserVersion.PlatformType.Equals(OnDemand.PlatformType.Android);

    public void Dispose() => GC.SuppressFinalize(this);
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/