namespace Saucery.OnDemand;

public enum PlatformType
{
    Chrome = 1,
    Firefox = 2,
    Safari = 3,
    Edge = 4,
    IE = 5,
    Apple = 6,
    Android = 7
}

public static class Extensions
{
    public static bool IsMobile(this PlatformType type)
    {
        return (int)type == (int)PlatformType.Android || (int)type == (int)PlatformType.Apple;
    }
}