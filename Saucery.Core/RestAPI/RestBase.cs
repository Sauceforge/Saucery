using System.Net;
using System.Text.Json;
using RestSharp;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI;

public abstract class RestBase {
    internal static readonly string UserName = Enviro.SauceUserName!;
    internal static readonly string AccessKey = Enviro.SauceApiKey!;
    internal RestClient? Client;
    private readonly RestAPILimitsChecker? _limitChecker = new();
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Async replacement for JSON retrieval
    protected async Task<string?> GetJsonResponseAsync(string requestProforma, CancellationToken ct = default)
    {
        var request = BuildRequest(requestProforma, Method.Get);
        var response = await GetResponseAsync(request, ct).ConfigureAwait(false);
        return response?.Content;
    }

    protected static RestRequest BuildRequest(string request, Method method)
    {
        request = string.Format(request, UserName);

        RestRequest restRequest = new(request, method);
        restRequest.AddHeader("Content-Type", SauceryConstants.JSON_CONTENT_TYPE);
        restRequest.RequestFormat = DataFormat.Json;

        return restRequest;
    }

    protected async Task EnsureExecutionAsync(RestRequest request, CancellationToken ct = default)
    {
        var response = await Client!.ExecuteAsync(request, ct).ConfigureAwait(false);
        _limitChecker?.Update(response);

        while (_limitChecker!.IsLimitExceeded())
        {
            var delay = _limitChecker.GetReset();
            await Task.Delay(delay, ct).ConfigureAwait(false);
            response = await Client!.ExecuteAsync(request, ct).ConfigureAwait(false);
            _limitChecker.Update(response);
        }
    }

    private async Task<RestResponse> GetResponseAsync(RestRequest request, CancellationToken ct = default)
    {
        var response = await Client!.ExecuteAsync(request, ct).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return response;
        }

        _limitChecker!.Update(response);

        while (_limitChecker.IsLimitExceeded())
        {
            Console.WriteLine(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_MSG);
            var delay = _limitChecker.GetReset();
            await Task.Delay(delay, ct).ConfigureAwait(false);
            response = await Client!.ExecuteAsync(request, ct).ConfigureAwait(false);
            _limitChecker.Update(response);
        }

        return response;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 29th July 2014
* 
*/