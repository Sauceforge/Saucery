using OpenQA.Selenium;
using Saucery.Core.Driver;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.RestAPI.TestStatus;
using Saucery.Core.Util;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 4)]

namespace Saucery.XUnit;

public class BaseFixture : IDisposable
{
    public SauceryRemoteWebDriver? Driver;
    internal static readonly SauceLabsStatusNotifier SauceLabsStatusNotifier;
    private static readonly SauceLabsFlowController SauceLabsFlowController;

    static BaseFixture()
    {
        SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        SauceLabsFlowController = new SauceLabsFlowController();
    }

    public bool InitialiseDriver(DriverOptions opts, int waitSecs)
    {
        SauceLabsFlowController.ControlFlow();
        try
        {
            Driver = new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), opts, waitSecs);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public void Dispose()
    {
        if (Driver is not null)
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/