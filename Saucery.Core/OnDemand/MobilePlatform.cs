using Saucery.Core.OnDemand.Base;

namespace Saucery.Core.OnDemand;

public class MobilePlatform : SaucePlatform
{
    public MobilePlatform(string desktopPlatformName, string browser = "", string browserVersion = "", string platform = "", string longName = "",
                          string longVersion = "", string deviceOrientation = "") 
        : base (desktopPlatformName, browser, browserVersion, "", platform, longName, longVersion, deviceOrientation)
    {
    }
}
