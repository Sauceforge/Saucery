using Saucery.Core.OnDemand.Base;

namespace Saucery.Core.OnDemand;

public class DesktopPlatform : SaucePlatform
{
    public DesktopPlatform(string desktopPlatformName, string browser, string browserVersion, string screenResolution = "") 
        : base(desktopPlatformName, browser, browserVersion, screenResolution)
    {
    }
}
