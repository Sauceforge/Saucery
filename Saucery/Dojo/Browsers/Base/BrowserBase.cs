using Saucery.RestAPI;
using System.Collections.Generic;

namespace Saucery.Dojo.Browsers.Base
{
    public abstract class BrowserBase : IBrowser
    {
        internal readonly string Name;
        public string DeviceName;
        internal string PlatformName;
        
        internal string PlatformVersion;
        internal string RecommendedBackendVersion;
        
        internal List<BrowserVersion> BrowserVersions;

        public BrowserBase(SupportedPlatform sp)
        {
            Name = sp.api_name;
            DeviceName = sp.long_name;
            PlatformName = sp.os;
            
            if (sp.IsMobilePlatform())
            {
                PlatformVersion = sp.short_version;
                RecommendedBackendVersion = sp.recommended_backend_version;
            }
            BrowserVersions = new List<BrowserVersion>();
        }

        public abstract BrowserVersion FindVersion(SupportedPlatform sp);
        public abstract bool IsSupportedVersion(SupportedPlatform sp);
        public abstract int MaximumVersion(SupportedPlatform sp);
        public abstract int MinimumVersion(SupportedPlatform sp);
    }
}