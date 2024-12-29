using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using System.Collections;

namespace Saucery.Core.DataSources;

public class SauceryTestData : IEnumerable 
{
    private static List<BrowserVersion>? BrowserVersions { get; set; }

    public IEnumerator GetEnumerator() => BrowserVersions?.GetEnumerator()!;

    protected static void SetPlatforms(List<SaucePlatform> platforms)
    {
        PlatformConfigurator platformConfigurator = new(PlatformFilter.All);
        ExpandAndFilter(platforms, platformConfigurator);
        //PlatformExpander expander = new(platformConfigurator, platforms);
        //var expandedPlatforms = expander.Expand();
        //BrowserVersions = platformConfigurator.FilterAll(expandedPlatforms);
    }

    protected static void SetPlatforms(List<SaucePlatform> platforms, PlatformFilter filter) {
        PlatformConfigurator platformConfigurator = new(filter);
        ExpandAndFilter(platforms, platformConfigurator);
        //PlatformExpander expander = new(platformConfigurator, platforms);
        //var expandedPlatforms = expander.Expand();
        //BrowserVersions = platformConfigurator.FilterAll(expandedPlatforms);
    }

    private static void ExpandAndFilter(List<SaucePlatform> platforms, PlatformConfigurator platformConfigurator)
    {
        PlatformExpander expander = new(platformConfigurator, platforms);
        var expandedPlatforms = expander.Expand();
        BrowserVersions = platformConfigurator.FilterAll(expandedPlatforms);
    }

    public static IEnumerable<BrowserVersion> Items => 
        BrowserVersions!
            .Select(x => x)
            .AsEnumerable();

    protected static IEnumerable<object[]> GetAllPlatforms() {
        List<object[]> allPlatforms = [];
        allPlatforms.AddRange(Items.Select(platform => (object[]) [platform]));

        return allPlatforms.AsEnumerable();
    }

    protected static List<Func<BrowserVersion>> GetAllPlatformsAsFunc() => 
        Items.Select(platform => (Func<BrowserVersion>)(() => platform)).ToList();
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/