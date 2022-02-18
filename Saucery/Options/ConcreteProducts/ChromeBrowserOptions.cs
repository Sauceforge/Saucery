using System;
using Saucery.Options.Base;
using Saucery.OnDemand;
using Saucery.Util;
using OpenQA.Selenium.Chrome;

namespace Saucery.Options.ConcreteProducts {
    internal class ChromeBrowserOptions : BaseOptions {
        public ChromeBrowserOptions(SaucePlatform platform, string testName) : base(testName)
        {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);

            DebugMessages.PrintDesktopOptionValues(platform);

            Console.WriteLine("Creating Chrome Options");
            var o = new ChromeOptions
            {
                BrowserVersion = platform.BrowserVersion,
                PlatformName = platform.Os,
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