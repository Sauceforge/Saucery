﻿using RestSharp;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.RestAPI
{
    internal class RestAPILimitsChecker {
        private RestResponse Response;
        private Dictionary<string, string> Headers;

        public RestAPILimitsChecker() {
            Headers = new Dictionary<string, string>();
        }

        public void Update(RestResponse response) {
            Response = response;
            foreach(var p in response.Headers) {
                if (!Headers.ContainsKey(p.Name)) {
                    Headers.Add(p.Name, p.Value.ToString());
                }
            }
        }

        internal bool IsLimitExceeded() {
            return NoRemaining() || IsIndicatorPresentInJson();
        }

        private bool IsIndicatorPresentInJson() {
            if (Response == null || Response.Content == null) {
                return true;
            }

            return Response.Content.Contains(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_INDICATOR);
        }

        private bool NoRemaining() {
            return int.Parse(Headers["X-Ratelimit-Remaining"]) <= 0;
        }

        internal int GetReset() {
            return int.Parse(Headers["X-Ratelimit-Reset"]);
        }
    }
}