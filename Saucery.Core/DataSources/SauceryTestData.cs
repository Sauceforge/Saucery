using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using System.Collections;

namespace Saucery.Core.DataSources;

public class SauceryTestData : IEnumerable {
    protected static List<BrowserVersion> BrowserVersions { get; set; }

    public IEnumerator GetEnumerator() => BrowserVersions?.GetEnumerator();

    protected static void SetPlatforms(List<SaucePlatform> platforms)
    {
        PlatformConfigurator platformConfigurator = new(PlatformFilter.ALL);
        PlatformExpander expander = new(platformConfigurator, platforms);
        List<SaucePlatform> expandedPlatforms = expander.Expand();
        BrowserVersions = platformConfigurator.FilterAll(expandedPlatforms);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/