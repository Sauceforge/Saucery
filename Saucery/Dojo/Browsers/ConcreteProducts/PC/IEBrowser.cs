using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteProducts.PC
{
    internal class IEBrowser : BrowserBase, IVersion
    {
        public IEBrowser(SupportedPlatform sp, string platformNameForOption) : base(sp, platformNameForOption)
        {
        }

        public override BrowserVersion FindVersion(SupportedPlatform sp)
        {
            return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
        }

        public int MaximumVersion(SupportedPlatform sp)
        {
            return sp.os switch
            {
                "Windows 2008" => 11,
                "Windows 2012" => 10,
                "Windows 2012 R2" => 11,
                "Windows 10" => 11,
                _ => 0,
            };
        }

        public int MinimumVersion(SupportedPlatform sp)
        {
            return sp.os switch
            {
                "Windows 2008" => 10,
                "Windows 2012" => 10,
                "Windows 2012 R2" => 11,
                "Windows 10" => 11,
                _ => 0,
            };
        }

        public override bool IsSupportedVersion(SupportedPlatform sp)
        {
            return (sp.short_version_as_int != 0 && sp.short_version_as_int >= MinimumVersion(sp) && sp.short_version_as_int <= MaximumVersion(sp))
                || sp.short_version.Equals("beta")
                || sp.short_version.Equals("dev")
                || sp.short_version.Equals("latest")
                || sp.short_version.Equals("latest-1");
        }
    }
}