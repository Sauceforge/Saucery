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
                
                if(p.IsDesktopPlatform(sp) || sp.IsMobilePlatform())
                {
                    p.Browsers.AddBrowser(sp, p.FindBrowser(sp));
                }
                
                platforms.Add(p);
            }
            else
            {
                if (p.IsDesktopPlatform(sp) || sp.IsMobilePlatform())
                {
                    p.Browsers.AddBrowser(sp, p.FindBrowser(sp));
                }
            }
        }

        public static PlatformBase FindPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
        {
            return platforms.Find(p => p.Name.Equals(sp.os));
        }

        public static void AddBrowser(this List<BrowserBase> browsers, SupportedPlatform sp, BrowserBase b = null)
        {
            if (b == null)
            {
                //first one
                b = new BrowserFactory(sp).CreateBrowser();
            }


            var v = b.FindVersion(sp);
            if (v == null && b.IsSupportedVersion(sp))
            {
                b.BrowserVersions.Add(new BrowserVersion(sp));
                browsers.Add(b);
            }
        }
    }
}