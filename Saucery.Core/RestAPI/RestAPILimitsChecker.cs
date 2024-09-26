using RestSharp;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI;

internal class RestAPILimitsChecker {
    private RestResponse _response = new();
    private Dictionary<string, string> _headers = [];

    public void Update(RestResponse response) {
        _response = response;
        foreach(var p in response.Headers!) {
            var newHeaders = _headers
                    .ToDictionary(entry => entry.Key,
                                  entry => entry.Value);

            if (_headers.ContainsKey(p.Name)) {
                newHeaders.Remove(p.Name);
            }

            newHeaders.Add(p.Name, p.Value);

            _headers = newHeaders;
        }
    }

    internal bool IsLimitExceeded() => 
        NoRemaining() || 
        IsIndicatorPresentInJson();

    private bool IsIndicatorPresentInJson() => 
        _response.Content == null || 
        _response.Content.Contains(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_INDICATOR);

    private bool NoRemaining() => 
        _headers.ContainsKey("x-ratelimit-remaining") && 
        int.Parse(_headers["x-ratelimit-remaining"]) <= 0;

    internal int GetReset() => 
        _headers.TryGetValue("x-ratelimit-reset", out var value) 
            ? int.Parse(value)
            : 0;

}