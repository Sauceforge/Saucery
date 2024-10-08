﻿using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using Saucery.Core.OnDemand;
using Saucery.Core.Util;

namespace Saucery.Core.Options.Base;

internal abstract class BaseOptions(string testName)
{
    protected DriverOptions? Opts;
    protected Dictionary<string, object> SauceOptions = new()
    {
        { SauceryConstants.SAUCE_USERNAME_CAPABILITY, Enviro.SauceUserName! },
        { SauceryConstants.SAUCE_ACCESSKEY_CAPABILITY, Enviro.SauceApiKey! },
        //This sets the Session column
        { SauceryConstants.SAUCE_SESSIONNAME_CAPABILITY, testName },
        //This sets the Build column
        { SauceryConstants.SAUCE_BUILDNAME_CAPABILITY, Enviro.BuildName },
        //Improve performance on SauceLabs
        { SauceryConstants.SAUCE_VUOP_CAPABILITY, false }
    };

    //This sets the Session column
    //This sets the Build column
    //Improve performance on SauceLabs
    //SauceOptions.Add(Constants.VISIBILITY_KEY, Constants.VISIBILITY_TEAM);

    protected void AddSauceLabsOptions(string nativeApp) {
        if (nativeApp != null) {
            SauceOptions.Add(SauceryConstants.SAUCE_NATIVE_APP_CAPABILITY, nativeApp);
        }
    }

    public DriverOptions? GetOpts(OnDemand.PlatformType type) {
        if (type.IsMobile())
        {
            ((AppiumOptions)Opts!).AddAdditionalAppiumOption(SauceryConstants.SAUCE_USERNAME_CAPABILITY, Enviro.SauceUserName);
            ((AppiumOptions)Opts).AddAdditionalAppiumOption(SauceryConstants.SAUCE_ACCESSKEY_CAPABILITY, Enviro.SauceApiKey);
        }

        if (type.IsApple())
        {
            ((AppiumOptions)Opts!).AutomationName = SauceryConstants.APPLE_AUTOMATION_NAME;
        }

        if (type.IsAndroid())
        {
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