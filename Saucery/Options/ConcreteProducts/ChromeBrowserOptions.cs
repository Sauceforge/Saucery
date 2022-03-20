using OpenQA.Selenium.Chrome;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
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
            //o.AddAdditionalCapability(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions, true);
            o.AddAdditionalOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
            //o.AddAdditionalChromeOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
            Opts = o;
        }
    }
}

/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */