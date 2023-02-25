using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Google;

internal class Android7PlatformCreator : PlatformCreator
{
    public Android7PlatformCreator(SupportedPlatform sp) : base(sp)
    {
    }

    public override PlatformBase Create() => new Android7Platform(Platform);
}