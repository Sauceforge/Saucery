using OpenQA.Selenium.Remote;
using Saucery2.Util;

namespace Saucery2.Capabilities.Base {
    internal abstract class BaseCapabilities {
        protected DesiredCapabilities Caps = null;
        private readonly string _testName;

        protected BaseCapabilities(string testName) {
            _testName = testName;
        }

        protected void AddSauceLabsCapabilities() {
            Caps.SetCapability(SauceryConstants.SAUCE_USERNAME_CAPABILITY, Enviro.SauceUserName);
            Caps.SetCapability(SauceryConstants.SAUCE_ACCESSKEY_CAPABILITY, Enviro.SauceApiKey);
            //This sets the Session column
            Caps.SetCapability(SauceryConstants.SAUCE_SESSIONNAME_CAPABILITY, _testName);
            //This sets the Build column
            Caps.SetCapability(SauceryConstants.SAUCE_BUILDNAME_CAPABILITY, Enviro.BuildNumber);
            //Improve performance on SauceLabs
            Caps.SetCapability(SauceryConstants.SAUCE_VUOP_CAPABILITY, false);
            //Caps.SetCapability(Constants.VISIBILITY_KEY, Constants.VISIBILITY_TEAM);
        }

        protected void AddSauceLabsCapabilities(string nativeApp) {
            AddSauceLabsCapabilities();
            if (nativeApp != null) {
                Caps.SetCapability(SauceryConstants.SAUCE_NATIVE_APP_CAPABILITY, nativeApp);
            }
        }

        public DesiredCapabilities GetCaps() {
            return Caps;
        }

        protected static string GetBrowser(string nativeApp, bool useChromeOnAndroid) {
            return nativeApp != null
                ? ""
                : useChromeOnAndroid
                    ? SauceryConstants.CHROME_BROWSER
                    : SauceryConstants.DEFAULT_ANDROID_BROWSER;
        }

        protected static string GetBrowser(string nativeApp) {
            return nativeApp != null ? "" : SauceryConstants.SAFARI_BROWSER;
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */