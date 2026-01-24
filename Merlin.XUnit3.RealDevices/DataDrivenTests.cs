using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit3;

namespace Merlin.XUnit3.RealDevices;

public class DataDrivenTests(ITestContextAccessor context, BaseFixture baseFixture) : SauceryXBase(context, baseFixture)
{
    [Theory]
    [MemberData(nameof(AllCombinations))]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        await InitialiseDriver(requestedPlatform, _testContextAccessor.Current?.Test!);

        var guineaPigPage = new GuineaPigPage(_baseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(_baseFixture.SauceryDriver(), "comments", data.ToString());
    }
    
    public static IEnumerable<object[]> AllCombinations() {
        _ = RequestedPlatformData.AllPlatforms;
        return GetAllCombinations([4, 5]);
    }
}