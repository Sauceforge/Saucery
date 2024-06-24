using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms.Base;

public abstract class PlatformCreator(SupportedPlatform sp) {
    internal readonly SupportedPlatform Platform = sp;

    public abstract PlatformBase Create();
}
