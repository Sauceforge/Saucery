using MyTestProject.PageObjects;
using Saucery.Core.Dojo;
using Saucery.XUnit3;

namespace MyTestProject;

public class DataDrivenTests(ITestContextAccessor output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture) 
{
    [Theory]
    [MemberData(nameof(AllCombinations))]
    public void DataDrivenTest(BrowserVersion requestedPlatform, int data) {
        InitialiseDriver(requestedPlatform, _testContextAccessor.Current?.Test!);

        var guineaPigPage = new GuineaPigPage(_baseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(_baseFixture.SauceryDriver(), "comments", data.ToString());
    }

    public static IEnumerable<object[]> AllCombinations() {
        var allPlatforms = RequestedPlatformData.AllPlatforms;
        return GetAllCombinations([4, 5]);
    }
}