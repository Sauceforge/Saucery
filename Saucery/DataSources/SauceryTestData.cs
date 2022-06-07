using Saucery.Dojo;
using Saucery.OnDemand;
using Saucery.OnDemand.Base;
using System.Collections;
using System.Collections.Generic;

namespace Saucery.DataSources;

public class SauceryTestData : IEnumerable {
    protected static List<BrowserVersion> BrowserVersions { get; set; }

    public IEnumerator GetEnumerator() {
        return BrowserVersions?.GetEnumerator();
    }

    protected static void SetPlatforms(List<SaucePlatform> platforms)
    {
        PlatformConfigurator platformConfigurator = new();
        PlatformExpander expander = new(platformConfigurator, platforms);
        BrowserVersions = platformConfigurator.FilterAll(expander.Expand());
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/