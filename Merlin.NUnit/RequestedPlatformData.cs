using Saucery.Core.DataSources;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace Merlin.NUnit;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        var platforms = new List<SaucePlatform>
        {
            //Mobile Platforms
            //new AndroidPlatform("Google Pixel 6 Pro GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            //new IOSPlatform("iPhone 13 Pro Max Simulator", "15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75->114", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_11, SauceryConstants.BROWSER_CHROME, "latest"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_12, SauceryConstants.BROWSER_CHROME, "latest"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_13, SauceryConstants.BROWSER_CHROME, "latest"),
            new DesktopPlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "16", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11", SauceryConstants.SCREENRES_800_600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99", SauceryConstants.SCREENRES_800_600)
        };

        SetPlatforms(platforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/