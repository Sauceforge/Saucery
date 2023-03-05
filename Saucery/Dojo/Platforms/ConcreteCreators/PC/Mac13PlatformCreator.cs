using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.PC;

internal class Mac13PlatformCreator : PlatformCreator
{
    public Mac13PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create() => new Mac13Platform(Platform);
}