using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public class IOSRealDevice(string longName, string longVersion = "") : RealDevice(SauceryConstants.PLATFORM_IOS, "", "", "", longName, longVersion)
{
}
