using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public class IOSPlatform : MobilePlatform
{
    public IOSPlatform(string longName, string longVersion = "", string deviceOrientation = "") 
        : base (SauceryConstants.PLATFORM_IOS, "", "", "", longName, longVersion, deviceOrientation)
    {
    }
}
