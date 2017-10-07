using System;
using System.Threading;
using RestSharp;
using Saucery2.RestAPI.FlowControl.Base;
using Saucery2.Util;

namespace Saucery2.RestAPI.FlowControl {
    internal class SauceLabsFlowController : FlowController {
        public override void ControlFlow() {
            while(TooManyTests()) {
                Thread.Sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
            }
        }

        protected override bool TooManyTests() {
            //int maxParallelMacSessionsAllowed;  //Possible future use.
            var json = GetJsonResponseForUser(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST);
            //Console.WriteLine("Concurrency JSON: " + json);

            var jsonStartIndex = json.IndexOf("\"remaining", StringComparison.Ordinal);

            var jsonEndIndex = json.Length - 3;
            var remainingSection = ExtractJsonSegment(json, jsonStartIndex, jsonEndIndex);
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