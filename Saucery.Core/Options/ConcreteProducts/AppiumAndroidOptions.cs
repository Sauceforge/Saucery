using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using Saucery.Core.Dojo;
using Saucery.Core.Options.Base;
using Saucery.Core.Util;

namespace Saucery.Core.Options.ConcreteProducts;

internal class AppiumAndroidOptions : BaseOptions {
    public AppiumAndroidOptions(BrowserVersion browserVersion, string testName)
        : base(testName)
    {
        Console.WriteLine(SauceryConstants.SETTING_UP, testName, SauceryConstants.ANDROID_ON_APPIUM);
        AddSauceLabsOptions(Enviro.SauceNativeApp!);

        DebugMessages.PrintAndroidOptionValues(browserVersion);

        Console.WriteLine("Creating Appium Options");

        AppiumOptions options = new();
        options.AddAdditionalAppiumOption("platformName", "Android");
        options.DeviceName = "Android Emulator";
        options.PlatformVersion = browserVersion.Name;
        //appiumOptions.AddAdditionalAppiumOption("app", "path/to/your/app.apk");
        //appiumOptions.AddAdditionalAppiumOption("name", "Your Test Name");
        options.AddAdditionalAppiumOption("appiumVersion", "1.21.0"); // Set the appropriate Appium version
        options.AddAdditionalAppiumOption("autoGrantPermissions", true);

        // Initialize the AndroidDriver with Sauce Labs capabilities
        //AndroidDriver<IWebElement> driver = new AndroidDriver<IWebElement>(sauceUrl, appiumOptions);

        //var options = new AppiumOptions
        //{
        //    PlatformName = browserVersion.PlatformNameForOption,
        //    BrowserName = SauceryConstants.CHROME_BROWSER,
        //    DeviceName = browserVersion.DeviceName,
        //    PlatformVersion = browserVersion.Name,
        //};

        //SauceOptions.Add(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, browserVersion.RecommendedAppiumVersion);
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