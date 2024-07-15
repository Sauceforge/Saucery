using Saucery.Core.DataSources;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;

namespace Merlin.NUnit.RealDevices;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        var platforms = new List<SaucePlatform>
        {
            //Real Devices
            //new AndroidRealDevice("Google Pixel 8 | Android 15 Beta", "15"),
            new AndroidRealDevice("Google.*", "15"),
            new IOSRealDevice("iPhone 15 Plus", "18"),
        };

        SetPlatforms(platforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 24th June 2024
* 
*/