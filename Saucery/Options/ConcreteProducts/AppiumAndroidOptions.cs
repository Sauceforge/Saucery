using System;
using OpenQA.Selenium.Appium;
using Saucery.Options.Base;
using Saucery.OnDemand;
using Saucery.Util;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;

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

            var options = new AppiumOptions();
            options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.ANDROID);
            options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, SauceryConstants.CHROME_BROWSER);
            options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.LongName);
            options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, sanitisedLongVersion);
            SauceOptions.Add(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, SauceryConstants.LATEST_APPIUM_VERSION);
            options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
            Opts = options;
            
            //Opts = new ChromeOptions();
            //Opts.AddAdditionalCapability(SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, platform.Device);
            //Opts.AddAdditionalCapability(SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, sanitisedLongVersion);
            //Opts.AddAdditionalCapability(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, platform.DeviceOrientation);


            //Opts.AddAdditionalCapability(SauceOpsConstants.SAUCE_BROWSER_NAME_CAPABILITY, SauceOpsConstants.CHROME_BROWSER);  //Required
            //Opts.AddAdditionalCapability(SauceOpsConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceOpsConstants.ANDROID);

            //Opts.AddAdditionalChromeOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */