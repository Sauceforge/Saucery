using System;
using System.Collections.Generic;
using Newtonsoft.Json;
//using NUnit.Framework;
using $rootnamespace$.Activation;
using $rootnamespace$.OnDemand;
using $rootnamespace$.Util;

namespace $rootnamespace$.Tests {
    [SetUpFixture]
    public class GlobalSetup {
        [OneTimeSetUp]
        public void RunBeforeAnyTests() {
            var validator = new ActivationValidator();
            validator.CheckActivation();
            var ondemand = Enviro.SauceOnDemandBrowsers;
            var platforms = JsonConvert.DeserializeObject(ondemand, typeof (List<SaucePlatform>)) as List<SaucePlatform>;
            
            Console.WriteLine(platforms != null 
                                ? string.Format("Testing on {0} platforms", platforms.Count) 
                                : "Platforms is null");
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 15th August 2014
 * 
 */