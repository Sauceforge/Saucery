using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Xunit.Abstractions;

namespace Merlin.XUnit;

public class DataDrivenTests : SauceryXBase
{
    public DataDrivenTests(ITestOutputHelper output, BaseFixture baseFixture) : base(output, baseFixture)
    {
    }

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
            List<object[]> allCombinations = new();

            //.ToList() needed to avoid InvalidOperationException
            //Collection was modified; enumeration operation may not execute.
            foreach (var platform in RequestedPlatformData.Items.ToList())
            {
                allCombinations.Add(new object[] { platform, 4 });
                allCombinations.Add(new object[] { platform, 5 });
            }

            foreach (var c in allCombinations)
            {
                yield return c;
            }
        }
    }
}