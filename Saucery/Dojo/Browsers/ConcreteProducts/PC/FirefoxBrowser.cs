using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteProducts.PC
{
    internal class FirefoxBrowser : BrowserBase
    {
        public FirefoxBrowser(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserVersion FindVersion(SupportedPlatform sp)
        {
            return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
        }

        public override int MaximumVersion(SupportedPlatform sp)
        {
            return sp.os switch
            {
                "Windows 2008" or "Windows 2012" or "Windows 2012 R2" or "Windows 10" or "Windows 11" or "Mac 12" or "Mac 11" or "Mac 10.15" or "Mac 10.14" or "Mac 10.13" or "Mac 10.12" => 97,
                "Mac 10.11" => 78,
                _ => 0,
            };
        }

        public override int MinimumVersion(SupportedPlatform sp)
        {
            return 78;
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