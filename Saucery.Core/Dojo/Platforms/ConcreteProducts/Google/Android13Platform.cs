using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;

public sealed class Android13Platform(SupportedPlatform sp) : PlatformBase(sp, SauceryConstants.ANDROID, [])
{
}