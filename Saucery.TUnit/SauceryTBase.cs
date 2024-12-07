using OpenQA.Selenium;
using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.Options;
using Saucery.Core.Util;
using TUnit.Core.Enums;

namespace Saucery.TUnit;

//public class SauceryTBase //: IClassFixture<BaseFixture>
public class SauceryTBase : BaseFixture
{
    private string? _testName;
    private BrowserVersion? _browserVersion;
    
    protected bool InitialiseDriver(BrowserVersion browserVersion)
    {
        _browserVersion = browserVersion;
        _browserVersion.SetTestName(GetTestName());
        _testName = _browserVersion.TestName;

        //DebugMessages.PrintPlatformDetails(platform);
        // set up the desired options
        OptionFactory = new OptionFactory(_browserVersion);
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
                var sessionId = Driver.SessionId.ToString();
                SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                Console.WriteLine($"SessionID={sessionId} job-name={_testName}");
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

    //protected SauceryTBase(ITestOutputHelper outputHelper, BaseFixture baseFixture) : base(outputHelper)
    //{
    //    _outputHelper = outputHelper;
    //}

    private static string GetTestName() => TestContext.Current?.TestDetails.TestName ?? "";

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
* Date: 7th December 2024
* 
*/