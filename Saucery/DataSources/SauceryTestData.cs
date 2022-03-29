using Saucery.DataSources.Base;
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
        protected static Compositor Compositor { get; set; }
        #endregion

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