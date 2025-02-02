using RestSharp;
using RestSharp.Authenticators;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsStatusNotifier : RestBase {
    protected SauceLabsStatusNotifier(string restBase)
    {
        RestClientOptions clientOptions = new(restBase) {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    protected void NotifyStatus(string fullRequestUrl, bool isPassed) {
        var request = BuildRequest(fullRequestUrl, Method.Put);

        var jobStatusObject = new { passed = isPassed };
        request.AddJsonBody(jobStatusObject);

        EnsureExecution(request);
    }

}