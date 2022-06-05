using Saucery.OnDemand.Base;

namespace Saucery.OnDemand;

public class DesktopPlatform : SaucePlatform
{
    public DesktopPlatform(string desktopPlatformName, string browser, string browserVersion, string screenResolution = "") 
        : base(desktopPlatformName, browser, browserVersion, screenResolution)
    {
    }
}
