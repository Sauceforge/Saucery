using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.Options;
using Saucery.Core.Util;
using TUnit.Core.Enums;

namespace Saucery.TUnit;

public class SauceryTBase : BaseFixture
{
    private string? _testName;
    private BrowserVersion? _browserVersion;

    protected bool InitialiseDriver(BrowserVersion browserVersion)
    {
        _browserVersion = browserVersion;
        _testName = BrowserVersion.GenerateTestName(browserVersion, GetTestName());

        // set up the desired options
        OptionFactory = new OptionFactory(browserVersion);
        var tuple = OptionFactory.CreateOptions(_testName);

        var driverInitialised = InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);

        while (!driverInitialised)
        {
            Console.WriteLine($"Driver failed to initialise: {_testName}.");
            driverInitialised = InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        }
        Console.WriteLine($"Driver successfully initialised: {_testName}.");

        return driverInitialised;
    }

    [After(Test)]
    public void TearDown()
    {
        try
        {
            _testName = null;
            if (Driver != null)
            {
                var passed = TestContext.Current?.Execution.Result?.State == TestState.Passed;
                // log the result to SauceLabs
                if(_browserVersion!.IsARealDevice()) {
                    var realDeviceJobs = SauceLabsRealDeviceAcquirer.AcquireRealDeviceJobs();
                    var jobs = realDeviceJobs?.entities.FindAll(x => x.name.Equals(_testName));
                    foreach (var job in jobs!) {
                        SauceLabsRealDeviceStatusNotifier.NotifyRealDeviceStatus(job.id, passed);
                    }
                } else {
                    SauceLabsEmulatedStatusNotifier.NotifyEmulatedStatus(Driver.SessionId.ToString(), passed);
                }

                Driver.Quit();
                Driver.Dispose();
                GC.SuppressFinalize(this);
                OptionFactory?.Dispose();
            }
        }
        catch (WebDriverException)
        {
            Console.WriteLine("Caught WebDriverException, quitting driver.");
            Driver?.Quit();
        }
    }

    private static string GetTestName() => TestContext.Current?.Metadata.TestDetails.TestName ?? "";
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/