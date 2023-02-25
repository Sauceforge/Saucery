using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Google;

internal class Android9PlatformCreator : PlatformCreator
{
    public Android9PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create() => new Android9Platform(Platform);
}