using OpenQA.Selenium;
using Saucery.Core.Dojo;
using Saucery.Core.Options;
using Saucery.Core.RestAPI.TestStatus;
using Xunit;

//[assembly: CollectionBehavior(MaxParallelThreads = 4)]

namespace Saucery.XUnit;

public class SauceryXBase : IClassFixture<BaseFixture>, IDisposable
{
    protected readonly BaseFixture BaseFixture;
    private string _testName;
    private BrowserVersion _browserVersion;

    protected bool InitialiseDriver(BrowserVersion browserVersion)
    {
        _browserVersion = browserVersion;

        //_browserVersion.SetTestName(TestContext.CurrentContext.Test.Name);
        _browserVersion.SetTestName("_testName");
        _testName = _browserVersion.TestName;

        //DebugMessages.PrintPlatformDetails(platform);
        // set up the desired options
        var factory = new OptionFactory(_browserVersion);
        var opts = factory.CreateOptions(_testName);

        bool driverInitialised = BaseFixture.InitialiseDriver(opts, 400);

        while (!driverInitialised)
        {
            //Console.WriteLine($"Driver failed to initialise: {TestContext.CurrentContext.Test.Name}.");
            Console.WriteLine($"Driver failed to initialise: _testName.");
            driverInitialised = BaseFixture.InitialiseDriver(opts, 400);
        }
        //Console.WriteLine($"Driver successfully initialised: {TestContext.CurrentContext.Test.Name}.");
        Console.WriteLine($"Driver successfully initialised: _testName.");

        return driverInitialised;
    }

    protected SauceryXBase(BaseFixture baseFixture)
    {
        BaseFixture = baseFixture;
    }

    public void Dispose()
    {
        try
        {
            if (BaseFixture.Driver != null)
            {
                //var passed = Equals(TestContext.CurrentContext.Result.Outcome, ResultState.Success);
                var passed = true;
                // log the result to SauceLabs
                var sessionId = BaseFixture.Driver.GetSessionId();
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
}