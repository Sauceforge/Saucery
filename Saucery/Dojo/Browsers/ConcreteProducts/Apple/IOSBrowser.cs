using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.ConcreteProducts.Apple;

internal class IOSBrowser : BrowserBase
{
    public IOSBrowser(SupportedPlatform sp, List<string> screenResolutions, string platformNameForOption) : base(sp, screenResolutions, platformNameForOption)
    {
    }

    public override BrowserVersion FindVersion(SupportedPlatform sp)
    {
        return BrowserVersions.Find(bv => bv.DeviceName.Equals(sp.long_name) && (bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version)));
    }

    public override bool IsSupportedVersion(SupportedPlatform sp)
    {
        return true;
    }
}