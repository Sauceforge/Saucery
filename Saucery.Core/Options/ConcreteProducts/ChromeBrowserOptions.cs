using OpenQA.Selenium.Chrome;
using Saucery.Core.Dojo;
using Saucery.Core.Options.Base;
using Saucery.Core.Util;

namespace Saucery.Core.Options.ConcreteProducts;

internal class ChromeBrowserOptions : BaseOptions {
    public ChromeBrowserOptions(BrowserVersion browserVersion, string testName) : base(testName)
    {
        Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);
        DebugMessages.PrintDesktopOptionValues(browserVersion);
        Console.WriteLine("Creating Chrome Options");
        
        var o = new ChromeOptions
        {
            BrowserVersion = browserVersion.Name,
            PlatformName = browserVersion.Os,
            UseSpecCompliantProtocol = true
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