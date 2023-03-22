using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.Base;

internal interface IVersion
{
    int MinimumVersion(SupportedPlatform sp);
}