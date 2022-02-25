using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteCreators.Google
{
    internal class AndroidPlatformCreator : PlatformCreator
    {
        public AndroidPlatformCreator(SupportedPlatform sp) : base(sp)
        {
        }

        public override PlatformBase Create()
        {
            return new AndroidPlatform(Platform);
        }
    }
}