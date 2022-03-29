using Newtonsoft.Json;
using Saucery.RestAPI.RecommendedAppiumVersion.Base;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.RestAPI.RecommendedAppiumVersion
{
    public class SauceLabsAppiumRecommender : PlatformAcquirer
    {
        public override string RecommendAppium() {
            var json = GetJsonResponse(SauceryConstants.RECOMMENDED_APPIUM_REQUEST);
            var recommendedAppiumVersion = JsonConvert.DeserializeObject<List<SupportedPlatform>>(json);
            return recommendedAppiumVersion[0].recommended_backend_version;
        }
    }
}

/*
 * Copyright Andrew Gray, SauceForge
 * Date: 10th August 2014
 * 
 */
