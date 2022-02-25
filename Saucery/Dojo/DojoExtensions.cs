using Saucery.Dojo.Browsers;
using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Platforms;
using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public static class DojoExtensions
    {
        public static void AddPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
        {
            var p = platforms.FindPlatform(sp);
            if (p == null)
            {
                //first one
                p = new PlatformFactory().CreatePlatform(sp);
                p.Browsers.AddBrowser(sp);
                platforms.Add(p);
            }
            else
            {
                p.Browsers.AddBrowser(sp);
            }
        }

        public static PlatformBase FindPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
        {
            return platforms.Find(p => p.Name.Equals(sp.os));
        }

        public static void AddBrowser(this List<BrowserBase> browsers, SupportedPlatform sp)
        {
            BrowserBase b = FindBrowser(browsers, sp);

            if (b == null) {
                //first one
                b = new BrowserFactory(sp).CreateBrowser();
                if (b == null) {
                    return;
                }
                if (b.IsSupportedVersion(sp)) {
                    b.BrowserVersions.Add(new BrowserVersion(sp));
                    browsers.Add(b);
                    return;
                }
            }
            
            //Browser found or created, now add version
            if (b.IsSupportedVersion(sp) && b.FindVersion(sp) == null) {
                b.BrowserVersions.Add(new BrowserVersion(sp));
            }
        }

        private static BrowserBase FindBrowser(this List<BrowserBase> browsers, SupportedPlatform sp) {
            BrowserBase extant = null;
            foreach (var b in browsers) {
                if (b.Name.Equals(sp.api_name) && b.DeviceName.Equals(sp.long_name) && b.PlatformName.Equals(sp.os)) {
                    extant = b;
                    break;
                }
            }
            return extant;
        }
    }
}