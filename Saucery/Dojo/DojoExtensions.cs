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
                p = PlatformFactory.CreatePlatform(sp);
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
            PlatformBase result = null;

            List<PlatformBase> mobilePlatforms = platforms.FindAll(p=>p.AutomationBackend.Equals("appium"));
            List<PlatformBase> desktopPlatforms = platforms.FindAll(p=>p.AutomationBackend.Equals("webdriver"));

            if (sp.IsMobilePlatform())
            {
                foreach(var m in mobilePlatforms)
                {
                    result = mobilePlatforms.Find(mp => mp.Name.Equals(sp.os) && mp.PlatformVersion.Equals(sp.short_version));
                    if(result != null)
                    {
                        break;
                    }
                }

            } else {
                foreach (var d in desktopPlatforms)
                {
                    result = desktopPlatforms.Find(dp => dp.Name.Equals(sp.os));
                    if (result != null)
                    {
                        break;
                    }
                }

            }

            //foreach (var p in platforms)
            //{
            //    if (sp.IsMobilePlatform() && p.IsMobilePlatform())
            //    {
            //        result = platforms.Find(p => p.Name.Equals(sp.os) && p.PlatformVersion.Equals(sp.short_version));
            //    }

            //    return p.IsMobilePlatform()
            //        ? platforms.Find(p => p.Name.Equals(sp.os) && p.PlatformVersion.Equals(sp.short_version))
            //        : platforms.Find(p => p.Name.Equals(sp.os));
            //}

            return result;
        }

        public static void AddBrowser(this List<BrowserBase> browsers, SupportedPlatform sp)
        {
            BrowserBase b = FindBrowser(browsers, sp);

            if (b == null) {
                //first one
                b = BrowserFactory.CreateBrowser(sp);
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