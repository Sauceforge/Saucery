﻿using OpenQA.Selenium.IE;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
    internal class IEBrowserOptions : BaseOptions {
        public IEBrowserOptions(BrowserVersion browserVersion, string testName) : base(testName)
        {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.DESKTOP_ON_WEBDRIVER);

            DebugMessages.PrintDesktopOptionValues(browserVersion);
            
            Console.WriteLine("Creating Internet Explorer Options");
            var o = new InternetExplorerOptions
            {
                PlatformName = browserVersion.Os,
                BrowserVersion = browserVersion.Name
            };
            //o.AddAdditionalCapability(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions, true);
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