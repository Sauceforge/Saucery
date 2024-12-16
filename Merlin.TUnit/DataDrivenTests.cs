using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;
using Shouldly;

namespace Merlin.TUnit;

public class DataDrivenTests : SauceryTBase
{
    [Test]
    [MethodDataSource(nameof(AllCombinations))]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());
    }

    public IEnumerable<(BrowserVersion, int)> AllCombinations() =>
        from browserVersion 
            in SauceryTestData.Items 
        from data in new[] { 4, 5 } 
        select (browserVersion, data);

    //public static List<Func<DataDrivenData>> AllCombinations()
    //{
    //    var allPlatforms = RequestedPlatformData.AllPlatforms;

    //    return GetAllCombinationsAsFunc([4, 5])
    //        .ConvertAll<Func<DataDrivenData>>(func => () =>
    //        {
    //            var array = func();
    //            var requestedPlatform = (BrowserVersion)array[0];
    //            var data = (int)array[1];

    //            return new DataDrivenData
    //            {
    //                RequestedPlatform = requestedPlatform,
    //                Data = data
    //            };
    //        });
    //}
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/