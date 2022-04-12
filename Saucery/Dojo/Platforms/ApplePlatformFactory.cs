using Saucery.Dojo.Platforms.Base;
using Saucery.Dojo.Platforms.ConcreteCreators.Apple;
using Saucery.RestAPI;

namespace Saucery.Dojo.Platforms
{
    public class ApplePlatformFactory
    {
        public static PlatformBase CreatePlatform(SupportedPlatform sp)
        {
            return sp.short_version switch
            {
                "15.4" => new IOS154PlatformCreator(sp).Create(),
                "15.2" => new IOS152PlatformCreator(sp).Create(),
                "15.0" => new IOS15PlatformCreator(sp).Create(),
                "14.5" => new IOS145PlatformCreator(sp).Create(),
                "14.4" => new IOS144PlatformCreator(sp).Create(),
                "14.3" => new IOS143PlatformCreator(sp).Create(),
                "14.0" => new IOS14PlatformCreator(sp).Create(),
                "13.4" => new IOS134PlatformCreator(sp).Create(),
                "13.2" => new IOS132PlatformCreator(sp).Create(),
                "13.0" => new IOS13PlatformCreator(sp).Create(),
                "12.4" => new IOS124PlatformCreator(sp).Create(),
                "12.2" => new IOS122PlatformCreator(sp).Create(),
                "12.0" => new IOS12PlatformCreator(sp).Create(),
                "11.3" => new IOS113PlatformCreator(sp).Create(),
                "11.2" => new IOS112PlatformCreator(sp).Create(),
                "11.1" => new IOS111PlatformCreator(sp).Create(),
                "11.0" => new IOS11PlatformCreator(sp).Create(),
                "10.3" => new IOS103PlatformCreator(sp).Create(),
                _ => null
            };
        }
    }
}
