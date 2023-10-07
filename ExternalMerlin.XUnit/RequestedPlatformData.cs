using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace ExternalMerlin.XUnit;

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

    public static IEnumerable<object[]> Platforms => BrowserVersions.Select(x => x.ToObjectArray()).AsEnumerable();

    public static IEnumerable<BrowserVersion> Items => BrowserVersions.Select(x => x).AsEnumerable();
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/