using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    internal class Mac11PlatformCreator : PlatformCreator
    {
        public Mac11PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Mac11Platform(Platform);
        }
    }
}