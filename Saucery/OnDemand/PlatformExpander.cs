using Saucery.OnDemand.Base;
using Saucery.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saucery.OnDemand;

public class PlatformExpander
{
    private List<SaucePlatform> ExpandedSet { get; set; }
    private List<SaucePlatform> Platforms { get; set; }

    public PlatformExpander(List<SaucePlatform> platforms)
    {
        ExpandedSet = new();
        Platforms = platforms;
    }

    public List<SaucePlatform> Expand()
    {
        foreach (SaucePlatform platform in Platforms)
        {
            if (!platform.NeedsExpansion())
            {
                ExpandedSet.Add(platform);
                continue;
            }
            
            //Needs Expansion
            string[] requestedVersions = platform.BrowserVersion.Split(SauceryConstants.PLATFORM_SEPARATOR);

            PlatformRange rangeType = RangeClassifer.Classify(requestedVersions);
            if(rangeType == PlatformRange.Invalid)
            {
                continue;
            }

            string lowerBound = requestedVersions[0];
            string upperBound = requestedVersions[1];

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
                default:
                    break;
            };
        }

        return ExpandedSet;
    }

    private void AddMixedRange(SaucePlatform platform, int v, string upperBound)
    {
        throw new NotImplementedException();
    }

    private void AddNonNumericRange(SaucePlatform platform, string lowerBound, string upperBound)
    {
        int lowerIndex = SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.IndexOf(lowerBound);
        int upperIndex = SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.IndexOf(upperBound);
        for (int i = lowerIndex; i <= upperIndex; i++)
        {
            AddPlatform(platform, SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.ElementAt(i));
        }
    }

    private void AddNumericRange(SaucePlatform platform, int lowerBound, int upperBound)
    {
        for (int version = lowerBound; version <= upperBound; version++)
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

    private void AddPlatform(SaucePlatform platform, string browserVersion)
    {
        ExpandedSet.Add(new()
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
}