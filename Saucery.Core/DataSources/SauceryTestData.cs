using System.Collections;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;

namespace Saucery.Core.DataSources;

public class SauceryTestData : IEnumerable 
{
    private static List<BrowserVersion>? BrowserVersions { get; set; }

    public IEnumerator GetEnumerator() => BrowserVersions?.GetEnumerator()!;

    protected static void SetPlatforms(List<SaucePlatform> platforms, PlatformConfigurator configurator) => 
        ExpandAndFilter(platforms, configurator);

    protected static void SetPlatforms(List<SaucePlatform> platforms, PlatformFilter filter) => 
        ExpandAndFilter(platforms, new(filter));

    private static void ExpandAndFilter(List<SaucePlatform> platforms, PlatformConfigurator platformConfigurator)
    {
        PlatformExpander expander = new(platformConfigurator, platforms);
        var expandedPlatforms = expander.Expand();
        BrowserVersions = platformConfigurator.FilterAll(expandedPlatforms);
    }

    /// <summary>
    /// Returns fresh instances to ensure test isolation.
    /// Each access creates new BrowserVersion instances to prevent state leakage across tests.
    /// </summary>
    public static IEnumerable<BrowserVersion> Items =>
        BrowserVersions!
            .Select(x => new BrowserVersion(x))
            .AsEnumerable();

    /// <summary>
    /// Returns platforms wrapped in object arrays, with fresh instances for test isolation.
    /// </summary>
    protected static IEnumerable<object[]> GetAllPlatforms() {
        List<object[]> allPlatforms = [];
        allPlatforms.AddRange(Items.Select(platform => (object[])[platform]));

        return allPlatforms.AsEnumerable();
    }

    /// <summary>
    /// Returns factory functions that create fresh BrowserVersion instances when invoked.
    /// This ensures each test gets its own independent instance, preventing state leakage.
    /// </summary>
    protected static List<Func<BrowserVersion>> GetAllPlatformsAsFunc() =>
        [.. BrowserVersions!.Select(template => (Func<BrowserVersion>)(() => new BrowserVersion(template)))];
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/