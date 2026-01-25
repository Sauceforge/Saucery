using ExternalMerlin.XUnit.PageObjects;
using Saucery.Core.Dojo;
using Saucery.XUnit;
using Xunit.Abstractions;

namespace ExternalMerlin.XUnit;

public class DataDrivenTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
{
    [Theory]
    [MemberData(nameof(AllCombinations))]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data) {
        await InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(BaseFixture.SauceryDriver(), "comments", data.ToString());
    }

    public static IEnumerable<object[]> AllCombinations() {
        var allPlatforms = RequestedPlatformData.AllPlatforms;
        return GetAllCombinations([4, 5]);
    }
}