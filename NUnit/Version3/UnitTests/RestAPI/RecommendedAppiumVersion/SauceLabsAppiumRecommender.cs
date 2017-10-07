using System.Collections.Generic;
using Newtonsoft.Json;
using UnitTests.RestAPI.RecommendedAppiumVersion.Base;

namespace UnitTests.RestAPI.RecommendedAppiumVersion {
    public class SauceLabsAppiumRecommender : AppiumRecommender {
        public override string RecommendAppium() {
            var json = GetJsonResponse(SauceryConstants.RECOMMENDED_APPIUM_REQUEST);
            var recommendedAppiumVersion = JsonConvert.DeserializeObject<List<AppiumPlatform>>(json);
            return recommendedAppiumVersion[0].recommended_backend_version;
        }
    }
}

/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 10th August 2014
 * 
 */
