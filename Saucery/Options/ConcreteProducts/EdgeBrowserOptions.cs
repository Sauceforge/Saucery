using OpenQA.Selenium.Edge;
using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
    internal class EdgeBrowserOptions : BaseOptions {
        public EdgeBrowserOptions(SaucePlatform platform, string testName) : base(testName)
        {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);

            DebugMessages.PrintDesktopOptionValues(platform);
            
            Console.WriteLine("Creating Edge Options");
            var o = new EdgeOptions
            {
                PlatformName = platform.Os,
                BrowserVersion = platform.BrowserVersion
            };
            o.AddAdditionalOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
            Opts = o;
        }
    }
}

/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */