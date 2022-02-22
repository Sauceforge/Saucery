﻿using System.Collections.Generic;

namespace Saucery.RestAPI {
    public class SupportedPlatform {
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


        public int short_version_as_int => int.TryParse(short_version, out int discard) ? int.Parse(short_version) : 0;
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 18th September 2014
 * 
 */