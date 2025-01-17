using Saucery.Core.Dojo;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace Saucery.Core.OnDemand;

public class PlatformExpander(PlatformConfigurator platformConfigurator, List<SaucePlatform> platforms)
{
    private List<SaucePlatform> ExpandedSet { get; set; } = [];
    private List<SaucePlatform> Platforms { get; set; } = platforms;
    private PlatformConfigurator PlatformConfigurator { get; set; } = platformConfigurator;

    public List<SaucePlatform> Expand()
    {
        foreach (var platform in Platforms)
        {
            if (!platform.NeedsExpansion() || 
                platform.IsAnAndroidDevice() || 
                platform.IsAnAppleDevice())
            {
                ExpandedSet.Add(platform);
                continue;
            }
            
            //Needs Expansion
            var requestedVersions = platform.BrowserVersion.Split(SauceryConstants.PLATFORM_SEPARATOR);

            var rangeType = RangeClassifer.Classify(requestedVersions);
            if(rangeType == PlatformRange.Invalid)
            {
                continue;
            }

            var lowerBound = requestedVersions[0];
            var upperBound = requestedVersions[1];

            switch (rangeType)
            {
                case PlatformRange.NumericOnly:
                    AddNumericRange(platform, int.Parse(lowerBound), int.Parse(upperBound));
                    break;
                case PlatformRange.NonNumericOnly:
                    AddNonNumericRange(platform, lowerBound, upperBound);
                    break;
                case PlatformRange.NumericNonNumeric:
                    AddMixedRange(platform, int.Parse(lowerBound), upperBound);
                    break;
            }
        }

        return ExpandedSet;
    }

    private void AddMixedRange(SaucePlatform platform, int lowerBound, string upperBound)
    {
        var maxVersion = PlatformConfigurator.FindMaxBrowserVersion(platform);
        AddNumericRange(platform, lowerBound, maxVersion);
        AddNonNumericRange(platform, SauceryConstants.BROWSER_VERSION_LATEST_MINUS1, upperBound);
    }

    private void AddNonNumericRange(SaucePlatform platform, string lowerBound, string upperBound)
    {
        var lowerIndex = SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.IndexOf(lowerBound);
        var upperIndex = SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.IndexOf(upperBound);
        for (var i = lowerIndex; i <= upperIndex; i++)
        {
            AddPlatform(platform, SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.ElementAt(i));
        }
    }

    private void AddNumericRange(SaucePlatform platform, int lowerBound, int upperBound)
    {
        for (var version = lowerBound; version <= upperBound; version++)
        {
            if ((platform.Browser.Equals(SauceryConstants.BROWSER_CHROME) ||
                platform.Browser.Equals(SauceryConstants.BROWSER_EDGE)) &&
                version == SauceryConstants.MISSING_CHROMIUM_VERSION)
            {
                //No Chromium 82 due to Covid-19.
                //See https://wiki.saucelabs.com/pages/viewpage.action?pageId=102715396
                continue;
            }

            AddPlatform(platform, version.ToString());
        }
    }

    private void AddPlatform(SaucePlatform platform, string browserVersion) => 
        ExpandedSet.Add(new SaucePlatform
        {
            Os = platform.Os,
            Browser = platform.Browser,
            BrowserVersion = browserVersion,
            ScreenResolution = platform.ScreenResolution,
            Platform = platform.Platform,
            LongName = platform.LongName,
            LongVersion = platform.LongVersion,
            DeviceOrientation = platform.DeviceOrientation
        });
}