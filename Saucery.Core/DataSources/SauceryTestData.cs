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

    protected static List<BrowserVersion> GetAllPlatformsAsList()
    {
        List<BrowserVersion> allPlatforms = [];
        allPlatforms.AddRange(from platform in Items
                              select platform);
        return allPlatforms;
    }

    protected static List<Func<BrowserVersion>> GetAllPlatformsAsFunc()
    {
        var listOfFunc = new List<Func<BrowserVersion>>();

        foreach (var platform in Items)
        {
            listOfFunc.Add(() => platform);
        }

        return listOfFunc;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/