using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    internal class Mac1015PlatformCreator
    {
        private readonly SupportedPlatform Platform;

        public Mac1015PlatformCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal PlatformBase Create()
        {
            return new Mac1015Platform(Platform);
        }
    }
}