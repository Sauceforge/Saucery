using Newtonsoft.Json;
using Saucery.DataSources.Base;
using Saucery.OnDemand;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.DataSources
{
    internal class BuiltInCompositor : Compositor
    {
        public override void Compose()
        {
            var platforms = new List<SaucePlatform>
            {
                //Desktop Platforms
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "99", SauceryConstants.SCREENRES_2560_1600),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "latest", SauceryConstants.SCREENRES_2560_1600),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_81, SauceryConstants.BROWSER_CHROME, "75", SauceryConstants.SCREENRES_800_600),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, "87"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_7, SauceryConstants.BROWSER_FIREFOX, "78"),
                new SaucePlatform(SauceryConstants.PLATFORM_MAC_1015, SauceryConstants.BROWSER_SAFARI, "13"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_IE, "11"),
                new SaucePlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_EDGE, "99"),

                //Mobile Platforms
                new SaucePlatform("Linux", "Chrome", "89", "", "Android", "Google Pixel 6 Pro GoogleAPI Emulator", "12.0", "", "Android", "1.22.1", "portrait"),
                //https://github.com/SeleniumHQ/selenium/issues/10460 
                //new SaucePlatform("iOS", "iphone", "", "Mac 11", "iPhone 13 Pro Max Simulator", "15.0", "", "iphone", "1.22.0", "portrait")
            };

            var json = JsonConvert.SerializeObject(platforms);
            Enviro.SetVar(SauceryConstants.SAUCE_ONDEMAND_BROWSERS, json);
        }
    }
}