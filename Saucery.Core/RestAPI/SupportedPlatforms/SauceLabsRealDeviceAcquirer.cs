using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.Util;
using System.Text.Json;

namespace Saucery.Core.RestAPI.SupportedPlatforms;

public class SauceLabsRealDeviceAcquirer : RestBase {
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