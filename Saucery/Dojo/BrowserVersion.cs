using Saucery.RestAPI;

namespace Saucery.Dojo
{
    public class BrowserVersion
    {
        public string Name;
        public string DeviceName;
        public BrowserVersion(SupportedPlatform sp)
        {
            Name = sp.latest_stable_version != string.Empty ? sp.latest_stable_version : sp.short_version;
            DeviceName = sp.long_name;
        }

        //public int GetMinimumBrowserVersion(string platformName, string browserName)
        //{
        //    switch (platformName)
        //    {
        //        case "Windows 2008":
        //            return new Windows7Browser(browserName).MinimumVersion();
        //        case "Windows 2012":
        //            return new Windows8Browser(browserName).MinimumVersion();
        //        case "Windows 2012 R2":
        //            return new Windows81Browser(browserName).MinimumVersion();
        //        case "Windows 10":
        //            return new Windows10Browser(browserName).MinimumVersion();
        //        case "Windows 11":
        //            return new Windows11Browser(browserName).MinimumVersion();
        //        case "Mac 11":
        //            return new Mac11Browser(browserName).MinimumVersion();
        //        case "Mac 12":
        //            return new Mac12Browser(browserName).MinimumVersion();
        //        case "Mac 10.10":
        //            return new Mac1010Browser(browserName).MinimumVersion();
        //        case "Mac 10.11":
        //            return new Mac1011Browser(browserName).MinimumVersion();
        //        case "Mac 10.12":
        //            return new Mac1012Browser(browserName).MinimumVersion();
        //        case "Mac 10.13":
        //            return new Mac1013Browser(browserName).MinimumVersion();
        //        case "Mac 10.14":
        //            return new Mac1014Browser(browserName).MinimumVersion();
        //        case "Mac 10.15":
        //            return new Mac1015Browser(browserName).MinimumVersion();
        //        case "Linux":
        //            return;
        //    }

        //    return 0;
        //}
    }
}