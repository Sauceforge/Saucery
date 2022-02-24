using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators
{
    internal class Windows7PlatformCreator : PlatformCreator
    {
        public Windows7PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Windows7Platform(Platform);
        }
    }
}