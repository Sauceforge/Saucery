﻿using OpenQA.Selenium.Safari;
using Saucery.Dojo;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
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