﻿using RestSharp;
using UnitTests.RestAPI.TestStatus.Base;

namespace UnitTests.RestAPI.TestStatus {
    public class SauceLabsStatusNotifier : StatusNotifier {
        public override void NotifyStatus(string jobId, bool isPassed) {
            var request = BuildRequest(string.Format(SauceryConstants.JOB_REQUEST, UserName, jobId), Method.PUT);
            request.AddParameter("Application/Json", "{\"passed\":" + "\"" + (isPassed ? "true" : "false") + "\"}", ParameterType.RequestBody);
            EnsureExecution(request);
            //Client.Execute(request);
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 10th August 2014
 * 
 */