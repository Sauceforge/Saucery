using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.Base
{
    public interface IBrowser
    {
        int MaximumVersion(SupportedPlatform sp);
        int MinimumVersion(SupportedPlatform sp);
    }
}