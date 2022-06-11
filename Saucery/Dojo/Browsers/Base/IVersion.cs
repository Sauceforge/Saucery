using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.Base;

interface IVersion
{
    int MinimumVersion(SupportedPlatform sp);
}