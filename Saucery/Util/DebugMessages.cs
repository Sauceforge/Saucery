﻿using Saucery.Dojo;
using Saucery.OnDemand;
using System;

namespace Saucery.Util;

public class DebugMessages {
    internal static void PrintHaveDesktopPlatform()
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine("DEBUG MESSAGE: We have a desktop platform");
            Console.Out.Flush();
        }
    }

    internal static void PrintHaveAndroidPlatform()
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine("DEBUG MESSAGE: We have an Android platform");
            Console.Out.Flush();
        }
    }

    internal static void PrintHaveApplePlatform()
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine("DEBUG MESSAGE: We have an Apple platform");
            Console.Out.Flush();
        }
    }

    public static void ExtractJsonSegment(string json, int startIndex, int endIndex)
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine("DEBUG MESSAGE: ExtractJsonSegment params {0} {1} {2}", json, startIndex, endIndex);
            Console.Out.Flush();
        }
    }

    public static void PrintDesktopOptionValues(BrowserVersion browserVersion)
    {
        if (UserChecker.ItIsMe())
        {
            Console.WriteLine("Desktop platform.Browser: {0}", browserVersion.BrowserName);
            Console.WriteLine("Desktop platform.Os: {0}", browserVersion.Os);
            Console.WriteLine("Desktop platform.BrowserVersion: {0}", browserVersion.Name);
        }
    }

    public static void PrintiOSOptionValues(BrowserVersion browserVersion)
    {
        Console.WriteLine("{0}:{1}\n{2}:{3}\n{4}:{5}\n{6}:{7}\n{8}:{9}\n{10}:{11}",
                          SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, browserVersion.DeviceName,
                          SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, browserVersion.DeviceOrientation,
                          SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, browserVersion.Name,
                          SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.PLATFORM_IOS,
                          SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, SauceryConstants.SAFARI_BROWSER,
                          SauceryConstants.SAUCE_DEVICE_CAPABILITY, browserVersion.IsAnIPhone() ? SauceryConstants.IPHONE_SIMULATOR : SauceryConstants.IPAD_SIMULATOR);
    }

    public static void PrintAndroidOptionValues(BrowserVersion browserVersion)
    {
        Console.WriteLine("{0}:{1}\n{2}:{3}\n{4}:{5}\n{6}:{7}\n{8}:{9}",
                          SauceryConstants.SAUCE_DEVICE_NAME_CAPABILITY, browserVersion.DeviceName,
                          SauceryConstants.SAUCE_DEVICE_ORIENTATION_CAPABILITY, browserVersion.DeviceOrientation,
                          SauceryConstants.SAUCE_BROWSER_NAME_CAPABILITY, SauceryConstants.CHROME_BROWSER,
                          SauceryConstants.SAUCE_PLATFORM_VERSION_CAPABILITY, browserVersion.Name,
                          SauceryConstants.SAUCE_PLATFORM_NAME_CAPABILITY, SauceryConstants.ANDROID);
    }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 25th August 2014
* 
*/