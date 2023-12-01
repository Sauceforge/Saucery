using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.ConcreteCreators.PC;

internal class Mac12PlatformCreator(SupportedPlatform sp) : PlatformCreator(sp)
{
    public override PlatformBase Create() => new Mac12Platform(Platform);
}