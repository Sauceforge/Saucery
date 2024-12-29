using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;

namespace Saucery.Core.Util;

public class DebugMessages {
    internal static void PrintHaveDesktopPlatform()
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine("DEBUG MESSAGE: We have a desktop platform");
            Console.Out.Flush();
        }
    }

    internal static void AvailablePlatformsAlreadyPopulated() 
    {
        if(UserChecker.ItIsMe()) {
            Console.WriteLine("DEBUG MESSAGE: AvailablePlatforms is already populated");
            Console.Out.Flush();
        }
    }

    internal static void AvailablePlatformsEmpty() {
        if(UserChecker.ItIsMe()) {
            Console.WriteLine("DEBUG MESSAGE: AvailablePlatforms is empty");
            Console.Out.Flush();
        }
    }

    internal static void PrintHaveAndroidPlatform(bool isReal)
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine(isReal ? "DEBUG MESSAGE: We have an Android Real Device" : "DEBUG MESSAGE: We have an emulated Android platform");
            Console.Out.Flush();
        }
    }

    internal static void PrintHaveApplePlatform(bool isReal)
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine(isReal ? "DEBUG MESSAGE: We have an Apple Real Device" : "DEBUG MESSAGE: We have an emulated Apple platform");
            Console.Out.Flush();
        }
    }

    public static void PrintDesktopOptionValues(BrowserVersion browserVersion)
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine($"Desktop platform.Browser: {browserVersion.BrowserName}");
            Console.WriteLine($"Desktop platform.Os: {browserVersion.Os}");
            Console.WriteLine($"Desktop platform.BrowserVersion: {browserVersion.Name}");
        }
    }

    public static void PrintiOSOptionValues(BrowserVersion browserVersion) => 
        Console.WriteLine($"{SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY}: {browserVersion.DeviceName}\n" +
                          $"{SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY}: {browserVersion.DeviceOrientation}\n" +
                          $"{SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY}: {browserVersion.Name}\n" +
                          $"{SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY}: {SauceryConstants.PLATFORM_IOS}\n" +
                          $"{SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY}: {SauceryConstants.SAFARI_BROWSER}\n" +
                          $"{SauceryConstants.SAUCE_DEVICE_CAPABILITY}: {0}",
                          browserVersion.IsAnIPhone() ? SauceryConstants.IPHONE_SIMULATOR : SauceryConstants.IPAD_SIMULATOR);

    public static void PrintAndroidOptionValues(BrowserVersion browserVersion) => 
        Console.WriteLine($"{SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY}:{browserVersion.DeviceName}\n" +
                          $"{SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY}:{browserVersion.DeviceOrientation}\n" +
                          $"{SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY}:{SauceryConstants.CHROME_BROWSER}\n" +
                          $"{SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY}:{browserVersion.Name}\n" +
                          $"{SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY}:{SauceryConstants.ANDROID}");
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 25th August 2014
* 
*/