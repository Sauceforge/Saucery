using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Apple; 

internal class IOS15PlatformCreator : PlatformCreator
{
    public IOS15PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create()
    {
        return new IOS15Platform(Platform);
    }
}