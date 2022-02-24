using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators
{
    internal class Windows8PlatformCreator : PlatformCreator
    {
        public Windows8PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Windows8Platform(Platform);
        }
    }
}