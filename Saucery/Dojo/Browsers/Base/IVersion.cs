using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.Base
{
    interface IVersion
    {
        int MaximumVersion(SupportedPlatform sp);
        int MinimumVersion(SupportedPlatform sp);
    }
}