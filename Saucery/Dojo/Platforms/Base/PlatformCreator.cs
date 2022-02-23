using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.Base
{
    public abstract class PlatformCreator
    {
        public abstract PlatformBase Create(SupportedPlatform sp);
    }
}
