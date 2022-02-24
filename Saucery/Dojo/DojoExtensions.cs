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
                    p.Browsers.AddBrowser(sp);
                }
                
                platforms.Add(p);
            }
            else
            {
                if (p.IsDesktopPlatform(sp) || sp.IsMobilePlatform())
                {
                    p.Browsers.AddBrowser(sp);
                }
            }
        }

        public static PlatformBase FindPlatform(this List<PlatformBase> platforms, SupportedPlatform sp)
        {
            return platforms.Find(p => p.Name.Equals(sp.os));
        }

        public static void AddBrowser(this List<BrowserBase> browsers, SupportedPlatform sp)
        {
            BrowserBase existent = null;
            foreach (var b in browsers)
            {
                if (b.Name.Equals(sp.api_name) && b.DeviceName.Equals(sp.long_name) && b.PlatformName.Equals(sp.os))
                {
                    existent = b;
                }
            }

            if (existent == null)
            {
                //first one
                existent = new BrowserFactory(sp).CreateBrowser();
                if (existent == null || !existent.IsSupportedVersion(sp))
                {
                    return;
                }
                if (existent.IsSupportedVersion(sp))
                {
                    existent.BrowserVersions.Add(new BrowserVersion(sp));
                    browsers.Add(existent);
                    return;
                }
            }
 
            if (existent.FindVersion(sp) == null && existent.IsSupportedVersion(sp))
            {
                existent.BrowserVersions.Add(new BrowserVersion(sp));
            }
        }
    }
}