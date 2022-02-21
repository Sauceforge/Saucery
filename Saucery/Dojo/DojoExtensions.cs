using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public static class DojoExtensions
    {
        public static void AddPlatform(this List<AvailablePlatform> platforms, SupportedPlatform platform)
        {
            var p = platforms.FindPlatform(platform);
            if (p == null)
            {
                //first one
                p = new AvailablePlatform(platform);
                p.Browsers.AddBrowser(platform, p.FindBrowser(platform));
                platforms.Add(p);
            }
            
            p.Browsers.AddBrowser(platform, p.FindBrowser(platform));
        }
        
        public static void AddBrowser(this List<Browser> browsers, SupportedPlatform sp, Browser b = null)
        {
            if (b == null)
            {
                //first one
                b = new Browser(sp);
                b.BrowserVersions.AddVersion(sp, b.FindVersion(sp));
                browsers.Add(b);
            }
            b.BrowserVersions.AddVersion(sp, b.FindVersion(sp));

        }

        public static void AddVersion(this List<BrowserVersion> versions, SupportedPlatform sp, BrowserVersion bv = null)
        {
            if (bv == null) {
                //first one
                bv = new BrowserVersion(sp);
            }
            versions.Add(bv);
        }
        

        public static AvailablePlatform FindPlatform(this List<AvailablePlatform> platforms, SupportedPlatform sp)
        {
            return platforms.Find(p => p.Name.Equals(sp.os));
        }
    }
}
