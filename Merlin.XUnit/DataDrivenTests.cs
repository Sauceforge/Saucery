﻿using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Xunit.Abstractions;

namespace Merlin.XUnit;

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
            List<object[]> allCombinations =
            [
                //.ToList() needed to avoid InvalidOperationException: Collection was modified; enumeration operation may not execute.
                .. from platform
                   in RequestedPlatformData.Items.ToList()
                   select new object[] { platform, 4 },
            ];
            
            return from c 
                   in allCombinations
                   select c;
        }
    }
}