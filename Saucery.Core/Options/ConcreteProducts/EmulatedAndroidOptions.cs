using OpenQA.Selenium.Appium;
using Saucery.Core.Dojo;
using Saucery.Core.Options.Base;
using Saucery.Core.Util;

namespace Saucery.Core.Options.ConcreteProducts;

internal class EmulatedAndroidOptions : BaseOptions {
    public EmulatedAndroidOptions(BrowserVersion browserVersion, string testName)
        : base(testName)
    {
        Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_APPIUM);
        AddSauceLabsOptions(Enviro.SauceNativeApp!);

        DebugMessages.PrintAndroidOptionValues(browserVersion);

        Console.WriteLine("Creating Appium Options");

        AppiumOptions options = new() {
            DeviceName = browserVersion.DeviceName,
            BrowserName = SauceryConstants.CHROME_BROWSER,
            PlatformVersion = browserVersion.Name
        };

        options.AddAdditionalAppiumOption("platformName", "Android");
        options.AddAdditionalAppiumOption("appiumVersion", browserVersion.RecommendedAppiumVersion);
        options.AddAdditionalAppiumOption("w3c", true);
        options.AddAdditionalAppiumOption("autoGrantPermissions", true);
        //appiumOptions.AddAdditionalAppiumOption("app", "path/to/your/app.apk");

        if(!string.IsNullOrEmpty(browserVersion.DeviceOrientation)) {
            SauceOptions.Add(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, browserVersion.DeviceOrientation);
        }
        options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
        Opts = options;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/