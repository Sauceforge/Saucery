using Saucery.OnDemand.Base;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.OnDemand;

public class PlatformExpander
{
    public static List<SaucePlatform> Expand(List<SaucePlatform> platforms)
    {
        List<SaucePlatform> expandedSet = new();

        foreach (SaucePlatform platform in platforms)
        {
            if (platform.NeedsExpansion())
            {
                string[] requestedVersions = platform.BrowserVersion.Split(SauceryConstants.PLATFORM_SEPARATOR);
                if (!int.TryParse(requestedVersions[0], out int lowerBoundVersion) ||
                    !int.TryParse(requestedVersions[1], out int upperBoundVersion))
                {
                    expandedSet.Add(platform);
                    continue;
                }
                for (int version = lowerBoundVersion; version <= upperBoundVersion; version++)
                {
                    if((platform.Browser.Equals(SauceryConstants.BROWSER_CHROME) || 
                        platform.Browser.Equals(SauceryConstants.BROWSER_EDGE)) &&
                        version == SauceryConstants.MISSING_CHROMIUM_VERSION)
                    {
                        //No Chromium 82 due to Covid-19.
                        //See https://wiki.saucelabs.com/pages/viewpage.action?pageId=102715396
                        continue;
                    }

                    expandedSet.Add(new()
                    {
                        Os = platform.Os,
                        Browser = platform.Browser,
                        BrowserVersion = version.ToString(),
                        ScreenResolution = platform.ScreenResolution,
                        Platform = platform.Platform,
                        LongName = platform.LongName,
                        LongVersion = platform.LongVersion,
                        DeviceOrientation = platform.DeviceOrientation
                    });
                }
            }
            else
            {
                expandedSet.Add(platform);
            }
        }

        return expandedSet;
    }
}
