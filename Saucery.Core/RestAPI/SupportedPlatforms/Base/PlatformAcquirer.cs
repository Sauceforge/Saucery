namespace Saucery.Core.RestAPI.SupportedPlatforms.Base;

public abstract class PlatformAcquirer : RestBase {
    public abstract List<SupportedPlatform>? AcquirePlatforms();
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 21st February 2022
* 
*/