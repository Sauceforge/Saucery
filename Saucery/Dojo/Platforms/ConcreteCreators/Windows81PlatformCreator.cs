using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators
{
    internal class Windows81PlatformCreator : PlatformCreator
    {
        public Windows81PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Windows81Platform(Platform);
        }
    }
}