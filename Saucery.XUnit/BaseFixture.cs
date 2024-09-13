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
using System;

//Needs XunitContext NuGet Package
//[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 4)]

namespace Saucery.XUnit;

public class BaseFixture : IDisposable
{
    public WebDriver? Driver;
    
    internal readonly SauceLabsStatusNotifier SauceLabsStatusNotifier;
    
    private readonly SauceLabsFlowController SauceLabsFlowController;
    
    public OptionFactory? OptionFactory;

    private readonly AppiumClientConfig AppiumClientConfig = new() { DirectConnect = true };

    public BaseFixture()
    {
        SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        SauceLabsFlowController = new SauceLabsFlowController();
    }

    public bool InitialiseDriver((DriverOptions opts, BrowserVersion browserVersion) tuple, int waitSecs)
    {
        SauceLabsFlowController.ControlFlow(tuple.browserVersion.IsARealDevice());
        try
        {
            Driver = OptionFactory!.IsApple()
                ? new IOSDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, TimeSpan.FromSeconds(waitSecs),
                    AppiumClientConfig)
                : OptionFactory!.IsAndroid()
                    ? new AndroidDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, TimeSpan.FromSeconds(waitSecs),
                        AppiumClientConfig)
                    : new SauceryRemoteWebDriver(new Uri(SauceryConstants.SAUCELABS_HUB), tuple.opts, waitSecs);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        if (Driver is not null)
        {
            Driver.Quit();
            Driver.Dispose();
        }

        OptionFactory?.Dispose();
    }

    public WebDriver SauceryDriver() => 
        Driver!;
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/