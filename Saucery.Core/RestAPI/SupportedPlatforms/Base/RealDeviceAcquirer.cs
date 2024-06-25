namespace Saucery.Core.RestAPI.SupportedPlatforms.Base;

public abstract class RealDeviceAcquirer : RestBase {
    public abstract List<SupportedPlatform>? AcquireRealDevicePlatforms();
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 21st February 2022
* 
*/