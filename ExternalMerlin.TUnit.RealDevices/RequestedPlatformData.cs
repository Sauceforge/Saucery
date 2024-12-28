using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;

namespace ExternalMerlin.TUnit.RealDevices;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        var platforms = new List<SaucePlatform>
        {
            //Real Devices
            new AndroidRealDevice("Google.*", "15"),
            new IOSRealDevice("iPhone 14 Pro Max", "16"),
        };

        SetPlatforms(platforms);
    }

    public static List<Func<BrowserVersion>> AllPlatforms() => GetAllPlatformsAsFunc();
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2024
* 
*/