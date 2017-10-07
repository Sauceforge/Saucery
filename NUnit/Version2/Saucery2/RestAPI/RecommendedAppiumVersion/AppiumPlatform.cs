using System.Collections.Generic;

namespace Saucery2.RestAPI.RecommendedAppiumVersion {
    public class AppiumPlatform {
        public List<object> deprecated_backend_versions { get; set; }
        public string short_version { get; set; }
        public string long_name { get; set; }
        public string recommended_backend_version { get; set; }
        public string long_version { get; set; }
        public string api_name { get; set; }
        public List<string> supported_backend_versions { get; set; }
        public string device { get; set; }
        public string latest_stable_version { get; set; }
        public string automation_backend { get; set; }
        public string os { get; set; }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */