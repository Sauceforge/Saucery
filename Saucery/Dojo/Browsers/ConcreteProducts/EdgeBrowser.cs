using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteProducts
{
    internal class EdgeBrowser : BrowserBase
    {
        private SupportedPlatform Platform;

        public EdgeBrowser(SupportedPlatform sp) : base(sp)
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
                "Windows 10" or "Windows 11" or "Mac 12" or "Mac 11" or "Mac 10.15" or "Mac 10.14" or "Mac 10.13" or "Mac 10.12" => 98,
                "Mac 10.11" or "Mac 10.10" => 81,
                _ => 0,
            };
        }

        public override int MinimumVersion(SupportedPlatform sp)
        {
            return 79;
        }

        internal override bool IsSupportedVersion(SupportedPlatform sp)
        {
            return (sp.short_version_as_int != 0 &&
                   sp.short_version_as_int >= MinimumVersion(sp) &&
                   sp.short_version_as_int <= MaximumVersion(sp)) ||
                   sp.short_version.Equals("beta") ||
                   sp.short_version.Equals("dev") ||
                   sp.short_version.Equals("latest") ||
                   sp.short_version.Equals("latest-1");
        }
    }
}