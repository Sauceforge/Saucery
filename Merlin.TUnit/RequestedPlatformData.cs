﻿using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;
using System.Collections;

namespace Merlin.TUnit;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        var platforms = new List<SaucePlatform>
        {
            //Emulated Mobile Platforms
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "15.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 14 Pro Max Simulator", "16.2", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "100", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "latest"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "latest", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "15", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99", SauceryConstants.SCREENRES_800_600)
        };

        SetPlatforms(platforms);
    }

    public static List<Func<BrowserVersion>> AllPlatforms() => GetAllPlatformsAsFunc();
    public static List<BrowserVersion> AllPlatformsAsList() => Items.ToList();

    public static List<Func<DataDrivenData>> AllCombinations(object[] data)
    {
        List<Func<DataDrivenData>> dataDrivenFuncs = [];

        foreach (var platform in AllPlatforms())
        {
            foreach (var obj in data)
            {
                dataDrivenFuncs.Add(() => new DataDrivenData
                {
                    Data = (int)obj,
                    RequestedPlatform = platform()
                });
            }
        }

        return dataDrivenFuncs;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2024
* 
*/