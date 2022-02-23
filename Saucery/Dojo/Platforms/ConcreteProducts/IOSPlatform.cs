using Saucery.Dojo.Platforms.Base;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.ConcreteProducts
{
    internal class IOSPlatform : PlatformBase
    {
        public IOSPlatform(SupportedPlatform sp) : base(sp)
        {
        }

        public override bool IsDesktopPlatform(SupportedPlatform sp)
        {
            return false;
        }
    }
}