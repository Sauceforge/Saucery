using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Apple; 

internal class IOS13PlatformCreator : PlatformCreator
{
    public IOS13PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create() => new IOS13Platform(Platform);
}