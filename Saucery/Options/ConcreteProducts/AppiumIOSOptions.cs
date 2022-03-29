using OpenQA.Selenium.Appium;
using Saucery.Dojo;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
    internal class AppiumIOSOptions : BaseOptions {
        public AppiumIOSOptions(BrowserVersion browserVersion, string testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_APPIUM);
            DebugMessages.PrintiOSOptionValues(browserVersion);
            Console.WriteLine("Creating iOS Options");

            AddSauceLabsOptions(Enviro.SauceNativeApp);

            var options = new AppiumOptions
            {
                PlatformName = browserVersion.PlatformNameForOption,
                BrowserName = SauceryConstants.SAFARI_BROWSER,
                DeviceName = browserVersion.DeviceName,
                PlatformVersion = browserVersion.Name
            };

            //options.AddAdditionalAppiumOption("platformName", browserVersion.PlatformNameForOption);
            //options.AddAdditionalAppiumOption("browserName", SauceryConstants.SAFARI_BROWSER);
            
            SauceOptions.Add(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, browserVersion.RecommendedAppiumVersion);
            options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
            Opts = options;
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */