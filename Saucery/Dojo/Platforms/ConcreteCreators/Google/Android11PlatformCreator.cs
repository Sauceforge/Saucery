using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Google;

internal class Android11PlatformCreator : PlatformCreator
{
    public Android11PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create()
    {
        return new Android11Platform(Platform);
    }
}