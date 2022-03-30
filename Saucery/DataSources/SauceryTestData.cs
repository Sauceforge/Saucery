using Saucery.Dojo;
using Saucery.OnDemand.Base;
using System.Collections;
using System.Collections.Generic;

namespace Saucery.DataSources
{
    public class SauceryTestData : IEnumerable {
        protected static List<BrowserVersion> BrowserVersions { get; set; }

        public IEnumerator GetEnumerator() {
            return BrowserVersions?.GetEnumerator();
        }

        protected static void SetPlatforms(List<SaucePlatform> platforms)
        {
            BrowserVersions = new PlatformConfigurator().Filter(platforms).ClassifyAll();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */