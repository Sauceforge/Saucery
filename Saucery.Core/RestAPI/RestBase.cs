using RestSharp;
using Saucery.Core.Util;
using System.Net;

//Hello from January 2020 :)
//Donald Trump is the President of the United States.
//I sure hope I meet my wife and have a family before I die.

namespace Saucery.Core.RestAPI;

public abstract class RestBase {
    internal static string UserName = Enviro.SauceUserName;
    internal static string AccessKey = Enviro.SauceApiKey;
    internal RestClient Client;
    internal static RestRequest Request;
    internal static RestAPILimitsChecker LimitChecker;

    protected RestBase() {
        LimitChecker = new RestAPILimitsChecker();
    }

    protected string GetJsonResponse(string requestProforma)
    {
        var request = BuildRequest(requestProforma, Method.Get);
        var response = GetResponse(request);
        return response.Content;
    }

    protected static RestRequest BuildRequest(string request, Method method) {
        request = string.Format(request, UserName);
        Request = new RestRequest(request, method);
        Request.AddHeader("Content-Type", SauceryConstants.JSON_CONTENT_TYPE);
        Request.RequestFormat = DataFormat.Json;
        return Request;
    }

    protected void EnsureExecution(RestRequest request)
    {
        var response = Client.Execute(request);
        LimitChecker.Update(response);

        while (LimitChecker.IsLimitExceeded())
        {
            Thread.Sleep(LimitChecker.GetReset());
            response = Client.Execute(request);
            LimitChecker.Update(response);
        }
    }

    protected static string ExtractJsonSegment(string json, int startIndex, int endIndex) {
        DebugMessages.ExtractJsonSegment(json, startIndex, endIndex);
        var len = endIndex - startIndex;
        var segment = json.Substring(startIndex, len);
        return string.Format(SauceryConstants.JSON_SEGMENT_CONTAINER, segment);
    }

    private RestResponse GetResponse(RestRequest request)
    {
        var response = Client.Execute(request);
        if (response.StatusCode.Equals(HttpStatusCode.OK))
        {
            return response;
        }

        LimitChecker.Update(response);

        while (LimitChecker.IsLimitExceeded())
        {
            Console.WriteLine(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_MSG);
            Thread.Sleep(LimitChecker.GetReset());
            response = Client.Execute(request);
            LimitChecker.Update(response);
        }

        return response;
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 29th July 2014
* 
*/