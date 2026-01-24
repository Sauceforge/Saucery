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
    protected readonly BaseFixture _baseFixture;
    private string? _testName;
    private BrowserVersion? _browserVersion;
    protected readonly ITestContextAccessor _testContextAccessor;

    protected SauceryXBase(ITestContextAccessor contextAccessor, BaseFixture baseFixture) {
        _testContextAccessor = contextAccessor;
        _baseFixture = baseFixture;

    }

    protected async Task InitialiseDriver(BrowserVersion browserVersion, ITest test) {
        _browserVersion = browserVersion;
        _testName = BrowserVersion.GenerateTestName(browserVersion, test.TestCase.TestMethod?.MethodName!);

        // set up the desired options
        _baseFixture.OptionFactory = new OptionFactory(browserVersion);
        var tuple = _baseFixture.OptionFactory.CreateOptions(_testName);

        var driverInitialised = await _baseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);

        while(!driverInitialised) {
            Console.WriteLine($"Driver failed to initialise: {_testName}.");
            driverInitialised = await _baseFixture.InitialiseDriver(tuple!, SauceryConstants.SELENIUM_COMMAND_TIMEOUT);
        }
        Console.WriteLine($"Driver successfully initialised: {_testName}.");
    }

    public async void Dispose() {
        try {
            _testName = null;
            if(_baseFixture.Driver != null) {
                var passed = _testContextAccessor.Current?.TestState?.Result == TestResult.Passed;
                // log the result to SauceLabs

                if(_browserVersion!.IsARealDevice()) {
                    var realDeviceJobs = await _baseFixture.SauceLabsRealDeviceAcquirer.AcquireRealDeviceJobs();
                    var jobs = realDeviceJobs?.entities.FindAll(x => x.name.Equals(_testName));
                    foreach(var job in jobs!) {
                        _baseFixture.SauceLabsRealDeviceStatusNotifier.NotifyRealDeviceStatus(job.id, passed);
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
