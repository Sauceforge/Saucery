using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.PC;

internal class Windows8PlatformCreator : PlatformCreator
{
    public Windows8PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create() => new Windows8Platform(Platform);
}