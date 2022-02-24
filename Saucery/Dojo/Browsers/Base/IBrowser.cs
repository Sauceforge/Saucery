using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.Base
{
    public interface IBrowser
    {
        abstract BrowserVersion FindVersion(SupportedPlatform sp);
        abstract int MaximumVersion(SupportedPlatform sp);
        abstract int MinimumVersion(SupportedPlatform sp);
        abstract bool IsSupportedVersion(SupportedPlatform sp);
    }
}