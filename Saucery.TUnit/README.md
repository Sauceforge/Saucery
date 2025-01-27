# Saucery

Saucery handles all the plumbing required to integrate with SauceLabs, making writing TUnit tests a breeze, so you only need to tell Saucery *what* you want. Saucery takes care of the *how*.

## Getting Started

The tests specified below are provided as examples only. Your tests, of course, will be specific to your System Under Test.

### Initial Setup

1. You'll need a SauceLabs account. You can get a free trial account [here](https://saucelabs.com/sign-up).
1. If you want to run your tests locally you need to set 2 environment variables, SAUCE_USER_NAME and SAUCE_API_KEY
1. To run your test suite from your GitHub Actions pipeline you need to set two secrets SAUCE_USER_NAME and SAUCE_API_KEY. Instructions on how to set Github Secrets are [here](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#creating-secrets-for-a-repository).

### TUnit

<img src="/Saucery.TUnit/Images/Saucery.TUnit.png" alt="Saucery.XUnit" width="100"/>

1. In your solution create a simple class library.
1. Add a NuGet Reference to [Saucery.TUnit](https://www.nuget.org/packages/Saucery.TUnit).

Your Project file should look something like this:

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Saucery.TUnit" Version="0.6.0" />
  </ItemGroup>

</Project>

```

#### IDE Setup
Follow the instructions [here](https://thomhurst.github.io/TUnit/docs/tutorial-basics/running-your-tests#visual-studio) to set up your IDE.

The ExternalMerlin.TUnit dogfood integration tests use the following template:

```
using ExternalMerlin.XUnit.PageObjects;
using Saucery.Core.Dojo;
using Saucery.TUnit;

namespace Merlin.TUnit.RealDevices;

public class DataDrivenTests : SauceryTBase
{
    [Test]
    [MethodDataSource(nameof(AllCombinations), Arguments = [new[] { 4, 5 }])]
    public async Task DataDrivenTest(BrowserVersion requestedPlatform, int data)
    {
        InitialiseDriver(requestedPlatform);

        var guineaPigPage = new GuineaPigPage(SauceryDriver(), "https://saucelabs.com/");

        guineaPigPage.TypeField(SauceryDriver(), "comments", data.ToString());

        var commentField = guineaPigPage.GetField(SauceryDriver(), "comments");
        await Assert.That(commentField).IsNotNull();

        var commentText = commentField.GetDomProperty("value");
        await Assert.That(commentText).Contains(data.ToString());
    }

    public static IEnumerable<Func<(BrowserVersion, int)>> AllCombinations(int[] data) =>
        RequestedPlatformData
        .AllPlatforms()
        .SelectMany(
            browserVersionFunc => data,
            (browserVersionFunc, datum) => new Func<(BrowserVersion, int)>(() => (browserVersionFunc(), datum))
        );
}
```

The above code will run *2* unit tests (2 DataDrivenTitle tests) on *all* the platforms you specify, in parallel by default.

#### Parallelism

- Parallelism in TUnit is default out of the box. For SauceLabs it needs to be constrained. 
- Have a look at MyParallelLimit.cs in the ExternalMerlin.TUnit project for an example of how to do that.
- We recommend 2 less than your limit. Our OpenSauce account has 5 so we specify 3 in our internal testing.

The other lines are mandatory. Let's break the key lines down.

```
public class DataDrivenTests : SauceryTBase
```

Your class must subclass `SauceryTBase`. SauceryT will take care of the rest.

A data driven test is specified like this:

```
[Test]
[MethodDataSource(nameof(AllCombinations), Arguments = [new[] { 4, 5 }])]
public async Task DataDrivenTest(Func<BrowserVersion> requestedPlatform, int data)
```

You can call the class what you like but it must take a `Func<BrowserVersion>` and the `data` as a parameter and subclass `SauceryTBase`.

`[MethodDataSource(nameof(AllCombinations)...]` is how you tell SauceryT what platforms you want to test on. You need to specify a class to do that. In this example its called `RequestedPlatformData` but you can call it anything you like.

Let's look at what it should contain.

```
using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.OnDemand;
using Saucery.Core.OnDemand.Base;
using Saucery.Core.Util;

namespace ExternalMerlin.TUnit;

public class RequestedPlatformData : SauceryTestData
{
    static RequestedPlatformData()
    {
        var platforms = new List<SaucePlatform>
        {
            //Emulated Mobile Platforms
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "15.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 14 Pro Max Simulator", "16.2", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "123"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "124", SauceryConstants.SCREENRES_2560_1600)
        };

        SetPlatforms(platforms, PlatformFilter.Emulated);
    }

    public static List<Func<BrowserVersion>> AllPlatforms() => GetAllPlatformsAsFunc();
```

The `List<SaucePlatform>` is what you will specify. The rest of the class is mandatory. Check out `SauceryConstants` for all the platform, browser and screenres enums.

## Platform Range Expansion
Platform range expansion is a feature unique to Saucery. Say you wanted to test on a range of browser versions but you didn't want to specify each individually. That's fine. Saucery supports specifying ranges.

```
    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119")
```

This will test on Windows 11 Chrome all available versions from 100 to 119 inclusive.

## Real Devices
Yes, Saucery supports Real Devices!