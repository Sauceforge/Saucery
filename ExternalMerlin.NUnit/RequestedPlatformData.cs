using Saucery.Core.DataSources;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace ExternalMerlin.NUnit;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        var platforms = new List<SaucePlatform>
        {
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_2560_1600),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "76", SauceryConstants.SCREENRES_2560_1600)
        };

        SetPlatforms(platforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/