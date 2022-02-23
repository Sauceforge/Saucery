using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.Base
{
    public abstract class BrowserCreator
    {
        public abstract BrowserBase Create(SupportedPlatform sp);
    }
}
