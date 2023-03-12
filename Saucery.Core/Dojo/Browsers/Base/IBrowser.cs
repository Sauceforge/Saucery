using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Browsers.Base;

public interface IBrowser
{
    abstract BrowserVersion FindVersion(SupportedPlatform sp);
    abstract bool IsSupportedVersion(SupportedPlatform sp);
}