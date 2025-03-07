﻿using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.Options;
using Saucery.Core.Util;
using TUnit.Core.Enums;

namespace Saucery.TUnit;

public class SauceryTBase : BaseFixture
{
    private readonly Lock _lock = new();
    private string? _testName;
    private BrowserVersion? _browserVersion;

    protected bool InitialiseDriver(BrowserVersion browserVersion)
    {
        lock (browserVersion)
        {
            _browserVersion = browserVersion;
            browserVersion.SetTestName(GetTestName());
            _testName = browserVersion.TestName;
        }

        // set up the desired options
        OptionFactory = new OptionFactory(browserVersion);
        var tuple = OptionFactory.CreateOptions(_testName!);

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
                var passed = TestContext.Current?.Result?.Status == Status.Passed;
                // log the result to SauceLabs
                lock (_lock)
                {
                    if(_browserVersion!.IsARealDevice()) {
                        var realDeviceJobs = SauceLabsRealDeviceAcquirer.AcquireRealDeviceJobs();
                        var jobs = realDeviceJobs?.entities.FindAll(x => x.name.Equals(_browserVersion!.TestName));
                        foreach (var job in jobs!) {
                            SauceLabsRealDeviceStatusNotifier.NotifyRealDeviceStatus(job.id, passed);
                        }
                    } else {
                        SauceLabsEmulatedStatusNotifier.NotifyEmulatedStatus(Driver.SessionId.ToString(), passed);
                    }
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

    private static string GetTestName() => TestContext.Current?.TestDetails.TestName ?? "";
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/