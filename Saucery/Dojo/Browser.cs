using Saucery.RestAPI;
using System;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public class Browser
    {
        public string Api;
        public string DeviceName;
        public List<BrowserVersion> BrowserVersions;

        public Browser(SupportedPlatform sp)
        {
            Api = sp.api_name;
            if (sp.api_name.Equals("android") ||
                sp.api_name.Equals("iphone") ||
                sp.api_name.Equals("ipad"))
            {
                DeviceName = sp.long_name;
            }
            BrowserVersions = new List<BrowserVersion>();
        }

        public BrowserVersion FindVersion(SupportedPlatform sp)
        {
            return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
        }
    }
}