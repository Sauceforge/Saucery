using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteProducts.PC;

internal class SafariBrowser : BrowserBase, IVersion
{
    public SafariBrowser(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption) : base(sp, screenResolutions, platformNameForOption)
    {
    }

    public override BrowserVersion FindVersion(SupportedPlatform sp)
    {
        return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
    }

    //public int MaximumVersion(SupportedPlatform sp)
    //{
    //    return sp.os switch
    //    {
    //        SauceryConstants.PLATFORM_MAC_12 => 15,
    //        SauceryConstants.PLATFORM_MAC_11 => 14,
    //        SauceryConstants.PLATFORM_MAC_1015 => 13,
    //        SauceryConstants.PLATFORM_MAC_1014 => 12,
    //        SauceryConstants.PLATFORM_MAC_1013 => 13,
    //        _ => 0,
    //    };
    //}

    public int MinimumVersion(SupportedPlatform sp)
    {
        return sp.os switch
        {
            SauceryConstants.PLATFORM_MAC_12 => 15,
            SauceryConstants.PLATFORM_MAC_11 => 14,
            SauceryConstants.PLATFORM_MAC_1015 => 13,
            SauceryConstants.PLATFORM_MAC_1014 => 12,
            SauceryConstants.PLATFORM_MAC_1013 => 12,
            _ => 0,
        };
    }

    public override bool IsSupportedVersion(SupportedPlatform sp)
    {
        return sp.short_version_as_int != 0 && sp.short_version_as_int >= MinimumVersion(sp); //&& sp.short_version_as_int <= MaximumVersion(sp);
    }
}