using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using Saucery.Core.OnDemand;
using Saucery.Core.Util;

namespace Saucery.Core.Options.Base;

internal abstract class BaseOptions(string testName) {
    protected DriverOptions? Opts;
    protected Dictionary<string, object> SauceOptions = new()
    {
        { SauceryConstants.SAUCE_USERNAME_CAPABILITY, Enviro.SauceUserName! },
        { SauceryConstants.SAUCE_ACCESSKEY_CAPABILITY, Enviro.SauceApiKey! },
        { SauceryConstants.SAUCE_SESSIONNAME_CAPABILITY, testName },
        { SauceryConstants.SAUCE_BUILDNAME_CAPABILITY, Enviro.BuildName },
        { SauceryConstants.SAUCE_VUOP_CAPABILITY, false }
    };

    /// <summary>
    /// Adds SauceLabs options for native applications.
    /// </summary>
    /// <param name="nativeApp">The native application to be tested.</param>
    protected void AddSauceLabsOptions(string? nativeApp) {
        if(!string.IsNullOrEmpty(nativeApp)) {
            SauceOptions[SauceryConstants.SAUCE_NATIVE_APP_CAPABILITY] = nativeApp;
        }
    }

    /// <summary>
    /// Gets the driver options based on the platform type.
    /// </summary>
    /// <param name="type">The platform type.</param>
    /// <returns>The configured driver options.</returns>
    public DriverOptions? GetOpts(OnDemand.PlatformType type) {
        if(type.IsMobile()) {
            var appiumOptions = (AppiumOptions)Opts!;
            appiumOptions.AddAdditionalAppiumOption(SauceryConstants.SAUCE_USERNAME_CAPABILITY, Enviro.SauceUserName);
            appiumOptions.AddAdditionalAppiumOption(SauceryConstants.SAUCE_ACCESSKEY_CAPABILITY, Enviro.SauceApiKey);
        }

        if(type.IsApple()) {
            ((AppiumOptions)Opts!).AutomationName = SauceryConstants.APPLE_AUTOMATION_NAME;
        }

        if(type.IsAndroid()) {
            ((AppiumOptions)Opts!).AutomationName = SauceryConstants.ANDROID_AUTOMATION_NAME;
        }

        return Opts;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/
