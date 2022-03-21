using Newtonsoft.Json;
using Saucery.DataSources.Base;
using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.Util;
using System.Collections;
using System.Collections.Generic;

namespace Saucery.DataSources
{

    public class PlatformTestData : IEnumerable {
        #region Attributes
        internal static List<SaucePlatform> Platforms { get; set; }
        internal static List<BrowserVersion> BrowserVersions { get; set; }
        internal static Compositor Compositor { get; set; }
        #endregion
        
        static PlatformTestData()
        {
            Compositor = CompositorBuilder.Build();
            Compositor.Compose();
            Platforms = JsonConvert.DeserializeObject<List<SaucePlatform>>(Enviro.SauceOnDemandBrowsers);
            Platforms.ClassifyAll();
            BrowserVersions = new PlatformConfigurator().Filter(Platforms);
            BrowserVersions.ClassifyAll();
        }

        public IEnumerator GetEnumerator() {
            return BrowserVersions?.GetEnumerator();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */