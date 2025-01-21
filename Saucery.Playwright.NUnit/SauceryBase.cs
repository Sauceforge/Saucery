using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.Driver;
using Saucery.Core.OnDemand;
using Saucery.Core.Options;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.RestAPI.TestStatus;
using Saucery.Core.Util;

namespace Saucery.Playwright.NUnit;

public class SauceryBase : PageTest
{
    private string? _testName;
    private WebDriver? Driver;
    private readonly BrowserVersion? _browserVersion;
    private static readonly SauceLabsStatusNotifier SauceLabsStatusNotifier;
    private static readonly SauceLabsFlowController SauceLabsFlowController;

    static SauceryBase()
    {
        SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        SauceLabsFlowController = new SauceLabsFlowController();
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

        //DebugMessages.PrintPlatformDetails(platform);
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
            if (Driver != null)
            {
                var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                // log the result to SauceLabs
                var sessionId = Driver.SessionId.ToString();
                SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                Console.WriteLine("SessionID={0} job-name={1}", sessionId, _testName);
                Driver.Quit();
            }
        }
        catch (WebDriverException)
        {
            Console.WriteLine("Caught WebDriverException, quitting driver.");
            Driver?.Quit();
        }
    }

    private bool InitialiseDriver((DriverOptions opts, BrowserVersion browserVersion) tuple, int waitSecs)
    {
        SauceLabsFlowController.ControlFlow(tuple.browserVersion.IsARealDevice());
        try
        {
            Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, waitSecs);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public SauceryRemoteWebDriver SauceryDriver() => (SauceryRemoteWebDriver)Driver!;
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 12th July 2014
* 
*/