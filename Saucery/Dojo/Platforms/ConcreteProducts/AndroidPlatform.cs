using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    internal class AndroidPlatform : PlatformBase
    {
        public AndroidPlatform(SupportedPlatform sp) : base(sp)
        {
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return false;
        }
    }
}