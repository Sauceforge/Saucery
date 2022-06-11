﻿using Saucery.Dojo.Platforms.Base;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using Saucery.RestAPI;
using Saucery.RestAPI.SupportedPlatforms;
using Saucery.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saucery.Dojo;

public class PlatformConfigurator
{
    SauceLabsPlatformAcquirer PlatformAcquirer { get; set; }
    public List<PlatformBase> AvailablePlatforms { get; set; }

    public PlatformConfigurator()
    {
        PlatformAcquirer = new SauceLabsPlatformAcquirer();
        var platforms = PlatformAcquirer.AcquirePlatforms();

        var filteredPlatforms = new List<SupportedPlatform>();
        //Not filtered for Min and Max Versions yet
        filteredPlatforms.AddRange(FindWindowsPlatforms(platforms));
        filteredPlatforms.AddRange(FindMacPlatforms(platforms, new List<string> { SauceryConstants.PLATFORM_MAC_1010, 
                                                                                  SauceryConstants.PLATFORM_MAC_1011,
                                                                                  SauceryConstants.PLATFORM_MAC_1012,
                                                                                  SauceryConstants.PLATFORM_MAC_1013,
                                                                                  SauceryConstants.PLATFORM_MAC_1014,
                                                                                  SauceryConstants.PLATFORM_MAC_1015,
                                                                                  SauceryConstants.PLATFORM_MAC_11,
                                                                                  SauceryConstants.PLATFORM_MAC_12 }));
        filteredPlatforms.AddRange(FindMobilePlatforms(platforms, new List<string> { "iphone", "ipad" }));
        filteredPlatforms.AddRange(FindMobilePlatforms(platforms, new List<string> { "android" }));

        AvailablePlatforms = new List<PlatformBase>();
        foreach (var sp in filteredPlatforms)
        {
            AvailablePlatforms.AddPlatform(sp);
        }

        AddLatestBrowserVersion(SauceryConstants.BROWSER_VERSION_LATEST);
        AddLatestBrowserVersion(SauceryConstants.BROWSER_VERSION_LATEST_MINUS1);
    }

    internal int FindMaxBrowserVersion(SaucePlatform platform)
    {
        //Desktop Platform Only
        var availablePlatform = AvailablePlatforms.Find(p => p.Name.Equals(platform.Os));
        var browser = availablePlatform.Browsers.Find(b => b.Name.Equals(platform.Browser));
        var numericBrowserVersions = browser.BrowserVersions.Where(x => x.Name.Any(char.IsNumber));
        var browserVersion = numericBrowserVersions.Aggregate((maxItem, nextItem) => (int.Parse(maxItem.Name) > int.Parse(nextItem.Name)) ? maxItem : nextItem);

        return int.Parse(browserVersion.Name);
    }

    private static List<SupportedPlatform> FindWindowsPlatforms(List<SupportedPlatform> platforms) => platforms.FindAll(p => p.os.Contains("Windows") && p.automation_backend.Equals("webdriver"));

    private static List<SupportedPlatform> FindMacPlatforms(List<SupportedPlatform> platforms, List<string> oses) => platforms.FindAll(p => oses.Any(o => o.Equals(p.os)) && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));

    private static List<SupportedPlatform> FindMobilePlatforms(List<SupportedPlatform> platforms, List<string> apis) => platforms.FindAll(p => apis.Any(a => a.Equals(p.api_name)) && p.automation_backend.Equals("appium"));

    public void AddLatestBrowserVersion(string version)
    {
        foreach(var p in AvailablePlatforms)
        {
            foreach (var b in p.Browsers)
            {
                if (p.BrowsersWithLatestVersion != null && p.BrowsersWithLatestVersion.Contains(b.Name))
                {
                    b.BrowserVersions.Add(new BrowserVersion(b, version, version, null, null));
                }
            }
        }
    }

    public List<BrowserVersion> FilterAll(List<SaucePlatform> platforms)
    {
        var bvs = new List<BrowserVersion>();

        foreach (var p in platforms)
        {
            var bv = Filter(p);
            if (bv != null)
            {
                bvs.Add(bv);
            }
        }

        Console.WriteLine(SauceryConstants.NUM_VALID_PLATFORMS, bvs.Count, platforms.Count);
        return bvs;
    }

    public BrowserVersion Filter(SaucePlatform platform)
    {
        var bv = Validate(platform);
        if (bv != null)
        {
            bv.ScreenResolution = platform.ScreenResolution;
            bv.DeviceOrientation = platform.DeviceOrientation;
        }
        return bv;
    }

    public BrowserVersion Validate(SaucePlatform requested)
    {
        BrowserVersion browserVersion = null;
        requested.Classify();
        switch (requested.PlatformType)
        {
            case PlatformType.Chrome:
            case PlatformType.Edge:
            case PlatformType.Firefox:
            case PlatformType.IE:
            case PlatformType.Safari:
                browserVersion = AvailablePlatforms.FindDesktopBrowser(requested);
                break;
            case PlatformType.Android:
                browserVersion = AvailablePlatforms.FindAndroidBrowser(requested);
                break;
            case PlatformType.Apple:
                browserVersion = AvailablePlatforms.FindIOSBrowser(requested);
                break;
            default:
                break;
        }

        return browserVersion != null ? browserVersion.Classify() : browserVersion;
    }
}