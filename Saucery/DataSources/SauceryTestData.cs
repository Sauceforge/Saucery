using Saucery.Dojo;
using Saucery.OnDemand;
using System.Collections;
using System.Collections.Generic;

namespace Saucery.DataSources
{
    public class SauceryTestData : IEnumerable {
        #region Attributes
        protected static List<SaucePlatform> Platforms { get; set; }
        protected static List<BrowserVersion> BrowserVersions { get; set; }
        #endregion

        public IEnumerator GetEnumerator() {
            return BrowserVersions?.GetEnumerator();
        }

        protected static void SetPlatforms(List<SaucePlatform> platforms)
        {
            Platforms = platforms.ClassifyAll();
            BrowserVersions = new PlatformConfigurator().Filter(Platforms).ClassifyAll();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */