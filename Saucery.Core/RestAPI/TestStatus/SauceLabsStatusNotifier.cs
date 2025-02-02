using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsStatusNotifier : RestBase {
    public SauceLabsStatusNotifier() {
        RestClientOptions clientOptions = new(SauceryConstants.SAUCE_REST_BASE) {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public virtual void NotifyStatus(string jobId, bool isPassed) {
        var request = BuildRequest(string.Format(SauceryConstants.JOB_REQUEST, UserName, jobId), Method.Put);

        var jobStatusObject = new { passed = isPassed, status = isPassed ? "passed" : "failed" };
        request.AddJsonBody(jobStatusObject);

        EnsureExecution(request);
    }
}