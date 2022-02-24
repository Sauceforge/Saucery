using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators
{
    internal class Windows10PlatformCreator : PlatformCreator
    {
        public Windows10PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Windows10Platform(Platform);
        }
    }
}