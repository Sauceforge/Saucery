using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.RestAPI.SupportedPlatforms.Base;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.SupportedPlatforms;

public class SauceLabsRealDeviceAcquirer : RealDeviceAcquirer {
    public SauceLabsRealDeviceAcquirer()
    {
        RestClientOptions clientOptions = new(SauceryConstants.SAUCE_REAL_DEVICE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public virtual List<SupportedPlatform>? AcquireRealDevicePlatforms() {
        var json = GetJsonResponse(SauceryConstants.SUPPORTED_REALDEVICE_PLATFORMS_REQUEST);
        var supportedPlatforms = JsonSerializer.Deserialize<List<SupportedPlatform>>(json!, JsonOptions);
        
        return supportedPlatforms;
    }
}