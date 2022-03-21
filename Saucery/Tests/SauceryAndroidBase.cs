﻿using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Saucery.DataSources;
using Saucery.Dojo;
using Saucery.Driver;
using System;

namespace Saucery.Tests
{
    [TestFixtureSource(typeof(PlatformTestData))]
    public class SauceryAndroidBase : SauceryRoot {
        protected SauceryAndroidDriver Driver;

        public SauceryAndroidBase(BrowserVersion browserVersion) : base(browserVersion) {
        }
        
        public override void InitialiseDriver(DriverOptions options, int waitSecs) {
            try {
                Driver = new SauceryAndroidDriver(options);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        [TearDown]
        public void Cleanup() {
            if (Driver != null) {
                var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                // log the result to SauceLabs
                SauceLabsStatusNotifier.NotifyStatus(Driver.GetSessionId(), passed);
                PrintSessionDetails();
                Driver.Quit();
            }
        }

        public void PrintSessionDetails() {
            try {
                var sessionId = Driver.GetSessionId();
                Console.WriteLine(@"SauceOnDemandSessionID={0} job-name={1}", sessionId, TestName);
            } catch(WebDriverException) {
                Console.WriteLine(@"Caught WebDriverException, quitting driver.");
                Driver.Quit();
            }
        }

        public override void InitialiseDriver(ICapabilities driver, int waitSecs) {
            throw new NotImplementedException();
        }
    }
}
/*
 * Copyright Andrew Gray, SauceForge
 * Date: 12th July 2014
 * 
 */