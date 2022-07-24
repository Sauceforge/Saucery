using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using Saucery.RestAPI.SupportedPlatforms.Base;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.RestAPI.SupportedPlatforms;

public class SauceLabsRealDeviceAcquirer : RealDeviceAcquirer {
    public SauceLabsRealDeviceAcquirer()
    {
        Client = new RestClient(SauceryConstants.SAUCE_REAL_DEVICE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };
    }

    public override List<SupportedRealDevicePlatform> AcquireRealDevicePlatforms()
    {
        var json = GetJsonResponse(SauceryConstants.SUPPORTED_REALDEVICE_PLATFORMS_REQUEST);
        var supportedRealDevicePlatforms = JsonConvert.DeserializeObject<List<SupportedRealDevicePlatform>>(json);
        return supportedRealDevicePlatforms;

        //return new List<SupportedRealDevicePlatform>();
    }
}