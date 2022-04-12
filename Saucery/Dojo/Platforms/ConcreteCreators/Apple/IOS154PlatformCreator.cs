using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Apple {
    internal class IOS154PlatformCreator : PlatformCreator
    {
        public IOS154PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new IOS154Platform(Platform);
        }
    }
}