using System;
using Saucery.Options.Base;
using Saucery.OnDemand;
using Saucery.Util;
using OpenQA.Selenium.Safari;

namespace Saucery.Options.ConcreteProducts {
    internal class SafariBrowserOptions : BaseOptions {
        public SafariBrowserOptions(SaucePlatform platform, string testName) : base(testName)
        {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);

            DebugMessages.PrintDesktopOptionValues(platform);

            Console.WriteLine("Creating Safari Options");
            var o = new SafariOptions
            {
                PlatformName = platform.Os,
                BrowserVersion = platform.BrowserVersion
            };
            o.AddAdditionalCapability(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
            Opts = o;
        }
    }
}

/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */