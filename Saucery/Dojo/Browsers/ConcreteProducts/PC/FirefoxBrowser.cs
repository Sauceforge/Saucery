using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteProducts.PC;

internal class FirefoxBrowser : BrowserBase, IVersion
{
    public FirefoxBrowser(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption) : base(sp, screenResolutions, platformNameForOption)
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
    //        SauceryConstants.PLATFORM_WINDOWS_11 or
    //        SauceryConstants.PLATFORM_WINDOWS_10 or
    //        SauceryConstants.PLATFORM_WINDOWS_81 or
    //        SauceryConstants.PLATFORM_WINDOWS_8 or
    //        SauceryConstants.PLATFORM_WINDOWS_7 or
    //        SauceryConstants.PLATFORM_MAC_12 or
    //        SauceryConstants.PLATFORM_MAC_11 or
    //        SauceryConstants.PLATFORM_MAC_1015 or
    //        SauceryConstants.PLATFORM_MAC_1014 or
    //        SauceryConstants.PLATFORM_MAC_1013 or
    //        SauceryConstants.PLATFORM_MAC_1012 => 98,
    //        SauceryConstants.PLATFORM_MAC_1011 => 78,
    //        _ => 0,
    //    };
    //}

    public int MinimumVersion(SupportedPlatform sp)
    {
        return 78;
    }

    public override bool IsSupportedVersion(SupportedPlatform sp)
    {
        return (sp.short_version_as_int != 0 && sp.short_version_as_int >= MinimumVersion(sp)) //&& sp.short_version_as_int <= MaximumVersion(sp))
            || sp.short_version.Equals("beta")
            || sp.short_version.Equals("dev")
            || sp.short_version.Equals("latest")
            || sp.short_version.Equals("latest-1");
    }
}