using System;
using System.Threading;
using RestSharp;
using UnitTests.RestAPI.FlowControl.Base;

namespace UnitTests.RestAPI.FlowControl {
    internal class SauceLabsFlowController : FlowController {
        public override void ControlFlow() {
            //Console.WriteLine(@"Debug: In Control Flow");
            while(TooManyTests()) {
                Thread.Sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
            }
        }

        protected override bool TooManyTests() {
            //Console.WriteLine(@"Debug: In TooManyTests");
            //int maxParallelMacSessionsAllowed;  //Possible future use.
            var json = GetJsonResponseForUser(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST);
            //Console.WriteLine(@"Debug: {0}", json);
            var remainingSection = ExtractJsonSegment(json, json.IndexOf("\"remaining", StringComparison.Ordinal), json.Length - 3);
            //Console.WriteLine(@"Debug: remainingsection = {0}", remainingSection);
            var flowControl = SimpleJson.DeserializeObject<FlowControl>(remainingSection);
            //Console.WriteLine(@"Debug: overall = {0}", flowControl.remaining.overall);
            return flowControl.remaining.overall <= 0;
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */