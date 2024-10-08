﻿using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Service;
using Saucery.Core.Dojo;
using Saucery.Core.Driver;
using Saucery.Core.OnDemand;
using Saucery.Core.Options;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.RestAPI.TestStatus;
using Saucery.Core.Util;

namespace Saucery;

public class SauceryBase()
{
    private string? _testName;
    protected WebDriver? Driver;
    private readonly BrowserVersion? _browserVersion;
    private readonly SauceLabsStatusNotifier SauceLabsStatusNotifier = new();
    private readonly SauceLabsFlowController SauceLabsFlowController = new();
    private OptionFactory? _optionFactory;
    private readonly AppiumClientConfig AppiumClientConfig = new() { DirectConnect = true };

    protected SauceryBase(BrowserVersion browserVersion) : this() => 
        _browserVersion = browserVersion;

    [SetUp]
    public void Setup()
    {
        _browserVersion?.SetTestName(TestContext.CurrentContext.Test.Name);
        _testName = _browserVersion?.TestName;

        //DebugMessages.PrintPlatformDetails(platform);
        // set up the desired options
        _optionFactory = new OptionFactory(_browserVersion!);
        var tuple = _optionFactory.CreateOptions(_testName!);

        var driverInitialised = InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);

        while (!driverInitialised)
        {
            Console.WriteLine($"Driver failed to initialise: {TestContext.CurrentContext.Test.Name}.");
            driverInitialised = InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        }
        Console.WriteLine($"Driver successfully initialised: {TestContext.CurrentContext.Test.Name}.");
    }

    [TearDown]
    public void Cleanup()
    {
        try
        {
            _testName = null;

            if (Driver != null)
            {
                //var passed = Equals(TestContext.CurrentContext.Result.Outcome., ResultState.Success);
                var isPassed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
                // log the result to SauceLabs
                var sessionId = Driver.SessionId.ToString();
                SauceLabsStatusNotifier.NotifyStatus(sessionId, isPassed);
                Console.WriteLine($"SessionID={sessionId} job-name={_testName}");
                Driver.Quit();
            }

            if(_optionFactory != null)
            {
                _optionFactory!.Dispose();
            }
        }
        catch (WebDriverException)
        {
            Console.WriteLine("Caught WebDriverException, quitting driver.");
            Driver?.Quit();
            _optionFactory!.Dispose();
        }
    }

    private bool InitialiseDriver((DriverOptions opts, BrowserVersion browserVersion) tuple, int waitSecs)
    {
        SauceLabsFlowController.ControlFlow(tuple.browserVersion.IsARealDevice());

        try
        {
            Driver = _optionFactory!.IsApple()
                ? new IOSDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, TimeSpan.FromSeconds(waitSecs), AppiumClientConfig)
                : _optionFactory!.IsAndroid()
                    ? new AndroidDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, TimeSpan.FromSeconds(waitSecs), AppiumClientConfig)
                    : new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, waitSecs);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    protected WebDriver SauceryDriver() => 
        Driver!;
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/