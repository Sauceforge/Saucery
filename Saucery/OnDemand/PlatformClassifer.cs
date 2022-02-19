namespace Saucery.OnDemand
{
    public static class PlatformClassifer
    {
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
                case "chrome":
                    platform.PlatformType = PlatformType.Chrome;
                    break;
                case "firefox":
                    platform.PlatformType = PlatformType.Firefox;
                    break;
                case "internet explorer":
                    platform.PlatformType = PlatformType.IE;
                    break;
                case "microsoftedge":
                    platform.PlatformType = PlatformType.Edge;
                    break;
                case "safari":
                    platform.PlatformType = PlatformType.Safari;
                    break;
                default:
                    break;
            }

            return platform;
        }
    }
}