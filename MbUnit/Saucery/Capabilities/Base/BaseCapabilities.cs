using System;
using OpenQA.Selenium.Remote;
using Saucery.Util;

namespace Saucery.Capabilities.Base {
    internal abstract class BaseCapabilities {
        protected DesiredCapabilities Caps = null;
        private readonly String _testName;

        protected BaseCapabilities(String testName){
            _testName = testName;
        }

        protected void AddSauceLabsCapabilities(){
            Caps.SetCapability(SauceryConstants.SAUCE_USERNAME_CAPABILITY, Environment.GetEnvironmentVariable(SauceryConstants.SAUCE_USER_NAME));
            Caps.SetCapability(SauceryConstants.SAUCE_ACCESSKEY_CAPABILITY, Environment.GetEnvironmentVariable(SauceryConstants.SAUCE_API_KEY));
            //This sets the Session column
            Caps.SetCapability(SauceryConstants.SAUCE_SESSIONNAME_CAPABILITY, _testName);
            //This sets the Build column
            Caps.SetCapability(SauceryConstants.SAUCE_BUILDNAME_CAPABILITY, Environment.GetEnvironmentVariable(SauceryConstants.JENKINS_BUILD_NUMBER)); 
            //Improve performance on SauceLabs
            Caps.SetCapability(SauceryConstants.SAUCE_VUOP_CAPABILITY, false);
            //Caps.SetCapability(Constants.VISIBILITY_KEY, Constants.VISIBILITY_TEAM);
        }

        public DesiredCapabilities GetCaps() {
            return Caps;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */