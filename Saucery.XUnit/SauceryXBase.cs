using System.Reflection;
using OpenQA.Selenium;
using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.Options;
using Saucery.Core.Util;

namespace Saucery.XUnit;

public class SauceryXBase : XunitContextBase, IClassFixture<BaseFixture>
{
    private readonly Lock _testStatusLock = new();
    protected readonly BaseFixture BaseFixture;
    private string? _testName;
    private readonly ITestOutputHelper _outputHelper;
    private BrowserVersion? _browserVersion;

    protected SauceryXBase(ITestOutputHelper outputHelper, BaseFixture baseFixture) : base(outputHelper) {
        BaseFixture = baseFixture;
        _outputHelper = outputHelper;
    }

    protected void InitialiseDriver(BrowserVersion browserVersion)
    {
        lock (browserVersion)
        {
            _browserVersion = browserVersion;
            browserVersion.SetTestName(GetTestName());
            _testName = browserVersion.TestName;
        }

        // set up the desired options
        BaseFixture.OptionFactory = new OptionFactory(browserVersion);
        var tuple = BaseFixture.OptionFactory.CreateOptions(_testName!);

        var driverInitialised = BaseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);

        while (!driverInitialised)
        {
            Console.WriteLine($"Driver failed to initialise: {_testName}.");
            driverInitialised = BaseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        }
        Console.WriteLine($"Driver successfully initialised: {_testName}.");
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
                
                
                    if(_browserVersion!.IsARealDevice()) {
                        lock (_testStatusLock)
                        {
                            var realDeviceJobs = BaseFixture.SauceLabsRealDeviceAcquirer.AcquireRealDeviceJobs();
                            var jobs = realDeviceJobs?.entities.FindAll(x => x.name.Equals(_browserVersion!.TestName));
                            foreach (var job in jobs!)
                            {
                                BaseFixture.SauceLabsRealDeviceStatusNotifier.NotifyRealDeviceStatus(job.id, passed);
                            }
                        }
                    } else {
                        BaseFixture.SauceLabsEmulatedStatusNotifier.NotifyEmulatedStatus(BaseFixture.Driver.SessionId.ToString(), passed);
                    }
                

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