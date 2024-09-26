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
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public virtual void ControlFlow(bool realDevices) {
        while(TooManyTests(realDevices)) {
            Thread.Sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
        }
    }

    protected virtual bool TooManyTests(bool realDevices) {
        //int maxParallelMacSessionsAllowed;  //Possible future use.
        var json = GetJsonResponse(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST);

        if (json == null)
        {
            return true;
        }

        //Console.WriteLine(@"Debug: {0}", json);
        //var remainingSection = ExtractJsonSegment(json!, json!.IndexOf("\"remaining", StringComparison.Ordinal), json.Length - 3);
        //Console.WriteLine(@"Debug: remainingsection = {0}", remainingSection);
        var flowControl = JsonSerializer.Deserialize<FlowControl>(json, JsonOptions);

        var org = flowControl?.concurrency.organization;
        var orgAllowed = realDevices ? org.allowed.rds : org.allowed.vms;
        var orgCurrent = realDevices ? org.current.rds : org.current.vms;

        return orgAllowed - orgCurrent <= 0;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/