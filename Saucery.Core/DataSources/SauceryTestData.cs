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
        PlatformConfigurator platformConfigurator = new(PlatformFilter.ALL);
        PlatformExpander expander = new(platformConfigurator, platforms);
        var expandedPlatforms = expander.Expand();
        BrowserVersions = platformConfigurator.FilterAll(expandedPlatforms);
    }

    public static IEnumerable<BrowserVersion> Items => 
        BrowserVersions!
            .Select(x => x)
            .AsEnumerable();

    protected static IEnumerable<object[]> GetAllCombinations(object[] data) {
        List<object[]> allCombinations = [];

        foreach(var platform in Items)
        {
            allCombinations.AddRange(data.Select(datum => (object[]) [platform, datum]));
        }

        return allCombinations;
    }

    protected static IEnumerable<object[]> GetAllPlatforms() {
        List<object[]> allPlatforms = [];
        allPlatforms.AddRange(Items.Select(platform => (object[]) [platform]));

        return allPlatforms.AsEnumerable();
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/