using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Xunit.Abstractions;

namespace ExternalMerlin.XUnit;

public class DataDrivenTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
{
    [Theory]
    [MemberData(nameof(AllCombinations))]
    public void DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(BaseFixture.SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(BaseFixture.SauceryDriver(), "comments", data.ToString());
    }

    public static IEnumerable<object[]> AllCombinations
    {
        get
        {
            List<object[]> allCombinations = [];

            foreach (var platform in RequestedPlatformData.Items)
            {
                allCombinations.Add([platform, 4]);
                allCombinations.Add([platform, 5]);
            }

            return from c in allCombinations
                   select c;
        }
    }
}