using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public static class PlatformClassifer
{
    public static SaucePlatform Classify(this SaucePlatform platform)
    {
        if (platform.IsAnAndroidDevice())
        {
            platform.PlatformType = PlatformType.Android;
            return platform;
        }

        if (platform.IsAnAppleDevice())
        {
            platform.PlatformType = PlatformType.Apple;
            return platform;
        }

        //Desktop
        platform.PlatformType = platform.Browser.ToLower() switch
        {
            SauceryConstants.BROWSER_CHROME => PlatformType.Chrome,
            SauceryConstants.BROWSER_FIREFOX => PlatformType.Firefox,
            SauceryConstants.BROWSER_IE => PlatformType.IE,
            SauceryConstants.BROWSER_EDGE_LOWER => PlatformType.Edge,
            SauceryConstants.BROWSER_SAFARI => PlatformType.Safari,
            _ => platform.PlatformType
        };

        return platform;
    }
}