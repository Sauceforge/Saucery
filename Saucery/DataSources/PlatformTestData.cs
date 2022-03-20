using Newtonsoft.Json;
using Saucery.DataSources.Base;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Saucery.DataSources
{

    public class PlatformTestData : IEnumerable {
        #region Attributes
        internal static List<SaucePlatform> Platforms { get; set; }
        internal static List<BrowserVersion> BrowserVersions { get; set; }
        internal static Compositor Compositor { get; set; }
        //internal static CompositorBuilder Builder { get; set; }

        internal string Os { get; set; }
        internal string Platform { get; set; }
        internal string BrowserName { get; set; }
        internal string BrowserVersion { get; set; }
        internal string LongName { get; set; }
        internal string LongVersion { get; set; }
        internal string Url { get; set; }
        internal string Device { get; set; }
        internal string DeviceType { get; set; }
        internal string DeviceOrientation { get; set; }
        #endregion

        #region Methods
        internal double ParseBrowserVersion()
        {
            return double.Parse(BrowserVersion);
        }

        #endregion

        static PlatformTestData()
        {
            //Builder = new CompositorBuilder();
            Compositor = CompositorBuilder.Build();
            Compositor.Compose();
            Platforms = JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers);
            Platforms.ClassifyAll();
            BrowserVersions = new PlatformConfigurator().Filter(Platforms);
        }

        public IEnumerator GetEnumerator() {
            //return BrowserVersions?.GetEnumerator(); //TODO: New way

            return Platforms?.Select(platform => new SaucePlatform(platform.Os,
                                                                   platform.Browser,
                                                                   platform.BrowserVersion,
                                                                   platform.Platform,
                                                                   platform.LongName,
                                                                   platform.LongVersion,
                                                                   platform.Url,
                                                                   platform.Device,
                                                                   platform.AppiumVersion,
                                                                   platform.DeviceOrientation).Classify()).GetEnumerator();

        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */