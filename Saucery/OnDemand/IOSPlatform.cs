using Saucery.Util;

namespace Saucery.OnDemand
{
    public class IOSPlatform : MobilePlatform
    {
        public IOSPlatform(string longName, string longVersion = "", string deviceOrientation = "") 
            : base (SauceryConstants.PLATFORM_IOS, "", "", "", longName, longVersion, deviceOrientation)
        {
        }
    }
}
