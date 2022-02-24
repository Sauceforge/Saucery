using Saucery.RestAPI;

namespace Saucery.Dojo.Browsers.Base
{
    public abstract class BrowserCreator
    {
        internal readonly SupportedPlatform Platform;

        public BrowserCreator(SupportedPlatform sp)
        {
            Platform = sp;
        }

        public abstract BrowserBase Create();
    }
}
