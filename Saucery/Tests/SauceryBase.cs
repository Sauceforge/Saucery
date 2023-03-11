using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.Driver;
using Saucery.Core.Options;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.RestAPI.TestStatus;
using Saucery.Core.Util;
using System;

namespace Saucery.Tests;

public class SauceryBase {
    protected string TestName;
    protected SauceryRemoteWebDriver Driver;
    protected readonly BrowserVersion BrowserVersion;
    internal static SauceLabsStatusNotifier SauceLabsStatusNotifier;
    internal static SauceLabsFlowController SauceLabsFlowController;

    static SauceryBase() {
        SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        SauceLabsFlowController = new SauceLabsFlowController();
    }

    protected SauceryBase(BrowserVersion browserVersion) {
        BrowserVersion = browserVersion;
    }

    public bool InitialiseDriver(DriverOptions opts, int waitSecs) {
        SauceLabsFlowController.ControlFlow();
        try {
            Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts, waitSecs);
            return true;
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    [SetUp]
    public void Setup() {
        BrowserVersion.SetTestName(TestContext.CurrentContext.Test.Name);
        TestName = BrowserVersion.TestName;

        //DebugMessages.PrintPlatformDetails(platform);
        // set up the desired options
        var factory = new OptionFactory(BrowserVersion);
        var opts = factory.CreateOptions(TestName);

        bool driverInitialised = InitialiseDriver(opts, 400);

        while (!driverInitialised)
        {
            Console.WriteLine($"Driver failed to initialise: {TestContext.CurrentContext.Test.Name}.");
            driverInitialised = InitialiseDriver(opts, 400);
        }
        Console.WriteLine($"Driver successfully initialised: {TestContext.CurrentContext.Test.Name}.");
    }

    [TearDown]
    public void Cleanup() {
        try {
            if (Driver != null) {
                var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                // log the result to SauceLabs
                var sessionId = Driver.GetSessionId();
                SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                Console.WriteLine(@"SessionID={0} job-name={1}", sessionId, TestName);
                Driver.Quit();
            }
        } catch (WebDriverException) {
            Console.WriteLine(@"Caught WebDriverException, quitting driver.");
            Driver.Quit();
        }
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/