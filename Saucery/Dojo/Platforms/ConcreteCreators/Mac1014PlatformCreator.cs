using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    internal class Mac1014PlatformCreator : PlatformCreator
    {
        public Mac1014PlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new Mac1014Platform(Platform);
        }
    }
}