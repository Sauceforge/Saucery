using Saucery.Dojo.Browsers;
using Saucery.Dojo.Browsers.Base;
using Saucery.Dojo.Platforms;
using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.OnDemand;
using Saucery.RestAPI;
using Saucery.Util;
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
                if(p == null)
                {
                    //SauceLabs may have just added it to the platform configurator.  Don't fall over.
                    return;
                }
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
                if (b != null) {
                    b.AddVersion(browsers, sp, false);
                }
            } else
            {
                b.AddVersion(browsers, sp, true);
            }

            
        }

        private static void AddVersion(this BrowserBase b, List<BrowserBase> browsers, SupportedPlatform sp, bool findVersion)
        {
            if (findVersion)
            {
                //Browser found or created, now add version
                if (b.IsSupportedVersion(sp) && b.FindVersion(sp) == null)
                {
                    var bv = new BrowserVersion(sp, b.PlatformNameForOption);
                    b.BrowserVersions.Add(bv);
                }
            }
            else
            {
                if (b.IsSupportedVersion(sp))
                {
                    var bv = new BrowserVersion(sp, b.PlatformNameForOption);
                    b.BrowserVersions.Add(bv);
                    browsers.Add(b);
                }
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
                case SauceryConstants.PLATFORM_WINDOWS_11:
                    platform = platforms.GetPlatform<Windows11Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_WINDOWS_10:
                    platform = platforms.GetPlatform<Windows10Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_WINDOWS_81:
                    platform = platforms.GetPlatform<Windows81Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_WINDOWS_8:
                    platform = platforms.GetPlatform<Windows8Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_WINDOWS_7:
                    platform = platforms.GetPlatform<Windows7Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_12:
                    platform = platforms.GetPlatform<Mac12Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_11:
                    platform = platforms.GetPlatform<Mac11Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_1015:
                    platform = platforms.GetPlatform<Mac1015Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_1014:
                    platform = platforms.GetPlatform<Mac1014Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_1013:
                    platform = platforms.GetPlatform<Mac1013Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_1012:
                    platform = platforms.GetPlatform<Mac1012Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_1011:
                    platform = platforms.GetPlatform<Mac1011Platform>()[0];
                    break;
                case SauceryConstants.PLATFORM_MAC_1010:
                    platform = platforms.GetPlatform<Mac1010Platform>()[0];
                    break;
                default:
                    break;
            }

            if (platform == null) { return null; }
            var browsers = platform.Browsers.Find(b=>b.Os.Equals(sp.Os) && b.Name.ToLower().Equals(sp.Browser.ToLower()));
            if (browsers == null) { return null; }
            return browsers.BrowserVersions.Find(v => v.Os.Equals(sp.Os) && v.BrowserName.ToLower().Equals(sp.Browser.ToLower()) && v.Name.ToLower().Equals(sp.BrowserVersion.ToLower()));
        }

        public static BrowserVersion FindAndroidBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
        {
            PlatformBase platform = null;
            var platformToSearchFor = string.Format("{0} {1}", sp.Os, sp.LongVersion);
            switch (platformToSearchFor)
            {
                case "Linux 12.0":
                    platform = platforms.GetPlatform<Android12Platform>()[0];
                    break;
                case "Linux 11.0":
                    platform = platforms.GetPlatform<Android11Platform>()[0];
                    break;
                case "Linux 10.0":
                    platform = platforms.GetPlatform<Android10Platform>()[0];
                    break;
                case "Linux 9.0":
                    platform = platforms.GetPlatform<Android9Platform>()[0];
                    break;
                case "Linux 8.1":
                    platform = platforms.GetPlatform<Android81Platform>()[0];
                    break;
                case "Linux 8.0":
                    platform = platforms.GetPlatform<Android8Platform>()[0];
                    break;
                case "Linux 7.1":
                    platform = platforms.GetPlatform<Android71Platform>()[0];
                    break;
                case "Linux 7.0":
                    platform = platforms.GetPlatform<Android7Platform>()[0];
                    break;
                case "Linux 6.0":
                    platform = platforms.GetPlatform<Android6Platform>()[0];
                    break;
                case "Linux 5.1":
                    platform = platforms.GetPlatform<Android51Platform>()[0];
                    break;
                default:
                    break;
            }

            if (platform == null) { return null; }
            var browsers = platform.Browsers.Find(b => b.Os.Equals(sp.Os) && b.DeviceName.Equals(sp.LongName));
            if (browsers == null) { return null; }
            return browsers.BrowserVersions.Count == 1
                ? browsers.BrowserVersions[0]
                : browsers.BrowserVersions.Find(v => v.Os.Equals(sp.Os) && v.DeviceName.Equals(sp.LongName) && v.Name.Equals(sp.BrowserVersion));
        }

        public static BrowserVersion FindIOSBrowser(this List<PlatformBase> platforms, SaucePlatform sp)
        {
            PlatformBase platform = null;
            var platformToSearchFor = string.Format("{0} {1}", sp.Os, sp.LongVersion);
            switch (platformToSearchFor)
            {
                case "iOS 15.0":
                    platform = platforms.GetPlatform<IOS15Platform>()[0];
                    break;
                case "iOS 14.5":
                    platform = platforms.GetPlatform<IOS145Platform>()[0];
                    break;
                case "iOS 14.4":
                    platform = platforms.GetPlatform<IOS144Platform>()[0];
                    break;
                case "iOS 14.3":
                    platform = platforms.GetPlatform<IOS143Platform>()[0];
                    break;
                case "iOS 14.0":
                    platform = platforms.GetPlatform<IOS14Platform>()[0];
                    break;
                case "iOS 13.4":
                    platform = platforms.GetPlatform<IOS134Platform>()[0];
                    break;
                case "iOS 13.2":
                    platform = platforms.GetPlatform<IOS132Platform>()[0];
                    break;
                case "iOS 13.0":
                    platform = platforms.GetPlatform<IOS13Platform>()[0];
                    break;
                case "iOS 12.4":
                    platform = platforms.GetPlatform<IOS124Platform>()[0];
                    break;
                case "iOS 12.2":
                    platform = platforms.GetPlatform<IOS122Platform>()[0];
                    break;
                case "iOS 12.0":
                    platform = platforms.GetPlatform<IOS12Platform>()[0];
                    break;
                case "iOS 11.3":
                    platform = platforms.GetPlatform<IOS113Platform>()[0];
                    break;
                case "iOS 11.2":
                    platform = platforms.GetPlatform<IOS112Platform>()[0];
                    break;
                case "iOS 11.1":
                    platform = platforms.GetPlatform<IOS111Platform>()[0];
                    break;
                case "iOS 11.0":
                    platform = platforms.GetPlatform<IOS11Platform>()[0];
                    break;
                case "iOS 10.3":
                    platform = platforms.GetPlatform<IOS103Platform>()[0];
                    break;
                default:
                    break;
            }

            if (platform == null) { return null; }
            var browsers = platform.Browsers.Find(b => b.PlatformNameForOption.Equals(sp.Os) && b.DeviceName.Equals(sp.LongName));
            if (browsers == null) { return null; }
            return browsers.BrowserVersions.Count == 1
                ? browsers.BrowserVersions[0]
                : browsers.BrowserVersions.Find(v => v.PlatformNameForOption.Equals(sp.Os) && v.DeviceName.Equals(sp.LongName) && v.Name.Equals(sp.BrowserVersion));
        }

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


        public static List<BrowserVersion> ClassifyAll(this List<BrowserVersion> browserVersions)
        {
            foreach (var bv in browserVersions)
            {
                bv.Classify();
            }
            return browserVersions;
        }

        public static BrowserVersion Classify(this BrowserVersion browserVersion)
        {
            if (browserVersion.IsAnAndroidDevice())
            {
                browserVersion.PlatformType = PlatformType.Android;
                return browserVersion;
            }
            else
            {
                if (browserVersion.IsAnAppleDevice())
                {
                    browserVersion.PlatformType = PlatformType.Apple;
                    return browserVersion;
                }
            }

            //Desktop
            switch (browserVersion.BrowserName.ToLower())
            {
                case SauceryConstants.BROWSER_CHROME:
                    browserVersion.PlatformType = PlatformType.Chrome;
                    break;
                case SauceryConstants.BROWSER_FIREFOX:
                    browserVersion.PlatformType = PlatformType.Firefox;
                    break;
                case SauceryConstants.BROWSER_IE:
                    browserVersion.PlatformType = PlatformType.IE;
                    break;
                case SauceryConstants.BROWSER_EDGE:
                    browserVersion.PlatformType = PlatformType.Edge;
                    break;
                case SauceryConstants.BROWSER_SAFARI:
                    browserVersion.PlatformType = PlatformType.Safari;
                    break;
                default:
                    break;
            }

            return browserVersion;
        }

    }
}