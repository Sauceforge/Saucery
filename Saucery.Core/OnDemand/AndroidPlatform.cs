using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public class AndroidPlatform(string longName, 
                             string longVersion = "", 
                             string deviceOrientation = "") : MobilePlatform(SauceryConstants.PLATFORM_LINUX, "", "", "", longName, longVersion, deviceOrientation)
{
}
