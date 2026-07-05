using Saucery.Core.Dojo.Browsers.Base;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.ConcreteProducts.Google;

internal class AndroidBrowser(
    SupportedPlatform sp, 
    string platformNameForOption,
    bool isArmRequired = false) : BrowserBase(sp, null!, platformNameForOption, isArmRequired)
{
    public override BrowserVersion? FindVersion(SupportedPlatform sp) => 
        BrowserVersions
        .Find(bv => 
              bv.DeviceName.Equals(sp.long_name) &&
             (bv.Name!.Equals(sp.latest_stable_version) || 
              bv.Name.Equals(sp.short_version)));

    public override bool IsSupportedVersion(SupportedPlatform sp) => true;
}