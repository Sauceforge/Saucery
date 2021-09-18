using OpenQA.Selenium;
using Saucery.Util;
using System.Collections.Generic;

namespace Saucery.Options.Base
{
    internal abstract class BaseOptions {
        protected DriverOptions Opts = null;
        protected Dictionary<string, object> SauceOptions = null;
        private readonly string _testName;

        protected BaseOptions(string testName) {
            _testName = testName;
            SauceOptions = new Dictionary<string, object>
            {
                { SauceryConstants.SAUCE_USERNAME_CAPABILITY, Enviro.SauceUserName },
                { SauceryConstants.SAUCE_ACCESSKEY_CAPABILITY, Enviro.SauceApiKey },
                { SauceryConstants.SELENIUM_VERSION_CAPABILITY, SauceryConstants.LATEST_SELENIUM_VERSION },
                //This sets the Session column
                { SauceryConstants.SAUCE_SESSIONNAME_CAPABILITY, _testName },
                //This sets the Build column
                { SauceryConstants.SAUCE_BUILDNAME_CAPABILITY, Enviro.BuildName },
                //Improve performance on SauceLabs
                { SauceryConstants.SAUCE_VUOP_CAPABILITY, false }
            };
            //SauceOptions.Add(Constants.VISIBILITY_KEY, Constants.VISIBILITY_TEAM);
        }

        protected void AddSauceLabsOptions(string nativeApp) {
            if (nativeApp != null) {
                SauceOptions.Add(SauceryConstants.SAUCE_NATIVE_APP_CAPABILITY, nativeApp);
            }
        }

        public DriverOptions GetOpts() {
            return Opts;
        }

        //protected static string GetBrowser(string nativeApp) {
        //    return nativeApp != null ? "" : SauceryConstants.SAFARI_BROWSER;
        //}
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 5th February 2020
 * 
 */