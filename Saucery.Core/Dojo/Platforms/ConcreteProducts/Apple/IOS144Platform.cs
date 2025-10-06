using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI;
using Saucery.Core.Util;

namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;

public sealed class IOS144Platform(SupportedPlatform sp) : PlatformBase(sp, SauceryConstants.PLATFORM_IOS, [])
{
}