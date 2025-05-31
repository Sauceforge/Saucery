using OpenQA.Selenium;
using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.Options;
using Saucery.Core.Util;
using Xunit;
using Xunit.Sdk;

namespace Saucery.XUnit3;

public class SauceryXBase : IClassFixture<BaseFixture>, IDisposable {
    private readonly Lock _testStatusLock = new();
    protected readonly BaseFixture _baseFixture;
    private string? _testName;
    private BrowserVersion? _browserVersion;
    private readonly ITestContextAccessor _testContextAccessor;

    protected SauceryXBase(ITestContextAccessor contextAccessor, BaseFixture baseFixture) {
        _testContextAccessor = contextAccessor;
        _baseFixture = baseFixture;
        
    }

    protected void InitialiseDriver(BrowserVersion browserVersion, ITest test) {
        lock(browserVersion) {
            _browserVersion = browserVersion;
            browserVersion.SetTestName(test.TestDisplayName);
            _testName = browserVersion.TestName;
        }

        // set up the desired options
        _baseFixture.OptionFactory = new OptionFactory(browserVersion);
        var tuple = _baseFixture.OptionFactory.CreateOptions(_testName!);

        var driverInitialised = _baseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);

        while(!driverInitialised) {
            Console.WriteLine($"Driver failed to initialise: {_testName}.");
            driverInitialised = _baseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        }
        Console.WriteLine($"Driver successfully initialised: {_testName}.");
    }

    public void Dispose() {
        try {
            _testName = null;
            if(_baseFixture.Driver != null) {
                var passed = _testContextAccessor.Current?.TestState?.Result == TestResult.Passed;
                // log the result to SauceLabs


                if(_browserVersion!.IsARealDevice()) {
                    lock(_testStatusLock) {
                        var realDeviceJobs = _baseFixture.SauceLabsRealDeviceAcquirer.AcquireRealDeviceJobs();
                        var jobs = realDeviceJobs?.entities.FindAll(x => x.name.Equals(_browserVersion!.TestName));
                        foreach(var job in jobs!) {
                            _baseFixture.SauceLabsRealDeviceStatusNotifier.NotifyRealDeviceStatus(job.id, passed);
                        }
                    }
                } else {
                    _baseFixture.SauceLabsEmulatedStatusNotifier.NotifyEmulatedStatus(_baseFixture.Driver.SessionId.ToString(), passed);
                }


                _baseFixture.Driver.Quit();
                GC.SuppressFinalize(this);
            }
        } catch(WebDriverException) {
            Console.WriteLine("Caught WebDriverException, quitting driver.");
            _baseFixture.Driver?.Quit();
        }
    }

    protected static IEnumerable<object[]> GetAllCombinations(object[] data) {
        List<object[]> allCombinations = [];

        foreach(var platform in SauceryTestData.Items) {
            allCombinations
                .AddRange(data
                    .Select(datum => (object[])[platform, datum]));
        }

        return allCombinations;
    }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 31st May 2025
* 
*/