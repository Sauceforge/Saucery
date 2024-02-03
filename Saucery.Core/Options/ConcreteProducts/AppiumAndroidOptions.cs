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

        AppiumOptions options = new() 
        {
            PlatformName = browserVersion.PlatformNameForOption,
            BrowserName = SauceryConstants.CHROME_BROWSER,
            DeviceName = browserVersion.DeviceName,
            PlatformVersion = browserVersion.Name,
        };

        SauceOptions.Add(SauceryConstants.SAUCE_APPIUM_VERSION_CAPABILITY, browserVersion.RecommendedAppiumVersion);
        SauceOptions.Add("idleTimeout", 1000);

        if (!string.IsNullOrEmpty(browserVersion.DeviceOrientation))
        {
            SauceOptions.Add(SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, browserVersion.DeviceOrientation);
        }
        options.AddAdditionalAppiumOption(SauceryConstants.SAUCE_OPTIONS_CAPABILITY, SauceOptions);
        Opts = options;
    }
}

//Build: https://app.saucelabs.com/builds/vdc/129a1f32fc803fcbb0af8014fc54f4f7
//https://app.saucelabs.com/tests/55649d023b9042829c4f4c3bfc3a92b4
//https://app.saucelabs.com/tests/60c13baaa183414bbb66713b901a71d8

//capabilities = new MutableCapabilities();
//capabilities.setCapability("platformName", "Android");
//capabilities.setCapability("browserName", "Chrome");
//capabilities.setCapability("appium:deviceName", "Android GoogleAPI Emulator");
//capabilities.setCapability("appium:newCommandTimeout", 90);
//capabilities.setCapability("appium:platformVersion", "11.0");
//capabilities.setCapability("automationName", "UiAutomator2");
////capabilities.setCapability("name", name.getMethodName());

//MutableCapabilities sauceOptions = new MutableCapabilities();
//sauceOptions.setCapability("build", "supportTest");
//sauceOptions.setCapability("name", "supportTest");
//sauceOptions.setCapability("appiumVersion", "2.0.0");
//sauceOptions.setCapability("sessionCreationTimeout", "10");
//capabilities.setCapability("sauce:options", sauceOptions);

//driver = new AndroidDriver<>(new URL("https://username:accesskey@ondemand.us-west-1.saucelabs.com:443/wd/hub"), capabilities);

/*
* Copyright Andrew Gray, SauceForge
* Date: 5th February 2020
* 
*/