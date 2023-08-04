using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Saucery.Core.Dojo;
using Saucery.Core.Options;
using System.Reflection;

//[assembly: CollectionBehavior(MaxParallelThreads = 4)]

namespace Saucery.XUnit;

public class SauceryXBase : XunitContextBase, IClassFixture<BaseFixture>
{
    protected readonly BaseFixture BaseFixture;
    private string _testName;
    private BrowserVersion _browserVersion;
    private readonly ITestOutputHelper _outputHelper;

    protected bool InitialiseDriver(BrowserVersion browserVersion)
    {
        _browserVersion = browserVersion;

        _browserVersion.SetTestName(GetTestName());
        _testName = _browserVersion.TestName;

        //DebugMessages.PrintPlatformDetails(platform);
        // set up the desired options
        var factory = new OptionFactory(_browserVersion);
        var opts = factory.CreateOptions(_testName);

        bool driverInitialised = BaseFixture.InitialiseDriver(opts, 400);

        while (!driverInitialised)
        {
            Console.WriteLine($"Driver failed to initialise: {_testName}.");
            driverInitialised = BaseFixture.InitialiseDriver(opts, 400);
        }
        Console.WriteLine($"Driver successfully initialised: {_testName}.");
        
        return driverInitialised;
    }

    public override void Dispose()
    {
        try
        {
            if (BaseFixture.Driver != null)
            {
                var passed = Context.TestException == null;
                // log the result to SauceLabs
                var sessionId = ((RemoteWebDriver)BaseFixture.Driver).SessionId; //BaseFixture.Driver.GetSessionId();
                BaseFixture.SauceLabsStatusNotifier.NotifyStatus(sessionId, passed);
                Console.WriteLine(@"SessionID={0} job-name={1}", sessionId, _testName);
                BaseFixture.Driver.Quit();
            }
        }
        catch (WebDriverException)
        {
            Console.WriteLine(@"Caught WebDriverException, quitting driver.");
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
        return ((ITest)testMember.GetValue(_outputHelper)).TestCase.TestMethod.Method.Name;
    }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 16th April 2023
* 
*/