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

namespace Saucery.TUnit;

public class BaseFixture
{
    public WebDriver? Driver;
    
    internal readonly SauceLabsStatusNotifier SauceLabsStatusNotifier = new();
    
    private readonly SauceLabsFlowController SauceLabsFlowController = new();
    
    public OptionFactory? OptionFactory;

    private readonly AppiumClientConfig AppiumClientConfig = new() { DirectConnect = true };

    public bool InitialiseDriver((DriverOptions opts, BrowserVersion browserVersion) tuple, int waitSecs)
    {
        SauceLabsFlowController.ControlFlow(tuple.browserVersion.IsARealDevice());
        try
        {
            Driver = OptionFactory!.IsApple()
                ? new IOSDriver(
                    new Uri(SauceryConstants.SAUCELABS_HUB), 
                    tuple.opts, 
                    TimeSpan.FromSeconds(waitSecs),
                    AppiumClientConfig)
                : OptionFactory!.IsAndroid()
                    ? new AndroidDriver(
                        new Uri(SauceryConstants.SAUCELABS_HUB), 
                        tuple.opts, 
                        TimeSpan.FromSeconds(waitSecs),
                        AppiumClientConfig)
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

    public WebDriver SauceryDriver() => 
        Driver!;
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/