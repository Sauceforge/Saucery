using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Service;
using Saucery.Core.Dojo;
using Saucery.Core.Driver;
using Saucery.Core.Options;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.RestAPI.TestStatus;
using Saucery.Core.Util;

namespace Saucery;

public class SauceryBase
{
    private string? _testName;
    protected WebDriver? Driver;
    private readonly BrowserVersion _browserVersion;
    private static readonly SauceLabsStatusNotifier SauceLabsStatusNotifier;
    private static readonly SauceLabsFlowController SauceLabsFlowController;
    private OptionFactory? _optionFactory;
    private readonly AppiumClientConfig AppiumClientConfig = new() { DirectConnect = true };

    static SauceryBase()
    {
        SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        SauceLabsFlowController = new SauceLabsFlowController();
    }

    protected SauceryBase(BrowserVersion browserVersion)
    {
        _browserVersion = browserVersion;
    }

    [SetUp]
    public void Setup()
    {
        _browserVersion.SetTestName(TestContext.CurrentContext.Test.Name);
        _testName = _browserVersion.TestName;

        //DebugMessages.PrintPlatformDetails(platform);
        // set up the desired options
        _optionFactory = new OptionFactory(_browserVersion);
        var opts = _optionFactory.CreateOptions(_testName!);

        bool driverInitialised = InitialiseDriver(opts!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);

        while (!driverInitialised)
        {
            Console.WriteLine($"Driver failed to initialise: {TestContext.CurrentContext.Test.Name}.");
            driverInitialised = InitialiseDriver(opts!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        }
        Console.WriteLine($"Driver successfully initialised: {TestContext.CurrentContext.Test.Name}.");
    }

    [TearDown]
    public void Cleanup()
    {
        try
        {
            if (Driver != null)
            {
                var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                // log the result to SauceLabs
                var sessionId = Driver.SessionId.ToString();
                SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                Console.WriteLine(@"SessionID={0} job-name={1}", sessionId, _testName);
                Driver.Quit();
            }
        }
        catch (WebDriverException)
        {
            Console.WriteLine(@"Caught WebDriverException, quitting driver.");
            Driver?.Quit();
        }
    }

    private bool InitialiseDriver(DriverOptions opts, int waitSecs)
    {
        SauceLabsFlowController.ControlFlow();
        try
        {
            if (_optionFactory!.IsApple())
            {
               Driver = new IOSDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts, TimeSpan.FromSeconds(waitSecs), AppiumClientConfig);
            }
            else
            {
                if (_optionFactory!.IsAndroid())
                {
                    Driver = new AndroidDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts, TimeSpan.FromSeconds(waitSecs), AppiumClientConfig);
                }
                else
                {
                    Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts, waitSecs);
                }
            }
            
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