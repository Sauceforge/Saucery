using MyTestProject.PageObjects;
using Saucery.Core.Dojo;
using Saucery.XUnit;
using Xunit.Abstractions;

namespace MyTestProject;

public class DataDrivenTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
{
    [Theory]
    [MemberData(nameof(AllCombinations))]
    public void DataDrivenTest(BrowserVersion requestedPlatform, int data) {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(BaseFixture.SauceryDriver(), "comments", data.ToString());
    }

    public static IEnumerable<object[]> AllCombinations() {
        var allPlatforms = RequestedPlatformData.AllPlatforms;
        return GetAllCombinations([4, 5]);
    }
}