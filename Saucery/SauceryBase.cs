using NUnit.Framework;
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
using Saucery.Core.RestAPI.SupportedPlatforms;
using Saucery.Core.RestAPI.TestStatus;
using Saucery.Core.Util;

namespace Saucery;

public class SauceryBase
{
    private string? _testName;
    protected WebDriver? Driver;
    private readonly BrowserVersion? _browserVersion;
    private readonly SauceLabsEmulatedStatusNotifier _sauceLabsEmulatedStatusNotifier = new();
    private readonly SauceLabsRealDeviceStatusNotifier _sauceLabsRealDeviceStatusNotifier = new();
    private readonly SauceLabsFlowController _sauceLabsFlowController = new();
    private readonly SauceLabsRealDeviceAcquirer _sauceLabsRealDeviceAcquirer = new();
    private OptionFactory? _optionFactory;
    private readonly AppiumClientConfig _appiumClientConfig = new() { DirectConnect = true };

    protected SauceryBase(BrowserVersion browserVersion) => 
        _browserVersion = browserVersion;

    [SetUp]
    public void Setup()
    {
        lock (_browserVersion!)
        {
            _browserVersion?.SetTestName(TestContext.CurrentContext.Test.Name);
            _testName = _browserVersion?.TestName;
        }

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
                var isPassed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
                // log the result to SauceLabs
                var sessionId = Driver.SessionId.ToString();
                lock (_browserVersion!)
                {
                    if (_browserVersion.IsARealDevice())
                    {
                        var realDeviceJobs = _sauceLabsRealDeviceAcquirer.AcquireRealDeviceJobs();
                        var job = realDeviceJobs?.entities.Find(x => x.name.Equals(_browserVersion.TestName));
                        _sauceLabsRealDeviceStatusNotifier.NotifyRealDeviceStatus(job!.id, isPassed);
                    }
                    else
                    {
                        _sauceLabsEmulatedStatusNotifier.NotifyEmulatedStatus(Driver.SessionId.ToString(), isPassed);
                    }
                }

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
        _sauceLabsFlowController.ControlFlow(tuple.browserVersion.IsARealDevice());

        try
        {
            Driver = _optionFactory!.IsApple()
                ? new IOSDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, TimeSpan.FromSeconds(waitSecs), _appiumClientConfig)
                : _optionFactory!.IsAndroid()
                    ? new AndroidDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, TimeSpan.FromSeconds(waitSecs), _appiumClientConfig)
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