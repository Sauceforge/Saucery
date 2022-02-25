using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.PC
{
    internal class Mac1012PlatformCreator : PlatformCreator
    {
        public Mac1012PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Mac1013Platform(Platform);
        }
    }
}