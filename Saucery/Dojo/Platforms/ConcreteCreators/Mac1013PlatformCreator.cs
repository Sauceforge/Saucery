using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    internal class Mac1013PlatformCreator
    {
        private readonly SupportedPlatform Platform;

        public Mac1013PlatformCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal PlatformBase Create()
        {
            return new Mac1013Platform(Platform);
        }
    }
}