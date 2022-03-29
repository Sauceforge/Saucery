using Saucery.Dojo.Platforms.Base;
using Saucery.OnDemand;
using Saucery.RestAPI;
using Saucery.RestAPI.SupportedPlatforms;
using Saucery.Util;
using System.Collections.Generic;
using System.Linq;

namespace Saucery.Dojo
{
    public class PlatformConfigurator
    {
        SauceLabsPlatformAcquirer PlatformAcquirer { get; set; }
        
        public List<PlatformBase> AvailablePlatforms { get; set; }

        public PlatformConfigurator()
        {
            PlatformAcquirer = new SauceLabsPlatformAcquirer();
            var platforms = PlatformAcquirer.AcquirePlatforms();

            var filteredPlatforms = new List<SupportedPlatform>();
            //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindWindowsPlatforms(platforms));
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, new List<string> { SauceryConstants.PLATFORM_MAC_1010, 
                                                                                      SauceryConstants.PLATFORM_MAC_1011,
                                                                                      SauceryConstants.PLATFORM_MAC_1012,
                                                                                      SauceryConstants.PLATFORM_MAC_1013,
                                                                                      SauceryConstants.PLATFORM_MAC_1014,
                                                                                      SauceryConstants.PLATFORM_MAC_1015,
                                                                                      SauceryConstants.PLATFORM_MAC_11,
                                                                                      SauceryConstants.PLATFORM_MAC_12 }));
            filteredPlatforms.AddRange(FindMobilePlatforms(platforms, new List<string> { "iphone", "ipad" }));
            filteredPlatforms.AddRange(FindMobilePlatforms(platforms, new List<string> { "android" }));

            AvailablePlatforms = new List<PlatformBase>();
            foreach (var sp in filteredPlatforms)
            {
                AvailablePlatforms.AddPlatform(sp);
            }

            AddLatestBrowserVersion("latest");
            AddLatestBrowserVersion("latest-1");
        }

        private static List<SupportedPlatform> FindWindowsPlatforms(List<SupportedPlatform> platforms) => platforms.FindAll(p => p.os.Contains("Windows") && p.automation_backend.Equals("webdriver"));

        private static List<SupportedPlatform> FindMacPlatforms(List<SupportedPlatform> platforms, List<string> oses) => platforms.FindAll(p => oses.Any(o => o.Equals(p.os)) && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));

        private static List<SupportedPlatform> FindMobilePlatforms(List<SupportedPlatform> platforms, List<string> apis) => platforms.FindAll(p => apis.Any(a => a.Equals(p.api_name)) && p.automation_backend.Equals("appium"));

        public void AddLatestBrowserVersion(string version)
        {
            foreach(var p in AvailablePlatforms)
            {
                foreach (var b in p.Browsers)
                {
                    if (p.BrowsersWithLatestVersion != null && p.BrowsersWithLatestVersion.Contains(b.Name))
                    {
                        b.BrowserVersions.Add(new BrowserVersion(b, version, version, null, null));
                    }
                }
            }
        }

        public BrowserVersion Validate(SaucePlatform requested)
        {
            BrowserVersion browserVersion = null;
            switch (requested.PlatformType)
            {
                case PlatformType.Chrome:
                case PlatformType.Edge:
                case PlatformType.Firefox:
                case PlatformType.IE:
                case PlatformType.Safari:
                    browserVersion = AvailablePlatforms.FindDesktopBrowser(requested);
                    break;
                case PlatformType.Android:
                    browserVersion = AvailablePlatforms.FindAndroidBrowser(requested);
                    break;
                case PlatformType.Apple:
                    browserVersion = AvailablePlatforms.FindIOSBrowser(requested);
                    break;
                default:
                    break;
            }

            //Console.WriteLine("{0} of {1} platforms request are valid", valid.Count, requested.Count);
            return browserVersion;
        }

        internal List<BrowserVersion> Filter(List<SaucePlatform> platforms)
        {
            var bvs = new List<BrowserVersion>();

            foreach (var p in platforms)
            {
                var bv = Validate(p);
                if (bv != null)
                {
                    bv.ScreenResolution = p.ScreenResolution;
                    bv.DeviceOrientation = p.DeviceOrientation;
                    bvs.Add(bv);
                }
            }

            return bvs;

            //return (from p in platforms
            //        let bv = Validate(p)
            //        where bv != null
            //        select bv).ToList();
        }
    }
}
