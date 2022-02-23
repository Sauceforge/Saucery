using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators
{
    internal class Windows10PlatformCreator
    {
        private readonly SupportedPlatform Platform;

        public Windows10PlatformCreator(SupportedPlatform platform)
        {
            Platform = platform;
        }

        internal PlatformBase Create()
        {
            return new Windows10Platform(Platform);
        }
    }
}