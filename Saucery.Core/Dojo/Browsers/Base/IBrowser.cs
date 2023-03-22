using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.Base;

public interface IBrowser
{
    BrowserVersion FindVersion(SupportedPlatform sp);
    bool IsSupportedVersion(SupportedPlatform sp);
}