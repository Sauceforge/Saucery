using Newtonsoft.Json;
using Saucery.RestAPI.SupportedPlatforms.Base;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.RestAPI.SupportedPlatforms {
    public class SauceLabsPlatformAcquirer : PlatformAcquirer {
        public override List<SupportedPlatform> AcquirePlatforms() {
            var json = GetJsonResponse(SauceryConstants.SUPPORTED_PLATFORMS_REQUEST);
            var supportedPlatforms = JsonConvert.DeserializeObject<List<SupportedPlatform>>(json);
            return supportedPlatforms;
        }
    }
}