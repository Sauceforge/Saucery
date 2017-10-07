using System;
using System.Collections.Generic;
using System.Linq;
using Saucery3.NuGet;
using Saucery3.OnDemand;

namespace Saucery3.Util {
    internal class OnceOnlyMessages {
        public static void CopyrightBanner() {
            OnceOnlyWriter.WriteLine(SauceryConstants.CONSOLE_RUNNER, Saucery3Constants.PRODUCTNAME/*, NuGetHelper.GetInstalledVersion()*/);
            OnceOnlyWriter.WriteLine(SauceryConstants.COPYRIGHT_NOTICE, GetCurrentYear());
            //NewVersionAvailable();
            OnceOnlyWriter.WriteLine(SauceryConstants.SPACE);
        }

        public static void TestingOn(List<SaucePlatform> platforms) {
            OnceOnlyWriter.WriteLine(platforms.Any()
                ? string.Format(SauceryConstants.TESTING_ON, platforms.Count, GetMoniker(platforms))
                : SauceryConstants.NO_PLATFORMS);
        }

        public static void OnDemand() {
            if (UserChecker.ItIsMe()) {
                OnceOnlyWriter.WriteLine(SauceryConstants.ON_DEMAND, Enviro.SauceOnDemandBrowsers);
            }
        }

        //private static void NewVersionAvailable() {
        //    if (NuGetHelper.IsNewVersionAvailable()) {
        //        var installed = NuGetHelper.GetInstalledVersion();
        //        var max = NuGetHelper.GetMaxVersion();
        //        OnceOnlyWriter.WriteLine(SauceryConstants.NEW_VERSION_AVAILABLE, installed, max);
        //    }
        //}

        public static void UsingLicenced() {
            OnceOnlyWriter.WriteLine(SauceryConstants.LICENCED_VERSION, Saucery3Constants.PRODUCTNAME);
        }

        public static void UsingTrial() {
            OnceOnlyWriter.WriteLine(SauceryConstants.TRIAL_VERSION, Saucery3Constants.PRODUCTNAME);
        }

        public static void DaysRemaining(double remaining) {
            OnceOnlyWriter.WriteLine(SauceryConstants.DAYS_REMAINING, remaining);
        }

        private static string GetCurrentYear() {
            return DateTime.Now.ToString(SauceryConstants.YEAR_FORMAT);
        }

        private static string GetMoniker(IReadOnlyCollection<SaucePlatform> platforms) {
            return platforms.Count == 1 ? "platform" : "platforms";
        }

        internal static void RestApiLimitExceeded() {
            OnceOnlyWriter.WriteLine(SauceryConstants.RESTAPI_LIMIT_EXCEEDED_MSG);
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 24th December 2015
 * 
 */