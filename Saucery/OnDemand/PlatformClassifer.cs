using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.OnDemand
{
    public static class PlatformClassifer
    {
        public static List<SaucePlatform> ClassifyAll(this List<SaucePlatform> platforms)
        {
            foreach (var p in platforms)
            {
                p.Classify();
            }
            return platforms;
        }
        
        public static SaucePlatform Classify(this SaucePlatform platform)
        {
            if (platform.IsAnAndroidDevice())
            {
                platform.PlatformType = PlatformType.Android;
                return platform;
            }
            else
            {
                if (platform.IsAnAppleDevice())
                {
                    platform.PlatformType = PlatformType.Apple;
                    return platform;
                }
            }

            //Desktop
            switch (platform.Browser.ToLower())
            {
                case SauceryConstants.BROWSER_CHROME:
                    platform.PlatformType = PlatformType.Chrome;
                    break;
                case SauceryConstants.BROWSER_FIREFOX:
                    platform.PlatformType = PlatformType.Firefox;
                    break;
                case SauceryConstants.BROWSER_IE:
                    platform.PlatformType = PlatformType.IE;
                    break;
                case "microsoftedge":
                    platform.PlatformType = PlatformType.Edge;
                    break;
                case SauceryConstants.BROWSER_SAFARI:
                    platform.PlatformType = PlatformType.Safari;
                    break;
                default:
                    break;
            }

            return platform;
        }
    }
}