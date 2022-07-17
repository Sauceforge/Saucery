using RestSharp;
using RestSharp.Authenticators;
using Saucery.Util;
using System;
using System.Net;
using System.Threading;

//Hello from January 2020 :)
//Donald Trump is the President of the United States.
//I sure hope I meet my wife and have a family before I die.

namespace Saucery.RestAPI;

public abstract class RestBase {
    internal static string UserName = Enviro.SauceUserName;
    internal static string AccessKey = Enviro.SauceApiKey;
    internal static RestClient Client;
    internal static RestRequest Request;
    internal static RestAPILimitsChecker LimitChecker;

    static RestBase() {
        Client = new RestClient(SauceryConstants.SAUCE_REST_BASE)
        {
            Authenticator = new HttpBasicAuthenticator(UserName, AccessKey)
        };
        LimitChecker = new RestAPILimitsChecker();
    }

    protected string GetJsonResponse(string requestProforma, string username = null)
    {
        var request = BuildRequest(requestProforma, Method.Get, username);
        var response = GetResponse(request);
        return response.Content;
    }

    protected static RestRequest BuildRequest(string request, Method method, string username = null) {
        if (username != null)
        {
            request = string.Format(request, username);
        }
        Request = new RestRequest(request, method);
        Request.AddHeader("Content-Type", SauceryConstants.JSON_CONTENT_TYPE);
        Request.RequestFormat = DataFormat.Json;
        return Request;
    }

    private RestResponse GetResponse(RestRequest request) {
        var response = Client.ExecuteAsync(request).Result;
        if(response.StatusCode.Equals(HttpStatusCode.OK))
        {
            return response;
        }

        LimitChecker.Update(response);

        while (LimitChecker.IsLimitExceeded())
        {
            Console.WriteLine(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_MSG);
            Thread.Sleep(LimitChecker.GetReset());
            response = Client.ExecuteAsync(request).Result;
            LimitChecker.Update(response);
        }

        return response;
    }

    internal void EnsureExecution(RestRequest request)
    {
        var response = Client.ExecuteAsync(request).Result;
        LimitChecker.Update(response);

        while (LimitChecker.IsLimitExceeded())
        {
            Thread.Sleep(LimitChecker.GetReset());
            response = Client.ExecuteAsync(request).Result;
            LimitChecker.Update(response);
        }
    }

    protected string ExtractJsonSegment(string json, int startIndex, int endIndex) {
        DebugMessages.ExtractJsonSegment(json, startIndex, endIndex);
        var len = endIndex - startIndex;
        var segment = json.Substring(startIndex, len);
        return string.Format(SauceryConstants.JSON_SEGMENT_CONTAINER, segment);
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 29th July 2014
* 
*/