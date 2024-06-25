using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public class AndroidRealDevice(string longName, 
                               string longVersion = "") : RealDevice(SauceryConstants.PLATFORM_LINUX, "", "", "", longName, longVersion)
{
}
