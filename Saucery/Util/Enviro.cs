﻿using System;

namespace Saucery.Util {
    public static class Enviro {
        internal static string SauceNativeApp => GetStringVar(SauceryConstants.SAUCE_NATIVE_APP);

        public static string SauceUserName => GetStringVar(SauceryConstants.SAUCE_USER_NAME);

        public static string SauceApiKey => GetStringVar(SauceryConstants.SAUCE_API_KEY);

        public static string SauceOnDemandBrowsers => GetStringVar(SauceryConstants.SAUCE_ONDEMAND_BROWSERS);

        internal static bool SauceUseChromeOnAndroid => GetBoolVar(SauceryConstants.SAUCE_USE_CHROME_ON_ANDROID);

        //TODO: Guid needs to be a singleton
        internal static string BuildName => string.Format("Desktop_{0}", GetStringVar(SauceryConstants.BUILD_NUMBER) ?? IDGenerator.Id);

        internal static string BuildNumber
        {
            get
            {
                var jenkins = JenkinsBuildNumber;
                return !string.IsNullOrEmpty(jenkins) ? jenkins : BambooBuildNumber;
            }
        }

        public static string RecommendedAppiumVersion => GetStringVar(SauceryConstants.RECOMMENDED_APPIUM_VERSION);

        public static void SetVar(string variableName, string value) {
            if (GetStringVar(variableName) == null) {
                //Set it
                Environment.SetEnvironmentVariable(variableName, value);
            }
        }

        private static string JenkinsBuildNumber
        {
            get { return GetStringVar(SauceryConstants.JENKINS_BUILD_NUMBER); }
        }

        private static string BambooBuildNumber
        {
            get { return GetStringVar(SauceryConstants.BAMBOO_BUILD_NUMBER); }
        }

        private static string GetStringVar(string envVar) {
            return envVar == null ? null : Environment.GetEnvironmentVariable(envVar);
        }

        private static bool GetBoolVar(string envVar) {
            var v = GetStringVar(envVar);
            return v != null && Convert.ToBoolean(v);
        }

        private static int GetIntVar(string envVar) {
            var v = GetStringVar(envVar);
            return v == null ? 0 : Convert.ToInt32(v);
        }

        private static double GetDoubleVar(string envVar) {
            var v = GetStringVar(envVar);
            return v == null ? 0 : Convert.ToDouble(v);
        }
    }
}

/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */