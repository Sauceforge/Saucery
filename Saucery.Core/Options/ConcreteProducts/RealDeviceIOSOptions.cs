using OpenQA.Selenium.Appium;
using Saucery.Core.Dojo;
using Saucery.Core.Options.Base;
using Saucery.Core.Util;

namespace Saucery.Core.Options.ConcreteProducts;

internal class RealDeviceIOSOptions : BaseOptions {
    public RealDeviceIOSOptions(BrowserVersion browserVersion, string testName) : base(testName) {
        Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.REAL_IOS_ON_APPIUM);
        DebugMessages.PrintiOSOptionValues(browserVersion);
        Console.WriteLine("Creating iOS Options");

        AddSauceLabsOptions(Enviro.SauceNativeApp!);

        AppiumOptions options = new()
        {
            PlatformName = browserVersion.PlatformNameForOption,
            BrowserName = SauceryConstants.SAFARI_BROWSER,
            DeviceName = browserVersion.DeviceName,
            PlatformVersion = browserVersion.PlatformNameForOption
        };

        //SauceOptions.Add("webviewConnectTimeout", SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        SauceOptions.Add(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, "latest");

        //if (!string.IsNullOrEmpty(browserVersion.DeviceOrientation))
        //{
        //    SauceOptions.Add(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, browserVersion.DeviceOrientation);
        //}
        options.AddAdditionalAppiumOption("webviewConnectTimeout", 50000);
        options.AddAdditionalAppiumOption("safariLogAllCommunication", true);
        options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
        Opts = options;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/