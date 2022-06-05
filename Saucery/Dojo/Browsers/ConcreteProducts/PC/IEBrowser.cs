using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteProducts.PC;

internal class IEBrowser : BrowserBase, IVersion
{
    public IEBrowser(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption) : base(sp, screenResolutions, platformNameForOption)
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
    //        SauceryConstants.PLATFORM_WINDOWS_10 => 11,
    //        SauceryConstants.PLATFORM_WINDOWS_81 => 11,
    //        SauceryConstants.PLATFORM_WINDOWS_8 => 10,
    //        SauceryConstants.PLATFORM_WINDOWS_7 => 11,
    //        _ => 0,
    //    };
    //}

    public int MinimumVersion(SupportedPlatform sp)
    {
        return sp.os switch
        {
            SauceryConstants.PLATFORM_WINDOWS_10 => 11,
            SauceryConstants.PLATFORM_WINDOWS_81 => 11,
            SauceryConstants.PLATFORM_WINDOWS_8 => 10,
            SauceryConstants.PLATFORM_WINDOWS_7 => 10,
            _ => 0,
        };
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