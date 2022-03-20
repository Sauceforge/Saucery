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

        public static SaucePlatform Classify(this SaucePlatform saucePlatform)
        {
            if (saucePlatform.IsAnAndroidDevice())
            {
                saucePlatform.PlatformType = PlatformType.Android;
                return saucePlatform;
            }
            else
            {
                if (saucePlatform.IsAnAppleDevice())
                {
                    saucePlatform.PlatformType = PlatformType.Apple;
                    return saucePlatform;
                }
            }

            //Desktop
            switch (saucePlatform.Browser.ToLower())
            {
                case "chrome":
                    saucePlatform.PlatformType = PlatformType.Chrome;
                    break;
                case "firefox":
                    saucePlatform.PlatformType = PlatformType.Firefox;
                    break;
                case "internet explorer":
                    saucePlatform.PlatformType = PlatformType.IE;
                    break;
                case "microsoftedge":
                    saucePlatform.PlatformType = PlatformType.Edge;
                    break;
                case "safari":
                    saucePlatform.PlatformType = PlatformType.Safari;
                    break;
                default:
                    break;
            }

            return saucePlatform;
        }
    }
}