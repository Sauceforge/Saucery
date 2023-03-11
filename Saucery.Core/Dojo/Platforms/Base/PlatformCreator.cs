using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.Base;

public abstract class PlatformCreator
{
    internal readonly SupportedPlatform Platform;

    public PlatformCreator(SupportedPlatform sp)
    {
        Platform = sp;
    }

    public abstract PlatformBase Create();
}
