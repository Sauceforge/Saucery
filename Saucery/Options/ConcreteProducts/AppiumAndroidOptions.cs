using OpenQA.Selenium.Appium;
using Saucery.Dojo;
using Saucery.Options.Base;
using Saucery.Util;
using System;

namespace Saucery.Options.ConcreteProducts
{
    internal class AppiumAndroidOptions : BaseOptions {
        public AppiumAndroidOptions(BrowserVersion browserVersion, string testName)
            : base(testName)
        {
            Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_APPIUM);
            AddSauceLabsOptions(Enviro.SauceNativeApp);

            DebugMessages.PrintAndroidOptionValues(browserVersion);

            Console.WriteLine("Creating Appium Options");

            var options = new AppiumOptions
            {
                PlatformName = browserVersion.PlatformNameForOption,
                BrowserName = SauceryConstants.CHROME_BROWSER,
                DeviceName = browserVersion.DeviceName,
                PlatformVersion = browserVersion.Name
            };

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