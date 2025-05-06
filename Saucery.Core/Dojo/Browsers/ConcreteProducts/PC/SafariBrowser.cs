using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Browsers.ConcreteProducts.PC;

internal class SafariBrowser(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption) : BrowserBase(sp, screenResolutions, platformNameForOption), IVersion
{
    public override BrowserVersion? FindVersion(SupportedPlatform sp) => 
        BrowserVersions
            .Find(bv => bv.Name!.Equals(sp.latest_stable_version) || 
                  bv.Name.Equals(sp.short_version));

    public int MinimumVersion(SupportedPlatform sp) => sp.Os switch
    {
        SauceryConstants.PLATFORM_MAC_14 => 18,
        SauceryConstants.PLATFORM_MAC_13 => 16,
        SauceryConstants.PLATFORM_MAC_12 => 15,
        SauceryConstants.PLATFORM_MAC_11 => 14,
        
        _ => 0,
    };

    public override bool IsSupportedVersion(SupportedPlatform sp) => 
        sp.short_version_as_int != 0 && 
        sp.short_version_as_int >= MinimumVersion(sp);
}