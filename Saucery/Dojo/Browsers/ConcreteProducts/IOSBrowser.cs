using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers
{
    internal class IOSBrowser : BrowserBase
    {
        public IOSBrowser(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserVersion FindVersion(SupportedPlatform sp)
        {
            return BrowserVersions.Find(bv => bv.DeviceName.Equals(sp.long_name) && (bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version)));
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
            return true;
        }
    }
}