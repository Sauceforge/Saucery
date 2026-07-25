using RestSharp;
using RestSharp.Authenticators;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsStatusNotifier : RestBase {
    protected SauceLabsStatusNotifier(string restBase) {
        RestClientOptions clientOptions = new(restBase) {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    protected Task NotifyStatus(string fullRequestUrl, bool isPassed) 
        => NotifyStatus(fullRequestUrl, isPassed, CancellationToken.None);

    protected async Task NotifyStatus(string fullRequestUrl, bool isPassed, CancellationToken ct) {
        var request = BuildRequest(fullRequestUrl, Method.Put);

        var jobStatusObject = new {
            passed = isPassed
        };

        request.AddJsonBody(jobStatusObject);

        await EnsureExecutionAsync(request, ct)
            .ConfigureAwait(false);
    }
}