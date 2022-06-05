using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms.Base;

public abstract class PlatformCreator
{
    internal readonly SupportedPlatform Platform;

    public PlatformCreator(SupportedPlatform sp)
    {
        Platform = sp;
    }

    public abstract PlatformBase Create();
}
