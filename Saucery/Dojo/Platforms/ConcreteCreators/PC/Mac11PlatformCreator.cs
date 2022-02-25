using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.PC
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