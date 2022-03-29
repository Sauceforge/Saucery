using Saucery.DataSources;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using Saucery.Util;
using System.Collections.Generic;

namespace Merlin
{
    public class RequestedPlatformData : SauceryTestData
    {
        static RequestedPlatformData()
        {
            var platforms = new List<SaucePlatform>
            {
                //Desktop Platforms
                new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99", SauceryConstants.SCREENRES_2560_1600),
                new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "latest", SauceryConstants.SCREENRES_2560_1600),
                new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_800_600),
                new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "87"),
                new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78"),
                new DesktopPlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "13"),
                new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
                new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),

                //Mobile Platforms
                new MobilePlatform(SauceryConstants.PLATFORM_LINUX, "Chrome", "89", "Android", "Google Pixel 6 Pro GoogleAPI Emulator", "12.0", "Android", "1.22.1", "portrait"),
                new MobilePlatform(SauceryConstants.PLATFORM_IOS, "iphone", "", SauceryConstants.PLATFORM_MAC_11, "iPhone 13 Pro Max Simulator", "15.0", "iphone", "1.22.0", "portrait")
            };

            SetPlatforms(platforms);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */