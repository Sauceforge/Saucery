using RestSharp;
using Saucery.Core.Util;
using System.Net;
using System.Text.Json;

//Hello from January 2020 :)
//Donald Trump is the President of the United States.
//I sure hope I meet my wife and have a family before I die.

namespace Saucery.Core.RestAPI;

public abstract class RestBase {
    internal static readonly string UserName = Enviro.SauceUserName!;
    internal static readonly string AccessKey = Enviro.SauceApiKey!;
    internal RestClient? Client;
    private static RestRequest? _request;
    private static RestAPILimitsChecker? _limitChecker;

    internal static JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected RestBase() 
    {
        _limitChecker = new RestAPILimitsChecker();
    }

    protected string? GetJsonResponse(string requestProforma)
    {
        var request = BuildRequest(requestProforma, Method.Get);
        var response = GetResponse(request);
        return response.Content;
    }

    protected static RestRequest BuildRequest(string request, Method method) {
        request = string.Format(request, UserName);
        _request = new RestRequest(request, method);
        _request.AddHeader("Content-Type", SauceryConstants.JSON_CONTENT_TYPE);
        _request.RequestFormat = DataFormat.Json;
        
        return _request;
    }
    
    protected void EnsureExecution(RestRequest request)
    {
        var response = Client!.Execute<RestRequest>(request);
        _limitChecker?.Update(response);

        while (_limitChecker!.IsLimitExceeded())
        {
            Thread.Sleep(_limitChecker.GetReset());
            response = Client!.Execute<RestRequest>(request);
            _limitChecker.Update(response);
        }
    }

    private RestResponse<RestRequest> GetResponse(RestRequest request)
    {
        var response = Client!.Execute<RestRequest>(request);
        if (response.StatusCode.Equals(HttpStatusCode.OK))
        {
            return response;
        }

        _limitChecker!.Update(response);

        while (_limitChecker.IsLimitExceeded())
        {
            Console.WriteLine(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_MSG);
            Thread.Sleep(_limitChecker.GetReset());
            response = Client!.Execute<RestRequest>(request);
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