using Saucery.Core.DataSources;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace ExternalMerlin.XUnit;

public class RequestedPlatformData : SauceryTestData 
{
    static RequestedPlatformData() 
    {
        List<SaucePlatform> platforms =
        [
            //Mobile Platforms
            //new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "14.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 14 Pro Max Simulator", "16.2", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "76", SauceryConstants.SCREENRES_2560_1600)
        ];

        SetPlatforms(platforms);
    }

    //public static IEnumerable<object[]> AllPlatforms => GetAllPlatforms();

    public static IEnumerable<object[]> AllPlatforms
    {
        get
        {
            List<object[]> allPlatforms = [];

            foreach(var platform in Items) {
                allPlatforms.Add([platform]);
            }

            return allPlatforms.AsEnumerable();
        }
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/