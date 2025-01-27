# Saucery

Saucery handles all the plumbing required to integrate with SauceLabs, making writing XUnit tests a breeze, so you only need to tell Saucery *what* you want. Saucery takes care of the *how*.

## Getting Started

The tests specified below are provided as examples only. Your tests, of course, will be specific to your System Under Test.

### Initial Setup

1. You'll need a SauceLabs account. You can get a free trial account [here](https://saucelabs.com/sign-up).
1. If you want to run your tests locally you need to set 2 environment variables, SAUCE_USER_NAME and SAUCE_API_KEY
1. To run your test suite from your GitHub Actions pipeline you need to set two secrets SAUCE_USER_NAME and SAUCE_API_KEY. Instructions on how to set Github Secrets are [here](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#creating-secrets-for-a-repository).

### XUnit

<img src="/Saucery.XUnit/Images/Saucery.XUnit.png" alt="Saucery.XUnit" width="100"/>

1. In your solution create a simple class library.
1. Add properties CopyLocalLockFileAssemblies and GenerateRuntimeConfigurationFiles to the top PropertyGroup of the project file and set them both to true.
1. Add a NuGet Reference to [Saucery.XUnit](https://www.nuget.org/packages/saucery.xunit) and [xunit.runner.visualstudio](https://www.nuget.org/packages/xunit.runner.visualstudio).

Your Project file should look something like this:

```
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
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
	<PackageReference Include="Saucery.XUnit" Version="4.5.7" />
  </ItemGroup>

</Project>
```

The ExternalMerlin.XUnit dogfood integration tests use the following template:

```
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

#### Parallelism

- Parallelism in XUnit is currently achieved by having tests in multiple classes.
- The Level of Parallelism is determined by the number of parallel threads you have paid for in your SauceLabs account.
- Parallism is optional so you can exclude `[assembly: CollectionBehavior(MaxParallelThreads = 5)]` lines if you wish. We recommend placing this line in a `Usings.cs` as it will apply to all your TestFixtures.

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

## Platform Range Expansion
Platform range expansion is a feature unique to Saucery. Say you wanted to test on a range of browser versions but you didn't want to specify each individually. That's fine. Saucery supports specifying ranges.

```
    new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_11, SauceryConstants.BROWSER_CHROME, "100->119")
```

This will test on Windows 11 Chrome all available versions from 100 to 119 inclusive.

## Real Devices
Yes, Saucery supports Real Devices!