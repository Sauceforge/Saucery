using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.Base
{
    public interface IBrowser
    {
        abstract BrowserVersion FindVersion(SupportedPlatform sp);
        abstract bool IsSupportedVersion(SupportedPlatform sp);
    }
}