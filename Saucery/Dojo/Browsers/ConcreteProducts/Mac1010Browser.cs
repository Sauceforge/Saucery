using Saucery.Dojo.Browsers.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.ConcreteProducts
{
    //OS X Yosemite
    public class Mac1010Browser : BrowserBase
    {
        public Mac1010Browser(SupportedPlatform sp) : base(sp)
        {
        }

        public override BrowserVersion FindVersion(SupportedPlatform sp)
        {
            return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
        }

        public override int MaximumVersion(SupportedPlatform sp)
        {
            return Name switch
            {
                "chrome" => 87,
                "MicrosoftEdge" => 81,
                _ => 0,
            };
        }

        public override int MinimumVersion(SupportedPlatform sp)
        {
            return Name switch
            {
                "chrome" => 75,
                "MicrosoftEdge" => 79,
                _ => 0,
            };
        }

        internal override bool IsSupportedVersion(SupportedPlatform sp) {
            return sp.short_version_as_int != 0 && 
                   sp.short_version_as_int >= MinimumVersion(sp) && 
                   sp.short_version_as_int <= MaximumVersion(sp);
        }
    }
}
