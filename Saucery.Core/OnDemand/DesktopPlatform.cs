using Saucery.Core.OnDemand.Base;

namespace Saucery.Core.OnDemand;

public class DesktopPlatform(string desktopPlatformName, string browser, string browserVersion, string screenResolution = "") : SaucePlatform(desktopPlatformName, browser, browserVersion, screenResolution)
{
}
