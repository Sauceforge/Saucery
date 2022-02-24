using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteProducts
{
    internal class SafariBrowser : BrowserBase
    {
        public SafariBrowser(SupportedPlatform sp) : base(sp)
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
                "Mac 12" => 15,
                "Mac 11" => 14,
                "Mac 10.15" => 13,
                "Mac 10.14" => 12,
                "Mac 10.13" => 13,
                _ => 0,
            };
        }

        public override int MinimumVersion(SupportedPlatform sp)
        {
            return sp.os switch
            {
                "Mac 12" => 15,
                "Mac 11" => 14,
                "Mac 10.15" => 13,
                "Mac 10.14" => 12,
                "Mac 10.13" => 12,
                _ => 0,
            };
        }

        internal override bool IsSupportedVersion(SupportedPlatform sp)
        {
            return sp.short_version_as_int != 0 &&
                   sp.short_version_as_int >= MinimumVersion(sp) &&
                   sp.short_version_as_int <= MaximumVersion(sp);
        }
    }
}