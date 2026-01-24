using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI.FlowControl;

public class SauceLabsFlowController : RestBase {
    public SauceLabsFlowController()
    {
        RestClientOptions clientOptions = new(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };

        Client = new RestClient(clientOptions);
    }

    public async Task ControlFlowAsync(bool realDevices, CancellationToken ct = default) {
        while (await TooManyTestsAsync(realDevices, ct).ConfigureAwait(false)) {
            await Task.Delay(SauceryConstants.SAUCELABS_FLOW_WAIT, ct).ConfigureAwait(false);
        }
    }

    private async Task<bool> TooManyTestsAsync(bool realDevices, CancellationToken ct = default) {
        var json = await GetJsonResponseAsync(SauceryConstants.ACCOUNT_CONCURRENCY_REQUEST, ct).ConfigureAwait(false);

        if (json == null)
        {
            return true;
        }

        var flowControl = JsonSerializer.Deserialize<FlowControl>(json, JsonOptions);

        var org = flowControl?.concurrency.organization;
        var current = org?.current;
        var allowed = org?.allowed;

        // No shared mutable state; lock is unnecessary here. Keep computation fast and lock-free.
        var orgAllowed = realDevices ? allowed?.rds : allowed?.vms;
        var orgCurrent = realDevices ? current?.rds : current?.vms;

        return orgAllowed - orgCurrent <= 0;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/