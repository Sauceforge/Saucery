using System.Net;
using RestSharp;

namespace UnitTests.RestAPI {
    public abstract class RestBase {
        internal static string UserName = Enviro.SauceUserName;
        internal static string AccessKey = Enviro.SauceApiKey;
        internal static RestClient Client;

        static RestBase() {
            Client = new RestClient(SauceryConstants.SAUCE_REST_BASE);
        }

        protected RestRequest BuildRequest(string request, Method method) {
            return new RestRequest(request, method) {
                Credentials = new NetworkCredential(UserName, AccessKey)
            };
        }

        protected string GetJsonResponse(string requestProforma) {
            var request = BuildRequest(requestProforma, Method.GET);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            return Client.Execute(request).Content;
        }

        protected string GetJsonResponseForUser(string requestProforma) {
            var request = BuildRequest(string.Format(requestProforma, UserName), Method.GET);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            return Client.Execute(request).Content;
        }

        protected string ExtractJsonSegment(string json, int startIndex, int endIndex) {
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