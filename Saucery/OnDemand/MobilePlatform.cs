using Saucery.OnDemand.Base;

namespace Saucery.OnDemand
{
    public class MobilePlatform : SaucePlatform
    {
        public MobilePlatform(string desktopPlatformName, string browser = "", string browserVersion = "", string screenResolution = "", string platform = "", string longName = "",
                              string longVersion = "", string url = "", string device = "", string appiumVersion = "", string deviceOrientation = "") 
            : base (desktopPlatformName, browser, browserVersion, screenResolution, platform, longName, longVersion, url, device, appiumVersion, deviceOrientation)
        {
        }
    }
}
