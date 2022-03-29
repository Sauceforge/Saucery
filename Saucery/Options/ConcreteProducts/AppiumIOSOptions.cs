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
            AddSauceLabsOptions(Enviro.SauceNativeApp);

            DebugMessages.PrintiOSOptionValues(browserVersion);

            Console.WriteLine("Creating iOS Options");

            //var options = new AppiumOptions()
            //{
            //    PlatformName = browserVersion.PlatformNameForOption,
            //    BrowserName = SauceryConstants.SAFARI_BROWSER,
            //    DeviceName = browserVersion.DeviceName,
            //    PlatformVersion = browserVersion.Name
            //};

            //SauceOptions.Add(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, browserVersion.RecommendedAppiumVersion);
            //options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
            //Opts = options;

            var options = new AppiumOptions();
            options.PlatformName = browserVersion.PlatformNameForOption;
            options.AddAdditionalAppiumOption("platformName", browserVersion.PlatformNameForOption);
            
            options.BrowserName = SauceryConstants.SAFARI_BROWSER;
            options.AddAdditionalAppiumOption("browserName", SauceryConstants.SAFARI_BROWSER);
            //options.AddAdditionalOption("appium:deviceName", browserVersion.DeviceName);
            //options.AddAdditionalOption("appium:platformVersion", browserVersion.Name);
            
            options.DeviceName = browserVersion.DeviceName;
            //options.AddAdditionalAppiumOption("deviceName", browserVersion.DeviceName);
            options.PlatformVersion = browserVersion.Name;

            //var sauceOptions = new Dictionary<string, object>();
            //sauceOptions.Add("appiumVersion", "1.22.2");
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