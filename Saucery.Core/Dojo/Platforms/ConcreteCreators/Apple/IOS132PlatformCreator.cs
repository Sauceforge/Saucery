using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.ConcreteCreators.Apple; 

internal class IOS132PlatformCreator(SupportedPlatform sp) : PlatformCreator(sp)
{
    public override PlatformBase Create() => new IOS132Platform(Platform);
}