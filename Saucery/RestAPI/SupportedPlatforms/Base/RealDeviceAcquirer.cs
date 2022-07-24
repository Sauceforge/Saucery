using System.Collections.Generic;

namespace Saucery.RestAPI.SupportedPlatforms.Base;

public abstract class RealDeviceAcquirer : RestBase {
    public abstract List<SupportedRealDevicePlatform> AcquireRealDevicePlatforms();
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 21st February 2022
* 
*/