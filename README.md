﻿<h1 align="center">

<img src="/Saucery.Core/Images/Saucery.Core.png" alt="Saucery" width="200"/>
<br/>
Saucery
</h1>

<div align="center">
    
<b>Automated testing made more awesome</b>

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/0ba3cb69efe14366af8c84e485e80077)](https://app.codacy.com/gh/Sauceforge/Saucery/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
![Codacy coverage](https://img.shields.io/codacy/coverage/0ba3cb69efe14366af8c84e485e80077)
![Created](https://img.shields.io/github/created-at/sauceforge/saucery)

![GitHub Repo stars](https://img.shields.io/github/stars/Sauceforge/Saucery) 
[![GitHub Sponsors](https://img.shields.io/github/sponsors/Sauceforge)](https://github.com/sponsors/Sauceforge)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/Sauceforge/Saucery/pipeline.yml) 
![Commits](https://img.shields.io/github/commit-activity/t/sauceforge/saucery)
![GitHub last commit (master)](https://img.shields.io/github/last-commit/Sauceforge/Saucery/master)
![Website](https://img.shields.io/website?url=https%3A%2F%2Fsauceforge.github.io)
![License](https://img.shields.io/github/license/Sauceforge/Saucery) 

</div>

Saucery handles all the plumbing required to integrate with SauceLabs, making writing tests a breeze. so you only need to tell Saucery *what* you want. Saucery takes care of the *how*.

Saucery comes in multiple flavors supporting popular test frameworks.

Note: The tests specified below are provided as examples only. Your tests, of course, will be specific to your System Under Test.

### Packages

| Package | Badges |
| --- | --- |
| Saucery | [![nuget](https://img.shields.io/nuget/v/Saucery.svg)](https://www.nuget.org/packages/Saucery/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery)](https://www.nuget.org/packages/Saucery/) |
| Saucery.XUnit | [![nuget](https://img.shields.io/nuget/v/Saucery.XUnit.svg)](https://www.nuget.org/packages/Saucery.XUnit/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery.XUnit)](https://www.nuget.org/packages/Saucery.XUnit/) |
| Saucery.TUnit | [![nuget](https://img.shields.io/nuget/v/Saucery.TUnit.svg)](https://www.nuget.org/packages/Saucery.TUnit/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery.TUnit)](https://www.nuget.org/packages/Saucery.TUnit/) |
| Saucery.XUnit.v3 | [![nuget](https://img.shields.io/nuget/v/Saucery.XUnit.v3.svg)](https://www.nuget.org/packages/Saucery.XUnit.v3/)  [![NuGet Downloads](https://img.shields.io/nuget/dt/Saucery.XUnit.v3)](https://www.nuget.org/packages/Saucery.XUnit.v3/) |

### Sponsoring
Saucery has been developed as an open-source project for over 10 years. If you find it valuable for your projects and team work, please consider supporting it and becoming a  [![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/Sauceforge)

### Dog food Status

We test Saucery itself on SauceLabs!

[![Build Status](https://app.saucelabs.com/buildstatus/saucefauge?saucy)](https://app.saucelabs.com/buildstatus/saucefauge?saucy)

### Initial Setup

These steps apply to all flavors:

1. You'll need a SauceLabs account. You can get a free trial account [here](https://saucelabs.com/sign-up).
1. If you want to run your tests locally you need to set 2 environment variables, SAUCE_USER_NAME and SAUCE_API_KEY
1. To run your test suite from your GitHub Actions pipeline you need to set two secrets SAUCE_USER_NAME and SAUCE_API_KEY. Instructions on how to set Github Secrets are [here](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#creating-secrets-for-a-repository).

### NUnit

<img src="/Saucery/Images/Saucery.NUnit.png" alt="Saucery" width="100"/>

#### 🏁 Quick Start

#### Using the Project Template (Recommended)
```bash
cd Templates
dotnet new install .\NUnit
dotnet new saucery-nunit -n "MyTestProject"
```

Checkout our dogfood project ExternalMerlin.NUnit for more.

### Writing XUnit Tests

<img src="/Saucery.XUnit/Images/Saucery.XUnit.png" alt="Saucery.XUnit" width="100"/>

1. In your solution create a simple class library.
1. Add properties CopyLocalLockFileAssemblies and GenerateRuntimeConfigurationFiles to the top PropertyGroup of the project file and set them both to true.
1. Add a NuGet Reference to [Saucery.XUnit](https://www.nuget.org/packages/saucery.xunit) and [xunit.runner.visualstudio](https://www.nuget.org/packages/xunit.runner.visualstudio).

Your Project file should look something like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit.runner.visualstudio" Version="3.0.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Saucery.XUnit" Version="4.5.24" />
  </ItemGroup>

</Project>
```

The ExternalMerlin.XUnit dogfood integration tests use the following template:

```csharp
using ExternalMerlin.XUnit.PageObjects;
using Saucery.Core.Dojo;
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

#### XUnit Parallelism

- Parallelism in XUnit is currently achieved by having tests in multiple classes.
- The Level of Parallelism is determined by the number of parallel threads you have paid for in your SauceLabs account.
- Parallelism is optional so you can exclude `[assembly: CollectionBehavior(MaxParallelThreads = 5)]` lines if you wish. We recommend placing this line in a `Usings.cs` as it will apply to all your TestFixtures.

Next, let's break down the key line.

```csharp
public class DataDrivenTests(ITestOutputHelper output, BaseFixture baseFixture) : SauceryXBase(output, baseFixture)
```

Your class must subclass `SauceryXBase` and pass an `ITestOutputHelper` and a `BaseFixture`. SauceryX will take care of the rest.

You need to specify a class to tell SauceryX what platforms you want to test on. In this example its called `RequestedPlatformData` but you can call it anything you like.

Let's look at what it should contain.

```csharp
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
            //Real Devices
            new AndroidRealDevice("Google Pixel 8 Pro", "14"),
            new IOSRealDevice("iPhone 14 Pro Max", "16"),

            //Emulated Mobile Platforms
            new AndroidPlatform("Google Pixel 8 Pro GoogleAPI Emulator", "14.0", SauceryConstants.DEVICE_ORIENTATION_PORTRAIT),
            new IOSPlatform("iPhone 14 Pro Max Simulator", "16.2", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE),

            //Desktop Platforms
            new DesktopPlatform(SauceryConstants.PLATFORM_LINUX, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "75"),
            new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, "76", SauceryConstants.SCREENRES_2560_1600)
        ];

        SetPlatforms(platforms, PlatformFilter.Emulated);
    }

    public static IEnumerable<object[]> AllPlatforms => GetAllPlatforms();
}
```

The `List<SaucePlatform>` is what you will specify. The rest of the class is mandatory. Check out `SauceryConstants` for all the platform, browser and screenres enums.

### IDE Setup

If you are using Visual Studio 17.12 or earlier, follow the instructions [here](https://thomhurst.github.io/TUnit/docs/tutorial-basics/running-your-tests#visual-studio) to set up your IDE.

### Writing TUnit Tests

<img src="/Saucery.TUnit/Images/Saucery.TUnit.png" alt="Saucery.XUnit" width="100"/>

1. In your solution create a simple class library.
1. Add a NuGet Reference to [Saucery.TUnit](https://www.nuget.org/packages/Saucery.TUnit).

Your Project file should look something like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Saucery.TUnit" Version="0.16.4" />
  </ItemGroup>

</Project>
```

The ExternalMerlin.TUnit dogfood integration tests use the following template:

```csharp
using ExternalMerlin.TUnit.PageObjects;
using Saucery.Core.Dojo;
using Saucery.TUnit;

namespace ExternalMerlin.TUnit;

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

#### TUnit Parallelism

- Parallelism in TUnit is default out of the box. For SauceLabs it needs to be constrained. 
- Have a look at MyParallelLimit.cs in the ExternalMerlin.TUnit project for an example of how to do that.
- We recommend 2 less than your limit. Our OpenSauce account has 5 so we specify 3 in our internal testing.

The other lines are mandatory. Let's break the key lines down.

```csharp
public class DataDrivenTests : SauceryTBase
```

Your class must subclass `SauceryTBase`. SauceryT will take care of the rest.

A data driven test is specified like this:

```csharp
[Test]
[MethodDataSource(nameof(AllCombinations), Arguments = [new[] { 4, 5 }])]
public async Task DataDrivenTest(Func<BrowserVersion> requestedPlatform, int data)
```

You can call the class what you like but it must take a `Func<BrowserVersion>` and the `data` as a parameter and subclass `SauceryTBase`.

`[MethodDataSource(nameof(AllCombinations)...]` is how you tell SauceryT what platforms you want to test on. You need to specify a class to do that. In this example its called `RequestedPlatformData` but you can call it anything you like.

Let's look at what it should contain.

```csharp
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
}
```

The `List<SaucePlatform>` is what you will specify. The rest of the class is mandatory. Check out `SauceryConstants` for all the platform, browser and screenres enums.

## Platform Range Expansion

Platform range expansion is a feature unique to Saucery. Say you wanted to test on a range of browser versions but you didn't want to specify each individually. That's fine. Saucery supports specifying ranges.

```csharp
new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119")
```

This will test on Windows 11 Chrome all available versions from 100 to 119 inclusive.

## Real Devices

Yes, Saucery supports Real Devices!

### Trends

[Nuget downloads](https://nugettrends.com/packages?months=24&ids=Saucery&ids=Saucery.XUnit&ids=Saucery.TUnit&ids=Saucery.Core&ids=Saucery.XUnit.v3)  
[GitHub stars](https://star-history.com/#sauceforge/Saucery)

## Contact

Author: Andrew Gray  
Twitter: [@agrayz](https://twitter.com/agrayz)