using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.RestAPI.SupportedPlatforms.Base;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.SupportedPlatforms;

public class SauceLabsPlatformAcquirer : PlatformAcquirer {
    public SauceLabsPlatformAcquirer()
    {
        Client = new RestClient(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };
    }
    public override List<SupportedPlatform> AcquirePlatforms() {
        var json = GetJsonResponse(SauceryConstants.SUPPORTED_PLATFORMS_REQUEST);
        var supportedPlatforms = JsonConvert.DeserializeObject<List<SupportedPlatform>>(json);
        return supportedPlatforms;
    }
}