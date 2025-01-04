using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;

namespace ExternalMerlin.NUnit.RealDevices;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        List<SaucePlatform> platforms =
        [
            //Real Devices
            new AndroidRealDevice("Google Pixel 9 Pro XL", "15"),
            new IOSRealDevice("iPhone 14 Pro Max", "16"),
        ];

        SetPlatforms(platforms, PlatformFilter.RealDevice);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/