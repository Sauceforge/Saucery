using Saucery.Core.OnDemand.Base;

namespace Saucery.Core.OnDemand;

public class MobilePlatform(string desktopPlatformName, 
                            string browser = "", 
                            string browserVersion = "", 
                            string platform = "", 
                            string longName = "",
                            string longVersion = "", 
                            string deviceOrientation = "") : SaucePlatform(desktopPlatformName, browser, browserVersion, "", platform, longName, longVersion, deviceOrientation)
{
}
