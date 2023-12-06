using RestSharp;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI;

internal class RestAPILimitsChecker {
    private RestResponse _response;
    private Dictionary<string, string> _headers;

    public RestAPILimitsChecker() {
        _response = new();
        _headers = [];
    }

    public void Update(RestResponse response) {
        _response = response;
        foreach(var p in response.Headers!) {
            var newHeaders = _headers
                    .ToDictionary(entry => entry.Key,
                                  entry => entry.Value);
            if (!_headers.ContainsKey(p.Name!)) {
                newHeaders.Add(p.Name!, p.Value!.ToString()!);
            } else {
                newHeaders.Remove(p.Name!);
                newHeaders.Add(p.Name!, p.Value!.ToString()!);
            }
            _headers = newHeaders;
        }
    }

    internal bool IsLimitExceeded() => NoRemaining() || IsIndicatorPresentInJson();

    private bool IsIndicatorPresentInJson()
    {
        return _response.Content == null || _response.Content.Contains(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_INDICATOR);
    }

    private bool NoRemaining() => int.Parse(_headers["x-ratelimit-remaining"]) <= 0;

    internal int GetReset() => int.Parse(_headers["x-ratelimit-reset"]);
}