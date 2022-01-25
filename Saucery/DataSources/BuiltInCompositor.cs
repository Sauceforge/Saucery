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
                new SaucePlatform("Windows 10", "chrome", "latest", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "chrome", "88", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "chrome", "75", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "firefox", "87", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "firefox", "86", "", "", "", "", "", ""),
                new SaucePlatform("macOS 10.15", "safari", "13.1", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "internet explorer", "11.285", "", "", "", "", "", ""),
                new SaucePlatform("Windows 10", "microsoftedge", "89.0", "", "", "", "", "", ""),

                //Mobile Platforms
                new SaucePlatform("android", "android", "chrome", "89.0", "Google Pixel 3 GoogleAPI Emulator", "11.0.", "", "android", "portrait"),
                new SaucePlatform("", "", "Safari", "14.3", "iPhone 12 Pro Simulator", "14.3", "", "iphone", "portrait")
        };

            var json = JsonConvert.SerializeObject(platforms);
            Enviro.SetVar(SauceryConstants.SAUCE_ONDEMAND_BROWSERS, json);
        }
    }
}