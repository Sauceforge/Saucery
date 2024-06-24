﻿using Saucery.Core.DataSources;
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
            new AndroidRealDevice("Google Pixel 8 Pro", "14"),
            new IOSRealDevice("iPhone 14 Pro Max", "16")
        };

        SetPlatforms(platforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 24th June 2024
* 
*/