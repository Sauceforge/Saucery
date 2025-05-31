using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit3;
using Xunit.Sdk;

namespace Merlin.XUnit3;

public class DataDrivenTests(ITestContextAccessor context, BaseFixture baseFixture) : SauceryXBase(context, baseFixture)
{
    [Theory]
    [MemberData(nameof(AllCombinations))]
    public void DataDrivenTest(BrowserVersion requestedPlatform, int data, ITest test)
    {
        InitialiseDriver(requestedPlatform, test);

        var guineaPigPage = new GuineaPigPage(_baseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(_baseFixture.SauceryDriver(), "comments", data.ToString());
    }
    
    public static IEnumerable<object[]> AllCombinations() {
        _ = RequestedPlatformData.AllPlatforms;
        return GetAllCombinations([4, 5]);
    }
}