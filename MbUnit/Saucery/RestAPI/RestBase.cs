using System;
using System.Net;
using RestSharp;
using Saucery.Util;

namespace Saucery.RestAPI {
    public abstract class RestBase {
        internal static string UserName;
        internal static string AccessKey;
        internal static RestClient Client;

        static RestBase() {
            UserName = Environment.GetEnvironmentVariable(SauceryConstants.SAUCE_USER_NAME);
            AccessKey = Environment.GetEnvironmentVariable(SauceryConstants.SAUCE_API_KEY);
            Client = new RestClient(SauceryConstants.SAUCE_REST_BASE);
        }

        protected RestRequest BuildRequest(string request, Method method) {
            return new RestRequest(request, method) {
                Credentials = new NetworkCredential(UserName, AccessKey)
            };
        }
    }
}