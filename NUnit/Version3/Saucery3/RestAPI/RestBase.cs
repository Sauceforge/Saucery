using System.Net;
using System.Threading;
using RestSharp;
using Saucery3.Util;
using System;

namespace Saucery3.RestAPI {
    public abstract class RestBase {
        internal static string UserName = Enviro.SauceUserName;
        internal static string AccessKey = Enviro.SauceApiKey;
        internal static RestClient Client;
        internal static RestAPILimitsChecker LimitChecker;

        static RestBase() {
            Client = new RestClient(SauceryConstants.SAUCE_REST_BASE);
            LimitChecker = new RestAPILimitsChecker();
        }

        protected RestRequest BuildRequest(string request, Method method) {
            return new RestRequest(request, method) {
                Credentials = new NetworkCredential(UserName, AccessKey)
            };
        }

        protected string GetJsonResponse(string requestProforma) {
            var request = BuildRequest(requestProforma, Method.GET);
            request.OnBeforeDeserialization = resp => { resp.ContentType = SauceryConstants.JSON_CONTENT_TYPE; };

            var response = GetResponse(request);
            return response.Content;
        }

        protected string GetJsonResponseForUser(string requestProforma) {
            var request = BuildRequest(string.Format(requestProforma, UserName), Method.GET);
            request.OnBeforeDeserialization = resp => { resp.ContentType = SauceryConstants.JSON_CONTENT_TYPE; };

            var response = GetResponse(request);
            //Console.WriteLine("GetJsonResponseForUser response Content: " + response.Content);
            //Console.Out.Flush();
            //Console.WriteLine("GetJsonResponseForUser response remaining: " + response.Headers[2].Value.ToString());
            //Console.WriteLine("GetJsonResponseForUser reset remaining: " + response.Headers[5].Value.ToString());
            return response.Content;
        }

        private IRestResponse GetResponse(RestRequest request) {
            var response = Client.Execute(request);
            LimitChecker.Update(response);

            while (LimitChecker.IsLimitExceeded())
            {
                OnceOnlyMessages.RestApiLimitExceeded();
                Thread.Sleep(LimitChecker.GetReset());
                response = Client.Execute(request);
                LimitChecker.Update(response);
            }

            return response;
        }

        internal void EnsureExecution(RestRequest request)
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

        protected string ExtractJsonSegment(string json, int startIndex, int endIndex) {
            DebugMessages.ExtractJsonSegment(json, startIndex, endIndex);
            var len = endIndex - startIndex;
            var segment = json.Substring(startIndex, len);
            return string.Format(SauceryConstants.JSON_SEGMENT_CONTAINER, segment);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 29th July 2014
 * 
 */