# Saucery

Saucery handles all the plumbing required to integrate with SauceLabs, making writing NUnit tests a breeze, so you only need to tell Saucery *what* you want. Saucery takes care of the *how*.

Note: The tests specified below are provided as examples only. Your tests, of course, will be specific to your System Under Test.

## Initial Setup

1. You'll need a SauceLabs account. You can get a free trial account [here](https://saucelabs.com/sign-up).
1. If you want to run your tests locally you need to set 2 environment variables, SAUCE_USER_NAME and SAUCE_API_KEY
1. To run your test suite from your GitHub Actions pipeline you need to set two secrets SAUCE_USER_NAME and SAUCE_API_KEY. Instructions on how to set Github Secrets are [here](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions#creating-secrets-for-a-repository).

## Writing NUnit Tests

1. In your solution create a simple class library.
1. Add properties CopyLocalLockFileAssemblies and GenerateRuntimeConfigurationFiles to the top PropertyGroup of the project file and set them both to true.
1. Add a NuGet Reference to [Saucery](https://www.nuget.org/packages/Saucery) and [NUnit3TestAdapter](https://www.nuget.org/packages/NUnit3TestAdapter).

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
    <PackageReference Include="NUnit3TestAdapter" Version="5.0.0" />
    <PackageReference Include="Saucery" Version="4.5.21" />
  </ItemGroup>

</Project>
```

The ExternalMerlin.NUnit dogfood integration tests use the following template:

```csharp
    using ExternalMerlin.XUnit.PageObjects;
    using NUnit.Framework;
    using Saucery;
    using Saucery.Core.Dojo;
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
        public void DataDrivenTest(int data) {
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

### Parallelism

- The Level of Parallelism is determined by the number of parallel threads you have paid for in your SauceLabs account.
- We recommend 1 less than your limit. Our OpenSauce account has 5 so we specify 4 in our internal testing.
- Parallism is optional so you can exclude the `[assembly: LevelOfParallelism(4)]` and `[Parallelizable]` lines if you wish.

The other lines are mandatory. Let's break the key lines down.

```csharp
    [TestFixture]
    [Parallelizable]
    [TestFixtureSource(typeof(RequestedPlatformData))]
    public class NuGetIntegrationTests(BrowserVersion browserVersion) : SauceryBase(browserVersion)
```

You can call the class what you like but it must take a `BrowserVersion` as a parameter and subclass `SauceryBase`.

`[TestFixtureSource(typeof(RequestedPlatformData))]` is how you tell Saucery what platforms you want to test on. You need to specify a class to do that. In this example its called `RequestedPlatformData` but you can call it anything you like.

Let's look at what it should contain.

```csharp
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