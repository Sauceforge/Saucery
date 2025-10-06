using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteCreators.Google;
using Saucery.Core.RestAPI;

namespace Saucery.Core.Dojo.Platforms;

public static class AndroidPlatformFactory
{
    public static PlatformBase? CreatePlatform(SupportedPlatform sp) => sp.short_version switch
    {
        "16.0" => new Android16PlatformCreator(sp).Create(),
        "15.0" => new Android15PlatformCreator(sp).Create(),
        "14.0" => new Android14PlatformCreator(sp).Create(),
        "13.0" => new Android13PlatformCreator(sp).Create(),
        "12.0" => new Android12PlatformCreator(sp).Create(),
        "11.0" => new Android11PlatformCreator(sp).Create(),
        "10.0" => new Android10PlatformCreator(sp).Create(),
        "9.0" => new Android9PlatformCreator(sp).Create(),
        "8.1" => new Android81PlatformCreator(sp).Create(),
        "8.0" => new Android8PlatformCreator(sp).Create(),
        "7.1" => new Android71PlatformCreator(sp).Create(),
        "7.0" => new Android7PlatformCreator(sp).Create(),
        "6.0" => new Android6PlatformCreator(sp).Create(),
        "5.1" => new Android51PlatformCreator(sp).Create(),
        _ => null
    };

    public static PlatformBase? CreateRealPlatform(SupportedPlatform sp) => sp.OsVersion?.Split(".")[0] switch {
        "16" => new Android16PlatformCreator(sp).Create(),
        "15" => new Android15PlatformCreator(sp).Create(),
        "14" => new Android14PlatformCreator(sp).Create(),
        "13" => new Android13PlatformCreator(sp).Create(),
        "12" => new Android12PlatformCreator(sp).Create(),
        "11" => new Android11PlatformCreator(sp).Create(),
        "10" => new Android10PlatformCreator(sp).Create(),
        "9" => new Android9PlatformCreator(sp).Create(),
        _ => null
    };
}
