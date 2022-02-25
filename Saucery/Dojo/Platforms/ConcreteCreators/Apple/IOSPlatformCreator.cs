using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Apple {
    internal class IOSPlatformCreator : PlatformCreator
    {
        public IOSPlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new IOSPlatform(Platform);
        }
    }
}