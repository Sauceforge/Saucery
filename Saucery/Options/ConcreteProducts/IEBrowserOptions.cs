using System;
using Saucery.Options.Base;
using Saucery.OnDemand;
using Saucery.Util;
using OpenQA.Selenium.IE;

namespace Saucery.Options.ConcreteProducts {
    internal class IEBrowserOptions : BaseOptions {
        public IEBrowserOptions(SaucePlatform platform, string testName) : base(testName)
        {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);

            DebugMessages.PrintDesktopOptionValues(platform);
            
            Console.WriteLine("Creating Internet Explorer Options");
            var o = new InternetExplorerOptions
            {
                PlatformName = platform.Os,
                BrowserVersion = platform.BrowserVersion
            };
            o.AddAdditionalCapability(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions, true);
            Opts = o;
        }
    }
}

/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */