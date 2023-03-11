using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.Base;

interface IVersion
{
    int MinimumVersion(SupportedPlatform sp);
}