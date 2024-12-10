using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;
using Shouldly;

namespace Merlin.TUnit;

public class DataDrivenTests : SauceryTBase
{
    [Test]
    [MethodDataSource(nameof(AllCombinations))]
    public async Task DataDrivenTest(DataDrivenData dataDrivenData)
    {
        InitialiseDriver(dataDrivenData.RequestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", dataDrivenData.Data.ToString());
    }

    public static List<Func<DataDrivenData>> AllCombinations()
    {
        var allPlatforms = RequestedPlatformData.AllPlatforms;

        return GetAllCombinationsAsFunc([4, 5])
            .ConvertAll<Func<DataDrivenData>>(func => () =>
            {
                var array = func();
                var requestedPlatform = (BrowserVersion)array[0];
                var data = (int)array[1];

                return new DataDrivenData
                {
                    RequestedPlatform = requestedPlatform,
                    Data = data
                };
            });
    }
}

public record DataDrivenData
{
    public BrowserVersion RequestedPlatform { get; set; }
    public int Data { get; set; }
}
/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/