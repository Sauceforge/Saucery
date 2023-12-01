using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public class IOSPlatform(string longName, string longVersion = "", string deviceOrientation = "") : MobilePlatform(SauceryConstants.PLATFORM_IOS, "", "", "", longName, longVersion, deviceOrientation)
{
}
