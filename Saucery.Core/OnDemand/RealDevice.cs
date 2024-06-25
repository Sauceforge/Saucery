using Saucery.Core.OnDemand.Base;

namespace Saucery.Core.OnDemand;

public class RealDevice(string desktopPlatformName, 
                        string browser = "", 
                        string browserVersion = "", 
                        string platform = "", 
                        string longName = "",
                        string longVersion = "") : SaucePlatform(desktopPlatformName, browser, browserVersion, "", platform, longName, longVersion)
{
}
