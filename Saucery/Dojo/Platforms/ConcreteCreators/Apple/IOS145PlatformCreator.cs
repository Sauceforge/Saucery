using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Apple;

internal class IOS145PlatformCreator : PlatformCreator
{
    public IOS145PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create()
    {
        return new IOS145Platform(Platform);
    }
}