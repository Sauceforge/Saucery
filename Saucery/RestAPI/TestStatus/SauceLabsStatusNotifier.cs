﻿using RestSharp;
using RestSharp.Authenticators;
using Saucery.RestAPI.TestStatus.Base;
using Saucery.Util;

namespace Saucery.RestAPI.TestStatus;

public class SauceLabsStatusNotifier : StatusNotifier {
    public SauceLabsStatusNotifier()
    {
        Client = new RestClient(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };
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