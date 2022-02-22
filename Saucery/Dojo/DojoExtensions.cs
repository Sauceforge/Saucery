using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo
{
    public static class DojoExtensions
    {
        public static void AddPlatform(this List<AvailablePlatform> platforms, SupportedPlatform sp)
        {
            var p = platforms.FindPlatform(sp);
            if (p == null)
            {
                //first one
                p = new AvailablePlatform(sp);
                
                if(p.IsDesktopPlatform() || p.IsMobilePlatform())
                {
                    p.Browsers.AddBrowser(sp, p.FindBrowser(sp));
                }
                
                platforms.Add(p);
            }
            else
            {
                if (p.IsDesktopPlatform() || p.IsMobilePlatform())
                {
                    p.Browsers.AddBrowser(sp, p.FindBrowser(sp));
                }
            }
        }

        public static AvailablePlatform FindPlatform(this List<AvailablePlatform> platforms, SupportedPlatform sp)
        {
            return platforms.Find(p => p.Name.Equals(sp.os));
        }

        public static void AddBrowser(this List<Browser> browsers, SupportedPlatform sp, Browser b = null)
        {
            if (b == null)
            {
                //first one
                b = new Browser(sp);
                b.BrowserVersions.AddVersion(sp);
                browsers.Add(b);
            }
            else
            {
                b.BrowserVersions.AddVersion(sp);
            }
        }

        public static void AddVersion(this List<BrowserVersion> versions, SupportedPlatform sp)
        {
            var minVersion = 0;
            switch (sp.api_name) {
                case "chrome":
                    minVersion = SauceryConstants.MIN_CHROME_SUPPORTED_VERSION;
                    break;
                case "firefox":
                    minVersion = SauceryConstants.MIN_FIREFOX_SUPPORTED_VERSION;
                    break;
                case "MicrosoftEdge":
                    minVersion = SauceryConstants.MIN_EDGE_SUPPORTED_VERSION;
                    break;
                case "internet explorer":
                    minVersion = SauceryConstants.MIN_IE_SUPPORTED_VERSION;
                    break;
                default:
                    break;
            }

            switch (sp.short_version_as_int)
            {
                case 0:
                    versions.Add(new BrowserVersion(sp)); //version not numeric
                    break;
                default:
                    if (sp.short_version_as_int >= minVersion)
                    {
                        versions.Add(new BrowserVersion(sp));
                    }
                    break;
            }
        }  
    }
}