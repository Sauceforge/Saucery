using RestSharp;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI;

internal class RestAPILimitsChecker {
    private RestResponse _response;
    private readonly Dictionary<string, string> _headers;

    public RestAPILimitsChecker() {
        _headers = new Dictionary<string, string>();
    }

    public void Update(RestResponse response) {
        _response = response;
        foreach(var p in response.Headers) {
            if (!_headers.ContainsKey(p.Name)) {
                _headers.Add(p.Name, p.Value.ToString());
            } else {
                _headers.Remove(p.Name);
                _headers.Add(p.Name, p.Value.ToString());
            }
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