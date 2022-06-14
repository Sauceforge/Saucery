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
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_2560_1600)
        };

        SetPlatforms(platforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/