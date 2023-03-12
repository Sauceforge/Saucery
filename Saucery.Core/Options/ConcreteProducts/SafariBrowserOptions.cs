using OpenQA.Selenium.Safari;
using Saucery.Core.Dojo;
using Saucery.Core.Options.Base;
using Saucery.Core.Util;

namespace Saucery.Core.Options.ConcreteProducts;

internal class SafariBrowserOptions : BaseOptions {
    public SafariBrowserOptions(BrowserVersion browserVersion, string testName) : base(testName)
    {
        Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);
        DebugMessages.PrintDesktopOptionValues(browserVersion);
        Console.WriteLine("Creating Safari Options");

        var o = new SafariOptions
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