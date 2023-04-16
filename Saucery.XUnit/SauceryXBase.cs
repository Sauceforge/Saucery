using Saucery.Core.Dojo;
using Saucery.Core.Driver;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.RestAPI.TestStatus;
using Xunit;

namespace Saucery.XUnit;


public class SauceryXBase : IClassFixture<ParallelTests>
{
    private string _testName;
    protected SauceryRemoteWebDriver Driver;
    private readonly BrowserVersion _browserVersion;
    private static readonly SauceLabsStatusNotifier SauceLabsStatusNotifier;
    private static readonly SauceLabsFlowController SauceLabsFlowController;

    static SauceryXBase()
    {
        SauceLabsStatusNotifier = new SauceLabsStatusNotifier();
        SauceLabsFlowController = new SauceLabsFlowController();
    }

    protected SauceryXBase(BrowserVersion browserVersion)
    {
        _browserVersion = browserVersion;
    }
}