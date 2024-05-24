using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.RestAPI.FlowControl.Base;
using Saucery.Core.Util;
using System.Text.Json;

namespace Saucery.Core.RestAPI.FlowControl;

public class SauceLabsFlowController : FlowController {
    public SauceLabsFlowController()
    {
        RestClientOptions clientOptions = new(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuth(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public override void ControlFlow() {
        while(TooManyTests()) {
            Thread.Sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
        }
    }

    protected override bool TooManyTests() {
        //int maxParallelMacSessionsAllowed;  //Possible future use.
        var json = GetJsonResponse(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST);

        //Console.WriteLine(@"Debug: {0}", json);
        var remainingSection = ExtractJsonSegment(json!, json!.IndexOf("\"remaining", StringComparison.Ordinal), json.Length - 3);
        //Console.WriteLine(@"Debug: remainingsection = {0}", remainingSection);
        var flowControl = JsonSerializer.Deserialize<FlowControl>(remainingSection);

        return flowControl?.remaining!.overall <= 0;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/