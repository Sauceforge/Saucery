using System;
using System.Threading;
using RestSharp;
using UnitTests.RestAPI.FlowControl.Base;

namespace UnitTests.RestAPI.FlowControl {
    internal class SauceLabsFlowController : FlowController {
        public override void ControlFlow() {
            while(TooManyTests()) {
                Thread.Sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
            }
        }

        protected override bool TooManyTests() {
            //int maxParallelMacSessionsAllowed;  //Possible future use.
            var json = GetJsonResponseForUser(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST);
            var remainingSection = ExtractJsonSegment(json, json.IndexOf("\"remaining", StringComparison.Ordinal), json.Length - 3);
            var flowControl = SimpleJson.DeserializeObject<FlowControl>(remainingSection);
            return flowControl.remaining.overall <= 0;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */