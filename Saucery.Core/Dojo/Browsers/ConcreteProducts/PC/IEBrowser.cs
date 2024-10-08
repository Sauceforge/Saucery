﻿using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Browsers.ConcreteProducts.PC;

internal class IEBrowser(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption) : BrowserBase(sp, screenResolutions, platformNameForOption), IVersion
{
    public override BrowserVersion? FindVersion(SupportedPlatform sp) => 
        BrowserVersions
            .Find(bv => bv.Name!.Equals(sp.latest_stable_version) || 
                  bv.Name.Equals(sp.short_version));

    public int MinimumVersion(SupportedPlatform sp) => sp.Os switch
    {
        SauceryConstants.PLATFORM_WINDOWS_10 => 11,
        SauceryConstants.PLATFORM_WINDOWS_81 => 11,
        SauceryConstants.PLATFORM_WINDOWS_8 => 10,
        SauceryConstants.PLATFORM_WINDOWS_7 => 10,
        _ => 0,
    };

    public override bool IsSupportedVersion(SupportedPlatform sp) => 
        (sp.short_version_as_int != 0 && 
         sp.short_version_as_int >= MinimumVersion(sp)) || 
         SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.Contains(sp.short_version!);
}