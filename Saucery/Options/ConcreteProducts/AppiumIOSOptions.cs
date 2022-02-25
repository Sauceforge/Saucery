using OpenQA.Selenium.Appium;
using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
    internal class AppiumIOSOptions : BaseOptions {
        public AppiumIOSOptions(SaucePlatform platform, string testName) : base(testName) {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.IOS_ON_APPIUM);
            AddSauceLabsOptions(Enviro.SauceNativeApp);

            DebugMessages.PrintiOSOptionValues(platform);

            Console.WriteLine("Creating iOS Options");

            var options = new AppiumOptions()
            {
                PlatformName = SauceryConstants.IOS_PLATFORM,
                BrowserName = SauceryConstants.SAFARI_BROWSER,
                DeviceName = platform.LongName,
                PlatformVersion = platform.SanitisedLongVersion()
            };

            SauceOptions.Add(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, platform.AppiumVersion);
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