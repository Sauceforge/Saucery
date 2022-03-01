using Saucery.Dojo.Browsers;
using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Platforms;
using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.OnDemand;
using Saucery.RestAPI;
using System.Collections.Generic;
using System.Linq;

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
                    b.BrowserVersions.Add(new BrowserVersion(sp, b.PlatformNameForOption));
                    browsers.Add(b);
                    return;
                }
            }
            
            //Browser found or created, now add version
            if (b.IsSupportedVersion(sp) && b.FindVersion(sp) == null) {
                b.BrowserVersions.Add(new BrowserVersion(sp, b.PlatformNameForOption));
            }
        }

        public static IList<T> GetPlatform<T>(this List<PlatformBase> availablePlatforms) where T : PlatformBase
        {
            return availablePlatforms.OfType<T>().ToList();
        }


        public static BrowserVersion FindDesktopBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
        {
            PlatformBase platform = null;
            switch (sp.Os)
            {
                case "Windows 11":
                    platform = platforms.GetPlatform<Windows11Platform>()[0];
                    break;
                case "Windows 10":
                    platform = platforms.GetPlatform<Windows10Platform>()[0];
                    break;
                case "Windows 2008":
                    platform = platforms.GetPlatform<Windows7Platform>()[0];
                    break;
                case "Windows 2012":
                    platform = platforms.GetPlatform<Windows8Platform>()[0];
                    break;
                case "Windows 2012 R2":
                    platform = platforms.GetPlatform<Windows81Platform>()[0];
                    break;
                case "Mac 10.10":
                    platform = platforms.GetPlatform<Mac1010Platform>()[0];
                    break;
                case "Mac 10.11":
                    platform = platforms.GetPlatform<Mac1011Platform>()[0];
                    break;
                case "Mac 10.12":
                    platform = platforms.GetPlatform<Mac1012Platform>()[0];
                    break;
                case "Mac 10.13":
                    platform = platforms.GetPlatform<Mac1013Platform>()[0];
                    break;
                case "Mac 10.14":
                    platform = platforms.GetPlatform<Mac1014Platform>()[0];
                    break;
                case "Mac 10.15":
                    platform = platforms.GetPlatform<Mac1015Platform>()[0];
                    break;
                case "Mac 11":
                    platform = platforms.GetPlatform<Mac11Platform>()[0];
                    break;
                case "Mac 12":
                    platform = platforms.GetPlatform<Mac12Platform>()[0];
                    break;
                default:
                    break;
            }

            if (platform == null) { return null; }

            var browsers = platform.Browsers.Find(b=>b.Os.Equals(sp.Os) && b.Name.Equals(sp.Browser));
            return browsers.BrowserVersions.Find(v => v.Os.Equals(sp.Os) && v.BrowserName.Equals(sp.Browser) && v.Name.Equals(sp.BrowserVersion));

        }

        //public static PlatformBase FindAndroidBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
        //{
        //    var androidPlatforms = new List<PlatformBase>();
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android12Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android11Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android10Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android9Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android81Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android8Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android71Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android7Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android6Platform>());
        //    androidPlatforms.AddRange(platforms.GetPlatform<Android51Platform>());

            
        //}

        //public static PlatformBase FindIOSBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
        //{
        //    var iosPlatforms = new List<PlatformBase>();
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS103Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS111Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS112Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS113Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS11Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS122Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS124Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS12Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS132Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS134Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS13Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS143Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS144Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS145Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS14Platform>());
        //    iosPlatforms.AddRange(platforms.GetPlatform<IOS15Platform>());
        //}

        private static BrowserBase FindBrowser(this List<BrowserBase> browsers, SupportedPlatform sp) {
            BrowserBase extant = null;
            foreach (var b in browsers) {
                if (b.Name.Equals(sp.api_name) && b.DeviceName.Equals(sp.long_name) && b.Os.Equals(sp.os)) {
                    extant = b;
                    break;
                }
            }
            return extant;
        }
    }
}