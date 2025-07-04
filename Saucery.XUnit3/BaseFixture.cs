﻿using OpenQA.Selenium;
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

//Needs XunitContext NuGet Package
//[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 4)]

namespace Saucery.XUnit3;

public class BaseFixture : IDisposable
{
    internal WebDriver? Driver;
    
    internal readonly SauceLabsEmulatedStatusNotifier SauceLabsEmulatedStatusNotifier = new();

    internal readonly SauceLabsRealDeviceStatusNotifier SauceLabsRealDeviceStatusNotifier = new();

    private readonly SauceLabsFlowController _sauceLabsFlowController = new();
    
    internal readonly SauceLabsRealDeviceAcquirer SauceLabsRealDeviceAcquirer = new();

    internal OptionFactory? OptionFactory;

    private readonly AppiumClientConfig _appiumClientConfig = new() { DirectConnect = true };

    public bool InitialiseDriver((DriverOptions opts, BrowserVersion browserVersion) tuple, int waitSecs)
    {
        _sauceLabsFlowController.ControlFlow(tuple.browserVersion.IsARealDevice());
        try
        {
            Driver = OptionFactory!.IsApple()
                ? new IOSDriver(
                    new Uri(SauceryConstants.SAUCELABS_HUB), 
                    tuple.opts, 
                    TimeSpan.FromSeconds(waitSecs),
                    _appiumClientConfig)
                : OptionFactory!.IsAndroid()
                    ? new AndroidDriver(
                        new Uri(SauceryConstants.SAUCELABS_HUB), 
                        tuple.opts, 
                        TimeSpan.FromSeconds(waitSecs),
                        _appiumClientConfig)
                    : new SauceryRemoteWebDriver(
                        new Uri(SauceryConstants.SAUCELABS_HUB), 
                        tuple.opts, 
                        waitSecs);

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
* Date: 31st May 2025
* 
*/