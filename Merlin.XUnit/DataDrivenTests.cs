using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Xunit.Abstractions;

namespace Merlin.XUnit;

public class DataDrivenTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
{
    [Theory]
    [MemberData(nameof(AllCombinations))]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        await InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(BaseFixture.SauceryDriver(), "comments", data.ToString());
    }
    
    public static IEnumerable<object[]> AllCombinations() {
        _ = RequestedPlatformData.AllPlatforms;
        return GetAllCombinations([4, 5]);
    }
}