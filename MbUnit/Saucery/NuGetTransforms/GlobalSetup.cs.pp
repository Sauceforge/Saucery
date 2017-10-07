using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Newtonsoft.Json;
using Saucery.Activation;
using Saucery.OnDemand;
using Saucery.Util;

namespace $rootnamespace$.Tests {
    [AssemblyFixture]
    public class GlobalSetup {
        [FixtureSetUp]
        public void RunBeforeAnyTests() {
            var validator = new ActivationValidator();
            validator.CheckActivation();
            var ondemand = Environment.GetEnvironmentVariable(SauceryConstants.SAUCE_ONDEMAND_BROWSERS);
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