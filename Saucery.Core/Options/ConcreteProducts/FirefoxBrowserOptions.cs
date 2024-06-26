﻿using OpenQA.Selenium.Firefox;
using Saucery.Core.Dojo;
using Saucery.Core.Options.Base;
using Saucery.Core.Util;

namespace Saucery.Core.Options.ConcreteProducts;

internal class FirefoxBrowserOptions : BaseOptions {
    public FirefoxBrowserOptions(BrowserVersion browserVersion, string testName) : base(testName)
    {
        Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);
        DebugMessages.PrintDesktopOptionValues(browserVersion);
        Console.WriteLine("Creating Firefox Options");

        FirefoxOptions o = new() 
        {
            PlatformName = browserVersion.Os,
            BrowserVersion = browserVersion.Name
        };

        if (!string.IsNullOrEmpty(browserVersion.ScreenResolution))
        {
            SauceOptions.Add(SauceryConstants.SCREEN_RESOLUTION_CAPABILITY, browserVersion.ScreenResolution);
        }

        o.AddAdditionalOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
        Opts = o;
    }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/