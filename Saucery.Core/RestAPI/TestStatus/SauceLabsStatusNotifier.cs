using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.RestAPI.TestStatus.Base;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.TestStatus;

public class SauceLabsStatusNotifier : StatusNotifier {
    public SauceLabsStatusNotifier()
    {
        RestClientOptions clientOptions = new(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public override void NotifyStatus(string jobId, bool isPassed) {
        var request = BuildRequest(string.Format(SauceryConstants.JOB_REQUEST, UserName, jobId), Method.Put);

        var jobStatusObject = new { passed = true };
        request.AddJsonBody(jobStatusObject);

        EnsureExecution(request);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 10th August 2014
* 
*/