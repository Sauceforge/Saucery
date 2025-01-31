using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.Util;
using System.Text.Json;

namespace Saucery.Core.RestAPI.FlowControl;

public class SauceLabsFlowController : RestBase {
    private readonly Lock _lock = new();

    public SauceLabsFlowController()
    {
        RestClientOptions clientOptions = new(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public void ControlFlow(bool realDevices) {
        while(TooManyTests(realDevices)) {
            Thread.Sleep(SauceryConstants.SAUCELABS_FLOW_WAIT);
        }
    }

    private bool TooManyTests(bool realDevices) {
        //int maxParallelMacSessionsAllowed;  //Possible future use.
        var json = GetJsonResponse(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST);

        if (json == null)
        {
            return true;
        }

        var flowControl = JsonSerializer.Deserialize<FlowControl>(json, JsonOptions);

        var org = flowControl?.concurrency.organization;
        var current = org?.current;
        var allowed = org?.allowed;

        lock (_lock)
        {
            var orgAllowed = realDevices ? allowed?.rds : allowed?.vms;
            var orgCurrent = realDevices ? current?.rds : current?.vms;

            return orgAllowed - orgCurrent <= 0;
        }
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/