using Saucery.Core.DataSources;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;

namespace Merlin.XUnit.RealDevices;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        List<SaucePlatform> platforms =
        [
            //Real Devices
            //new AndroidRealDevice("Google Pixel 8 | Android 15 Beta", "15"),
            new AndroidRealDevice("Google.*", "15"),
            new IOSRealDevice("iPhone 15 Pro Max", "17"),
        ];

        SetPlatforms(platforms);
    }

    public static IEnumerable<object[]> AllPlatforms => GetAllPlatforms();
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 24th June 2024
* 
*/