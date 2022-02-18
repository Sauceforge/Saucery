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
            var platforms =  new List<SaucePlatform>
            {
                //Desktop Platforms
                new SaucePlatform("Windows 10", "chrome", "latest", "", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "chrome", "88", "", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "chrome", "75", "", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "firefox", "87", "", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "firefox", "86", "", "", "", "", "", "", ""),
                new SaucePlatform("macOS 10.15", "safari", "13", "", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "internet explorer", "11", "", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "microsoftedge", "98", "", "", "", "", "", "", ""),

                //Mobile Platforms
                new SaucePlatform("Android", "Chrome", "89", "Android", "Google Pixel 3 GoogleAPI Emulator", "12.0", "", "Android", "1.22.1", "portrait"),
                //new SaucePlatform("iOS", "Safari", "13", "iOS", "iPhone 12 Pro Simulator", "14.3", "", "iphone", "1.21.0", "portrait")
        };

            var json = JsonConvert.SerializeObject(platforms);
            Enviro.SetVar(SauceryConstants.SAUCE_ONDEMAND_BROWSERS, json);
        }
    }
}