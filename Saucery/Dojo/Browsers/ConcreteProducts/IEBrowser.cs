using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteProducts
{
    internal class IEBrowser : BrowserBase
    {
        public IEBrowser(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserVersion FindVersion(SupportedPlatform sp)
        {
            return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
        }

        public override int MaximumVersion(SupportedPlatform sp)
        {
            throw new System.NotImplementedException();
        }

        public override int MinimumVersion(SupportedPlatform sp)
        {
            throw new System.NotImplementedException();
        }

        internal override bool IsSupportedVersion(SupportedPlatform sp)
        {
            return sp.short_version_as_int != 0 &&
                   sp.short_version_as_int >= MinimumVersion(sp) &&
                   sp.short_version_as_int <= MaximumVersion(sp);
        }
    }
}