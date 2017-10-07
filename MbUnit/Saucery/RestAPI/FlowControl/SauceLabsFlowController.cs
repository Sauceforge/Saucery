using System.Threading;
using RestSharp;
using Saucery.RestAPI.FlowControl.Base;
using Saucery.Util;

namespace Saucery.RestAPI.FlowControl {
    internal class SauceLabsFlowController : FlowController {
        public override void ControlFlow() {
            while(TooManyTests()) {
                Thread.Sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
            }
        }

        protected override bool TooManyTests() {
            //int maxParallelMacSessionsAllowed;  //Possible future use.
            var request = BuildRequest(string.Format(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST, UserName), Method.GET);
            var flowControl = Client.Execute<FlowControl>(request).Data;
            return flowControl.concurrency.jenkinsvacc.remaining.overall <= 0;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */