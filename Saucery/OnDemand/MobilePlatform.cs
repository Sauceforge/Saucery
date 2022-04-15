using Saucery.OnDemand.Base;

namespace Saucery.OnDemand
{
    public class MobilePlatform : SaucePlatform
    {
        public MobilePlatform(string desktopPlatformName, string browser = "", string browserVersion = "", string platform = "", string longName = "",
                              string longVersion = "", string appiumVersion = "", string deviceOrientation = "") 
            : base (desktopPlatformName, browser, browserVersion, "", platform, longName, longVersion, appiumVersion, deviceOrientation)
        {
        }
    }
}
