using OpenQA.Selenium.Appium;
using Saucery.OnDemand;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
    internal class AppiumAndroidOptions : BaseOptions {
        public AppiumAndroidOptions(SaucePlatform platform, string testName)
            : base(testName)
        {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_APPIUM);
            var sanitisedLongVersion = platform.SanitisedLongVersion();
            AddSauceLabsOptions(Enviro.SauceNativeApp);

            DebugMessages.PrintAndroidOptionValues(platform, sanitisedLongVersion);

            Console.WriteLine("Creating Appium Options");

            var options = new AppiumOptions
            {
                PlatformName = SauceryConstants.ANDROID,
                BrowserName = SauceryConstants.CHROME_BROWSER,
                DeviceName = platform.LongName,
                PlatformVersion = sanitisedLongVersion
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