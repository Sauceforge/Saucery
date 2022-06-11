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

    public int MinimumVersion(SupportedPlatform sp)
    {
        return 78;
    }

    public override bool IsSupportedVersion(SupportedPlatform sp)
    {
        return (sp.short_version_as_int != 0 && sp.short_version_as_int >= MinimumVersion(sp))
            || SauceryConstants.BROWSER_VERSIONS_NONNUMERIC.Contains(sp.short_version);
    }
}