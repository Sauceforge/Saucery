using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.Driver;
using Saucery.Core.OnDemand;
using Saucery.Core.Options;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.RestAPI.SupportedPlatforms;
using Saucery.Core.RestAPI.TestStatus;
using Saucery.Core.Util;

namespace Saucery.Playwright.NUnit;

public class SauceryBase : PageTest
{
    private string? _testName;
    private WebDriver? _driver;
    private readonly BrowserVersion? _browserVersion;
    private static readonly SauceLabsEmulatedStatusNotifier SauceLabsEmulatedStatusNotifier;
    private static readonly SauceLabsRealDeviceStatusNotifier SauceLabsRealDeviceStatusNotifier;
    private static readonly SauceLabsFlowController SauceLabsFlowController;
    private static readonly SauceLabsRealDeviceAcquirer SauceLabsRealDeviceAcquirer;

    static SauceryBase()
    {
        SauceLabsEmulatedStatusNotifier = new SauceLabsEmulatedStatusNotifier();
        SauceLabsFlowController = new SauceLabsFlowController();
        SauceLabsRealDeviceAcquirer = new SauceLabsRealDeviceAcquirer();
        SauceLabsRealDeviceStatusNotifier = new SauceLabsRealDeviceStatusNotifier();
    }

    protected SauceryBase()
    {

    }

    protected SauceryBase(BrowserVersion browserVersion) => 
        _browserVersion = browserVersion;

    [SetUp]
    public void Setup()
    {
        lock(_browserVersion!) {
            _browserVersion!.SetTestName(TestContext.CurrentContext.Test.Name);
            _testName = _browserVersion.TestName!;
        }

        // set up the desired options
        var factory = new OptionFactory(_browserVersion);
        var tuple = factory.CreateOptions(_testName);

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
            if (_driver != null)
            {
                var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                // log the result to SauceLabs
                lock(_browserVersion!) {
                    if(_browserVersion.IsARealDevice()) {
                        var realDeviceJobs = SauceLabsRealDeviceAcquirer.AcquireRealDeviceJobs();
                        var job = realDeviceJobs?.entities.Find(x => x.name.Equals(_browserVersion.TestName));
                        SauceLabsRealDeviceStatusNotifier.NotifyRealDeviceStatus(job!.id, passed);
                    } else {
                        SauceLabsEmulatedStatusNotifier.NotifyEmulatedStatus(_driver.SessionId.ToString(), passed);
                    }
                }

                _driver.Quit();
            }
        }
        catch (WebDriverException)
        {
            Console.WriteLine("Caught WebDriverException, quitting driver.");
            _driver?.Quit();
        }
    }

    private bool InitialiseDriver((DriverOptions opts, BrowserVersion browserVersion) tuple, int waitSecs)
    {
        SauceLabsFlowController.ControlFlow(tuple.browserVersion.IsARealDevice());
        try
        {
            _driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, waitSecs);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public SauceryRemoteWebDriver SauceryDriver() => (SauceryRemoteWebDriver)_driver!;
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/