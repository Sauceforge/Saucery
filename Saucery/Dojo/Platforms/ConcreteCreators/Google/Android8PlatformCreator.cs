using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Google;

internal class Android8PlatformCreator : PlatformCreator
{
    public Android8PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create()
    {
        return new Android8Platform(Platform);
    }
}