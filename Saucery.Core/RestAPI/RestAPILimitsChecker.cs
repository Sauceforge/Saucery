using RestSharp;
using Saucery.Core.Util;

namespace Saucery.Core.RestAPI;

internal class RestAPILimitsChecker {
    private RestResponse Response;
    private readonly Dictionary<string, string> Headers;

    public RestAPILimitsChecker() {
        Headers = new Dictionary<string, string>();
    }

    public void Update(RestResponse response) {
        Response = response;
        foreach(var p in response.Headers) {
            if (!Headers.ContainsKey(p.Name)) {
                Headers.Add(p.Name, p.Value.ToString());
            } else {
                Headers.Remove(p.Name);
                Headers.Add(p.Name, p.Value.ToString());
            }
        }
    }

    internal bool IsLimitExceeded() => NoRemaining() || IsIndicatorPresentInJson();

    private bool IsIndicatorPresentInJson() {
        if (Response == null || Response.Content == null) {
            return true;
        }

        return Response.Content.Contains(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_INDICATOR);
    }

    private bool NoRemaining() => int.Parse(Headers["x-ratelimit-remaining"]) <= 0;

    internal int GetReset() => int.Parse(Headers["x-ratelimit-reset"]);
}