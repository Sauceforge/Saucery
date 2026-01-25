using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.Util;

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

    public virtual async Task<List<SupportedPlatform>?> AcquirePlatforms(CancellationToken ct = default) {
        var json = await GetJsonResponseAsync(SauceryConstants.SUPPORTED_PLATFORMS_REQUEST, ct).ConfigureAwait(false);
        var supportedPlatforms = JsonSerializer.Deserialize<List<SupportedPlatform>>(json!, JsonOptions);

        return supportedPlatforms;
    }
}