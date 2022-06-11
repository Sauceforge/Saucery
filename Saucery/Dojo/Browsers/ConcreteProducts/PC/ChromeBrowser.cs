using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteProducts.PC;

internal class ChromeBrowser : BrowserBase, IVersion
{
    public ChromeBrowser(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption) : base(sp, screenResolutions, platformNameForOption)
    {
    }

    public override BrowserVersion FindVersion(SupportedPlatform sp)
    {
        return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
    }

    public int MinimumVersion(SupportedPlatform sp)
    {
        return sp.os switch
        {
            SauceryConstants.PLATFORM_WINDOWS_11 or
            SauceryConstants.PLATFORM_WINDOWS_10 or
            SauceryConstants.PLATFORM_WINDOWS_81 or
            SauceryConstants.PLATFORM_WINDOWS_8 or
            SauceryConstants.PLATFORM_WINDOWS_7 or
            SauceryConstants.PLATFORM_MAC_12 or
            SauceryConstants.PLATFORM_MAC_11 or
            SauceryConstants.PLATFORM_MAC_1015 or
            SauceryConstants.PLATFORM_MAC_1014 or
            SauceryConstants.PLATFORM_MAC_1013 or
            SauceryConstants.PLATFORM_MAC_1012 or
            SauceryConstants.PLATFORM_MAC_1011 or
            SauceryConstants.PLATFORM_MAC_1010 => 75,
            _ => 0,
        };
    }

    public override bool IsSupportedVersion(SupportedPlatform sp)
    {
        return (sp.short_version_as_int != 0 && sp.short_version_as_int >= MinimumVersion(sp))
            || SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.Contains(sp.short_version);
    }
}