﻿using OpenQA.Selenium;
using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.Options;
using Saucery.Core.Util;
using System.Reflection;

namespace Saucery.XUnit;

public class SauceryXBase : XunitContextBase, IClassFixture<BaseFixture>
{
    protected readonly BaseFixture BaseFixture;
    private string? _testName;
    private BrowserVersion? _browserVersion;
    private readonly ITestOutputHelper _outputHelper;

    protected bool InitialiseDriver(BrowserVersion browserVersion)
    {
        _browserVersion = browserVersion;
        _browserVersion.SetTestName(GetTestName());
        _testName = _browserVersion.TestName;

        //DebugMessages.PrintPlatformDetails(platform);
        // set up the desired options
        BaseFixture.OptionFactory = new OptionFactory(_browserVersion);
        var tuple = BaseFixture.OptionFactory.CreateOptions(_testName!);

        var driverInitialised = BaseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);

        while (!driverInitialised)
        {
            Console.WriteLine($"Driver failed to initialise: {_testName}.");
            driverInitialised = BaseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        }
        Console.WriteLine($"Driver successfully initialised: {_testName}.");
        
        return driverInitialised;
    }

    public override void Dispose()
    {
        try
        {
            _testName = null;
            if (BaseFixture.Driver != null)
            {
                var passed = Context.TestException == null;
                // log the result to SauceLabs
                var sessionId = BaseFixture.Driver.SessionId.ToString();
                BaseFixture.SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                Console.WriteLine($"SessionID={sessionId} job-name={_testName}");
                BaseFixture.Driver.Quit();
                GC.SuppressFinalize(this);
            }
        }
        catch (WebDriverException)
        {
            Console.WriteLine("Caught WebDriverException, quitting driver.");
            BaseFixture.Driver?.Quit();
        }
    }

    protected SauceryXBase(ITestOutputHelper outputHelper, BaseFixture baseFixture) : base(outputHelper)
    {
        BaseFixture = baseFixture;
        _outputHelper = outputHelper;
    }

    private string GetTestName()
    {
        var type = _outputHelper.GetType();
        var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
        return ((ITest)testMember!.GetValue(_outputHelper)!).TestCase.TestMethod.Method.Name;
    }

    protected static IEnumerable<object[]> GetAllCombinations(object[] data) {
        List<object[]> allCombinations = [];

        foreach(var platform in SauceryTestData.Items)
        {
            allCombinations
                .AddRange(data
                    .Select(datum => (object[]) [platform, datum]));
        }

        return allCombinations;
    }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/