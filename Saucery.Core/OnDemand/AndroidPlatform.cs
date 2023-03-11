using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public class AndroidPlatform : MobilePlatform
{
    public AndroidPlatform(string longName, string longVersion = "", string deviceOrientation = "") 
        : base (SauceryConstants.PLATFORM_LINUX, "", "", "", longName, longVersion, deviceOrientation)
    {
    }
}
