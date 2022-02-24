using Saucery.RestAPI;

namespace Saucery.Dojo
{
    public class BrowserVersion
    {
        public string Name;
        public string DeviceName;
        public BrowserVersion(SupportedPlatform sp)
        {
            Name = sp.latest_stable_version != string.Empty ? sp.latest_stable_version : sp.short_version;
            DeviceName = sp.long_name;
        }
    }
}