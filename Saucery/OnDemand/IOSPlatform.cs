namespace Saucery.OnDemand
{
    public class IOSPlatform : MobilePlatform
    {
        public IOSPlatform(string desktopPlatformName, string longName, string longVersion = "", string deviceOrientation = "") 
            : base (desktopPlatformName, "", "", "", longName, longVersion, "", "", deviceOrientation)
        {
        }
    }
}
