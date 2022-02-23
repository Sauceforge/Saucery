using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.Base
{
    public abstract class BrowserBase : IBrowser
    {
        internal readonly string Name;
        internal string ShortVersion;
        internal string RecommendedBackendVersion;
        internal string DeviceName;
        internal List<BrowserVersion> BrowserVersions;

        public BrowserBase(SupportedPlatform sp)
        {
            Name = sp.api_name;
            //ShortVersion = sp.short_version;
            //RecommendedBackendVersion = sp.recommended_backend_version;
            if (sp.IsMobilePlatform())
            {
                DeviceName = sp.long_name;
                ShortVersion = sp.short_version;
                RecommendedBackendVersion = sp.recommended_backend_version;
            }
            BrowserVersions = new List<BrowserVersion>();
        }

        public abstract BrowserVersion FindVersion(SupportedPlatform sp);
        //{
        //    return BrowserVersions.Find(bv => bv.Name.Equals(sp.latest_stable_version) || bv.Name.Equals(sp.short_version));
        //}

        public abstract int MaximumVersion(SupportedPlatform sp);
        public abstract int MinimumVersion(SupportedPlatform sp);
        internal abstract bool IsSupportedVersion(SupportedPlatform sp);
        
    }
}
