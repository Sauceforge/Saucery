﻿using Saucery.Dojo.Platforms.Base;
using Saucery.OnDemand;
using Saucery.RestAPI;
using System.Collections.Generic;
using System.Linq;

namespace Saucery.Dojo
{
    public class PlatformConfigurator
    {
        public List<PlatformBase> AvailablePlatforms { get; set; }

        public PlatformConfigurator(List<SupportedPlatform> platforms)
        {
            //To Keep
            //windows_platforms           (898)
            //mac1010_webdriver_platforms (70)
            //mac1011_webdriver_platforms (151)
            //mac1012_webdriver_platforms (190)
            //mac1013_webdriver_platforms (191)
            //mac1014_webdriver_platforms (189)
            //mac1015_webdriver_platforms (149)
            //mac11_webdriver_platforms   (93)
            //mac12_webdriver_platforms   (93)
            //ios_appium_platforms        (460)   //IOS versions is short_version. All have recommended backend version.
            //android_appium_platforms    (159)
            //TOTAL                       2494

            //var windows_platforms = platforms.FindAll(p => p.os.Contains("Windows") && p.automation_backend.Equals("webdriver"));

            //var mac1010_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.10") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            //var mac1011_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.11") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            //var mac1012_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.12") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            //var mac1013_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.13") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            //var mac1014_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.14") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            //var mac1015_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 10.15") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            //var mac11_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 11") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));
            //var mac12_webdriver_platforms = platforms.FindAll(p => p.os.Equals("Mac 12") && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));

            //var ios_appium_platforms = platforms.FindAll(p => (p.api_name.Equals("iphone") || p.api_name.Equals("ipad")) && p.automation_backend.Equals("appium"));
            //var ios_appium_platforms_byversion = ios_appium_platforms.GroupBy(g => g.recommended_backend_version);

            //var android_webdriver_platforms = platforms.FindAll(p => p.api_name.Equals("android") && p.automation_backend.Equals("webdriver"));
            //var android_webdriver_platforms_byversion = android_webdriver_platforms.GroupBy(g => g.recommended_backend_version);
            //var android_appium_platforms = platforms.FindAll(p => p.api_name.Equals("android") && p.automation_backend.Equals("appium"));
            //var android_appium_platforms_byversion = android_appium_platforms.GroupBy(g => g.recommended_backend_version);

            var filteredPlatforms = new List<SupportedPlatform>();
            filteredPlatforms.AddRange(FindWindowsPlatforms(platforms));          //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 10.10")); //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 10.11")); //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 10.12")); //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 10.13")); //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 10.14")); //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 10.15")); //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 11"));    //Not filtered for Min and Max Versions yet
            filteredPlatforms.AddRange(FindMacPlatforms(platforms, "Mac 12"));    //Not filtered for Min and Max Versions yet
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

        private static List<SupportedPlatform> FindMacPlatforms(List<SupportedPlatform> platforms, string os) => platforms.FindAll(p => p.os.Equals(os) && p.automation_backend.Equals("webdriver") && !p.api_name.Equals("ipad") && !p.api_name.Equals("iphone"));

        private List<SupportedPlatform> FindMobilePlatforms(List<SupportedPlatform> platforms, List<string> apis) => platforms.FindAll(p => apis.Any(a => a.Equals(p.api_name)) && p.automation_backend.Equals("appium"));

        public void AddLatestBrowserVersion(string version)
        {
            foreach(var p in AvailablePlatforms)
            {
                foreach (var b in p.Browsers)
                {
                    if (p.BrowsersWithLatestVersion != null && p.BrowsersWithLatestVersion.Contains(b.Name))
                    {
                        b.BrowserVersions.Add(new BrowserVersion(b.Os,
                                                                 version,
                                                                 b.Name,
                                                                 version,
                                                                 b.PlatformVersion,
                                                                 b.AutomationBackend,
                                                                 b.DeviceName,
                                                                 b.RecommendedAppiumVersion,
                                                                 null,
                                                                 null));
                    }
                }
            }
        }

        public BrowserVersion Validate(SaucePlatform requested)
        {
            BrowserVersion browserVersion = null;
            //TODO: Needs work
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
    }
}
