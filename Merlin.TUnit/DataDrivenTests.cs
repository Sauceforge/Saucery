using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.TUnit;
using Shouldly;
using System.Drawing.Drawing2D;

namespace Merlin.TUnit;

public class DataDrivenTests : SauceryTBase
{
    //[Test]
    //public async Task ComboTest()
    //{
    //    var combos = RequestedPlatformData.AllCombinations([4, 5]);
    //    var count = combos.Count;
    //    Console.WriteLine($"combos: {count}");
    //}


    [Test]
    //[MethodDataSource(nameof(AllCombinations))]
    //[MethodDataSource(typeof(RequestedPlatformData), nameof(RequestedPlatformData.AllCombinations([4,5])))]
    
    //public async Task DataDrivenTest(DataDrivenData dataDrivenData)
    public async Task DataDrivenTest(
        [Matrix(nameof(RequestedPlatformData.AllPlatformsAsList))] BrowserVersion requestedPlatform,
        [Matrix(4, 5)] int data)
    {
        //InitialiseDriver(dataDrivenData.RequestedPlatform);
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        //guineaPigPage.TypeField(SauceryDriver(), "comments", dataDrivenData.Data.ToString());
        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());
    }
}

/*
* Copyright Andrew Gray, SauceForge
* Date: 7th December 2024
* 
*/