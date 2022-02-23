using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    internal class Mac12PlatformCreator
    {
        private readonly SupportedPlatform Platform;

        public Mac12PlatformCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal PlatformBase Create()
        {
            return new Mac12Platform(Platform);
        }
    }
}