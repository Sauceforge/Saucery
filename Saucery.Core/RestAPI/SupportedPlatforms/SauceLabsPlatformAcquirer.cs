using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.Util;
using System.Text.Json;

namespace Saucery.Core.RestAPI.SupportedPlatforms;

public class SauceLabsPlatformAcquirer : RestBase {
    public SauceLabsPlatformAcquirer()
    {
        RestClientOptions clientOptions = new(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public virtual List<SupportedPlatform>? AcquirePlatforms() {
        var json = GetJsonResponse(SauceryConstants.SUPPORTED_PLATFORMS_REQUEST);
        var supportedPlatforms = JsonSerializer.Deserialize<List<SupportedPlatform>>(json!, JsonOptions);

        return supportedPlatforms;
    }
}