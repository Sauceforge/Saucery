using Saucery.RestAPI;

namespace Saucery.Dojo
{
    public class BrowserVersion
    {
        public string Name;
        public BrowserVersion(SupportedPlatform sp)
        {
            Name = sp.latest_stable_version != "" ? sp.latest_stable_version : sp.short_version;
        }
    }
}