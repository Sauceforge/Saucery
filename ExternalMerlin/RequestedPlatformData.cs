using Saucery.DataSources;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using Saucery.Util;

namespace ExternalMerlin;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        var platforms = new List<SaucePlatform>
        {
            //Mobile Platforms
            new AndroidPlatform("Google Pixel 6 Pro GoogleAPI Emulator", "12.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 13 Pro Max Simulator", "15.4", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75->102", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST, SauceryConstants.SCREENRES_2560_1600)
        };

        SetPlatforms(platforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/