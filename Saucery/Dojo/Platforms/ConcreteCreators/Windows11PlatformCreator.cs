using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    internal class Windows11PlatformCreator
    {
        private readonly SupportedPlatform Platform;

        public Windows11PlatformCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal PlatformBase Create()
        {
            return new Windows11Platform(Platform);
        }
    }
}