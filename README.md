<h1 align="center">

<img src="https://raw.githubusercontent.com/SauceForge/Saucery/master/Saucery.Core/Images/Saucery.Core.png" alt="Saucery" width="200"/>
<br/>
Saucery
</h1>

<div align="center">
    
<b>Automated testing made more awesome</b>

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=for-the-badge)](https://raw.githubusercontent.com/SauceForge/Saucery/master/LICENSE)
[![CI-CD](https://img.shields.io/github/actions/workflow/status/SauceForge/Saucery/pipeline.yml?style=for-the-badge)](https://github.com/SauceForge/Saucery/actions/workflows/pipeline.yml)

</div>

Saucery handles all the plumbing required to integrate with SauceLabs, making writing tests a breeze. Saucery comes in multiple flavors supporting popular test frameworks.

### Dog food Status
We test Saucery itself on SauceLabs!

[![Build Status](https://app.saucelabs.com/buildstatus/saucefauge)](https://app.saucelabs.com/u/saucefauge)


## Getting Started

Saucery takes care of the plumbing required to talk to SauceLabs, so you only need to tell Saucery *what* you want. Saucery takes care of the *how*.

Your tests, of course, will be specific to your System Under Test. The ones specified below are provided as examples only.

### Initial Setup

These steps apply to all flavors:
1. You'll need a SauceLabs account. You can get a free trial account [here](https://saucelabs.com/sign-up).
1. If you want to run your tests locally you need to set 2 environment variables, SAUCE_USER_NAME and SAUCE_API_KEY
1. To run your test suite from your GitHub Actions pipeline you need to set two secrets SAUCE_USER_NAME and SAUCE_API_KEY. Instructions on how to set Github Secrets are [here](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#creating-secrets-for-a-repository).

### NUnit

1. In your solution create a simple class library.
1. Add a NuGet Reference to [Saucery](https://www.nuget.org/packages/Saucery).
1. Start with the following template:

```
using NUnit.Framework;
using Saucery;
using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Shouldly;

[assembly: LevelOfParallelism(4)]

namespace ExternalMerlin.NUnit;

[TestFixture]
[Parallelizable]
[TestFixtureSource(typeof(RequestedPlatformData))]
public class NuGetIntegrationTests(BrowserVersion browserVersion) : SauceryBase(browserVersion) 
{
    [Test]
    [TestCase(5)]
    [TestCase(4)]
    public void DataDrivenTitleTest(int data) {
        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());
        Driver?.Title.ShouldContain("I am a page title - Sauce Labs");
    }

    [Test]
    public void ClickLinkTest() {
        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");
    
        // find and click the link on the page
        guineaPigPage.ClickLink(SauceryDriver());

        // verify the browser was navigated to the correct page
        Driver?.Url.ShouldContain("saucelabs.com/test-guinea-pig2.html");
    }
}

```

The above code will run *3* unit tests (1 ClickLink test and 2 DataDrivenTitle tests) on *all* the platforms you specify, in parallel.

The Level of Parallelism is determined by the number of parallel threads you have paid for in your SauceLabs account.

We recommend 1 less than your limit. Our OpenSauce account has 5 so we specify 4 in our internal testing.

Parallism is optional so you can exclude the `[assembly: LevelOfParallelism(4)]` and `[Parallelizable]` lines if you wish.

The other lines are mandatory. Let's break the key lines down.

```
[TestFixture]
[Parallelizable]
[TestFixtureSource(typeof(RequestedPlatformData))]
public class NuGetIntegrationTests(BrowserVersion browserVersion) : SauceryBase(browserVersion)
```

You can call the class what you like but it must take a `BrowserVersion` as a parameter and subclass `SauceryBase`.

`[TestFixtureSource(typeof(RequestedPlatformData))]` is how you tell Saucery what platforms you want to test on. You need to specify a class to do that. In this example its called `RequestedPlatformData` but you can call it anything you like.

Let's look at what it should contain.

```
using Saucery.Core.DataSources;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace ExternalMerlin.NUnit;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        List<SaucePlatform> platforms =
        [
            //Mobile Platforms
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "14.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 14 Pro Max Simulator", "16.2", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "76", SauceryConstants.SCREENRES_2560_1600)
        ];

        SetPlatforms(platforms);
    }
}
```

The `List<SaucePlatform>` is what you will specify. The rest of the class is mandatory. Check out `SauceryConstants` for all the platform, browser and screenres enums.

### XUnit

1. In your solution create a simple class library.
1. Add a NuGet Reference to [Saucery.XUnit](https://www.nuget.org/packages/saucery.xunit)
1. Start with the following template:

```
using Saucery.Core.Dojo;
using Saucery.Tests.Common.PageObjects;
using Saucery.XUnit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(MaxParallelThreads = 5)]

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

    public static IEnumerable<object[]> AllCombinations => GetAllCombinations([4, 5]);
}
```

The above code will run *2* unit tests (2 DataDrivenTitle tests) on *all* the platforms you specify.

Parallelism in XUnit is currently achieved by having tests in multiple classes.

The Level of Parallelism is determined by the number of parallel threads you have paid for in your SauceLabs account.

Parallism is optional so you can exclude `[assembly: CollectionBehavior(MaxParallelThreads = 5)]` lines if you wish. We recommend placing this line in a `Usings.cs` as it will apply to all your TestFixtures.

Next, let's break down the key line.

```
public class DataDrivenTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
```

Your class must subclass `SauceryXBase` and pass an `ITestOutputHelper` and a `BaseFixture`. SauceryX will take care of the rest.

You need to specify a class to tell SauceryX what platforms you want to test on. In this example its called `RequestedPlatformData` but you can call it anything you like.

Let's look at what it should contain.

```
using Saucery.Core.DataSources;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace ExternalMerlin.XUnit;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        List<SaucePlatform> platforms =
        [
            //Mobile Platforms
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "14.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 14 Pro Max Simulator", "16.2", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "76", SauceryConstants.SCREENRES_2560_1600)
        ];

        SetPlatforms(platforms);
    }

    public static IEnumerable<object[]> AllPlatforms => GetAllPlatforms();
}
```

The `List<SaucePlatform>` is what you will specify. The rest of the class is mandatory. Check out `SauceryConstants` for all the platform, browser and screenres enums.

## Flavors
### Saucery
[![NuGet version (Saucery)](https://img.shields.io/nuget/v/Saucery.svg?style=for-the-badge)](https://www.nuget.org/packages/Saucery/)

### Saucery.XUnit
[![NuGet version (Saucery)](https://img.shields.io/nuget/v/Saucery.XUnit.svg?style=for-the-badge)](https://www.nuget.org/packages/Saucery.XUnit/)

## Resources
### Download statistics
![Nuget](https://img.shields.io/nuget/dt/Saucery.svg?label=Saucery%40nuget&style=for-the-badge)
![Nuget](https://img.shields.io/nuget/dt/Saucery.XUnit.svg?label=Saucery.XUnit%40nuget&style=for-the-badge)

### Trends
[Nuget downloads](https://nugettrends.com/packages?months=24&ids=Saucery&ids=Saucery.XUnit)  
[GitHub stars](https://star-history.com/#sauceforge/Saucery)

## Contact
Author: Andrew Gray  
Twitter: [@agrayz](https://twitter.com/agrayz)