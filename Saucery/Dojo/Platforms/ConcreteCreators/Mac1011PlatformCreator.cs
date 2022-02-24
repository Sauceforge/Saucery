using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    internal class Mac1011PlatformCreator : PlatformCreator
    {
        public Mac1011PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Mac1011Platform(Platform);
        }
    }
}