using System.Text.RegularExpressions;
using Xunit;
using static Saucery.CoverageGapRemediator.Tests.FixtureRepository;

namespace Saucery.CoverageGapRemediator.Tests;

/// <summary>
/// Adding a platform Sauce offers but Saucery.Core does not declare: Core registrations, product
/// classes (including isArmRequired), and the same tests in both Core test projects.
/// </summary>
public sealed class AdditionTests
{
    private const string Products = "Saucery.Core/Dojo/Platforms/ConcreteProducts";
    private const string Creators = "Saucery.Core/Dojo/Platforms/ConcreteCreators";
    private const string BrowserCreators = "Saucery.Core/Dojo/Browsers/ConcreteCreators/PC";
    private const string Dojo = "Saucery.Core/Dojo/DojoExtensions.cs";

    // ---------------------------------------------------------------- simulators / emulators

    [Fact]
    public void iOS_simulator_version_is_registered_as_a_simulator_only()
    {
        using var repo = Add("iOS", "27.1");

        Assert.Contains("public sealed class IOS271Platform", repo.Read($"{Products}/Apple/IOS271Platform.cs"));
        Assert.Contains("new IOS271Platform(Platform)", repo.Read($"{Creators}/Apple/IOS271PlatformCreator.cs"));
        Assert.Contains("\"iOS 27.1\" => platforms.GetPlatform<IOS271Platform>()", repo.Read(Dojo));

        var factory = repo.Read("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs");
        Assert.Contains("\"27.1\" => new IOS271PlatformCreator(sp).Create(),", factory);
        Assert.Equal(1, Count(factory, "IOS271PlatformCreator"));
    }

    [Theory]
    [InlineData(TUnit)]
    [InlineData(XUnit)]
    public void iOS_simulator_version_gets_simulator_tests_only(string project)
    {
        using var repo = Add("iOS", "27.1");

        var types = $"{project}/REST/PlatformTypes.cs";
        Assert.Contains("typeof(IOS271Platform)", repo.Member(types, "SupportedPlatformTypes"));
        Assert.DoesNotContain("IOS271Platform", repo.Member(types, "SupportedRealDeviceTypes"));
    }

    [Theory]
    [InlineData(TUnit)]
    [InlineData(XUnit)]
    public void Simulator_version_list_gains_the_version_only_when_it_already_tests_the_sibling(string project)
    {
        // The list tests 16.2 but stops short of 27.0 (later simulators use other device names).
        using var after164 = Add("iOS", "16.4");
        Assert.Contains("\"16.0\",\"16.2\",\"16.4\",", after164.Read($"{project}/IOSFactoryVersionTests.cs"));

        using var after271 = Add("iOS", "27.1");
        Assert.DoesNotContain("27.1", after271.Read($"{project}/IOSFactoryVersionTests.cs"));
    }

    [Theory]
    [InlineData(TUnit)]
    [InlineData(XUnit)]
    public void Android_emulator_version_is_registered_with_a_test_row(string project)
    {
        using var repo = Add("Android", "5.0");

        Assert.True(repo.Exists($"{Products}/Google/Android5Platform.cs"));
        Assert.Contains("\"Linux 5.0\" => platforms.GetPlatform<Android5Platform>()", repo.Read(Dojo));
        Assert.Contains("\"5.0\" => new Android5PlatformCreator(sp).Create(),", repo.Read("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs"));
        Assert.Contains("new AndroidPlatform(\"Google Pixel 3 Emulator\", \"5.0\"", repo.Read($"{project}/AndroidFactoryVersionTests.cs"));
        Assert.Contains("typeof(Android5Platform)", repo.Member($"{project}/REST/PlatformTypes.cs", "SupportedPlatformTypes"));
    }

    // ---------------------------------------------------------------- real devices

    [Fact]
    public void Real_device_major_is_registered_as_a_real_device_only()
    {
        using var repo = Add("iOS", "28.x", devices: ["iPhone 17 Pro"]);

        Assert.True(repo.Exists($"{Products}/Apple/IOS28Platform.cs"));
        var dojo = repo.Read(Dojo);
        Assert.Contains("\"iOS 28\" => platforms.GetPlatform<IOS28Platform>()", dojo);
        Assert.DoesNotContain("\"iOS 28.0\"", dojo);

        var factory = repo.Read("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs");
        Assert.Contains("\"28\" => new IOS28PlatformCreator(sp).Create(),", factory);
        Assert.DoesNotContain("\"28.0\"", factory);
    }

    [Theory]
    [InlineData(TUnit)]
    [InlineData(XUnit)]
    public void Real_device_major_gets_real_device_tests_using_a_device_sauce_reports(string project)
    {
        using var repo = Add("iOS", "28.x", devices: ["iPhone 17 Pro"]);

        var types = $"{project}/REST/PlatformTypes.cs";
        Assert.Contains("typeof(IOS28Platform)", repo.Member(types, "SupportedRealDeviceTypes"));
        Assert.DoesNotContain("IOS28Platform", repo.Member(types, "SupportedPlatformTypes"));
        Assert.Contains("\"27\", \"28\"", repo.Read($"{project}/RealIOSFactoryVersionTests.cs"));
        Assert.Contains("new IOSRealDevice(\"iPhone 17 Pro\", \"28\")", repo.Read($"{project}/DataProviders/PlatformDataClass.cs"));
    }

    [Fact]
    public void Real_device_major_reuses_the_classes_its_simulators_already_have()
    {
        // Re-adding iOS 14 real devices: IOS14Platform already exists for the 14.0 simulator.
        using var repo = new FixtureRepository();
        var remove = new CoverageDiscrepancy(DiscrepancyType.ObsoleteCandidate, "iOS", PlatformRequirementKind.MobileMajorWildcard, "iOS 14.x", "14.x");
        ObsoleteSynthesizer.Apply(repo.Context, null!, remove, repo.NewChanges());
        var product = repo.Read($"{Products}/Apple/IOS14Platform.cs");

        AdditionSynthesizer.Apply(repo.Context, Missing("iOS", "14.x", devices: ["iPhone 12"]), repo.NewChanges());

        Assert.Equal(product, repo.Read($"{Products}/Apple/IOS14Platform.cs"));
        Assert.Contains("\"iOS 14\" => platforms.GetPlatform<IOS14Platform>()", repo.Read(Dojo));
        Assert.Equal(1, Count(repo.Read(Dojo), "\"iOS 14.0\""));
    }

    // ---------------------------------------------------------------- desktop

    [Fact]
    public void Windows_version_gets_only_the_browsers_sauce_offers_on_it()
    {
        using var repo = Add("Windows", "8", browsers: ["chrome", "firefox", "internet explorer"]);

        var product = repo.Read($"{Products}/PC/Windows8Platform.cs");
        Assert.Contains("SauceryConstants.PLATFORM_WINDOWS_8,", product);
        Assert.Contains("[SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_IE]", product);
        Assert.DoesNotContain("BROWSER_EDGE", product);

        var browserCreator = repo.Read($"{BrowserCreators}/Windows8BrowserCreator.cs");
        Assert.Contains("SauceryConstants.BROWSER_IE => new IEBrowser", browserCreator);
        Assert.DoesNotContain("EdgeBrowser", browserCreator);
    }

    [Fact]
    public void Windows_version_is_registered_everywhere_without_redefining_its_constant()
    {
        using var repo = Add("Windows", "8", browsers: ["chrome", "firefox", "internet explorer"]);

        Assert.Contains("SauceryConstants.PLATFORM_WINDOWS_8 => platforms.GetPlatform<Windows8Platform>()", repo.Read(Dojo));
        Assert.Contains("SauceryConstants.PLATFORM_WINDOWS_8 => new Windows8PlatformCreator(sp).Create(),", repo.Read("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs"));
        Assert.Contains("SauceryConstants.PLATFORM_WINDOWS_8 => new Windows8BrowserCreator(sp).Create(SauceryConstants.PLATFORM_WINDOWS_8,", repo.Read("Saucery.Core/Dojo/Browsers/BrowserFactory.cs"));
        Assert.Equal(1, Count(repo.Read("Saucery.Core/Util/SauceryConstants.cs"), "PLATFORM_WINDOWS_8 ="));
    }

    [Fact]
    public void Desktop_sibling_is_one_that_already_supports_every_browser_sauce_offers()
    {
        // Windows 8 has no Edge; Windows 8.1 does. Cloning 8.1 from the nearer Windows 8 could
        // only narrow its browsers, so it must be modelled on Windows 10.
        using var repo = Add("Windows", "8", browsers: ["chrome", "firefox", "internet explorer"]);
        AdditionSynthesizer.Apply(repo.Context, Missing("Windows", "8.1", browsers: ["chrome", "firefox", "internet explorer", "MicrosoftEdge"]), repo.NewChanges());

        var product = repo.Read($"{Products}/PC/Windows81Platform.cs");
        Assert.Contains("SauceryConstants.BROWSER_EDGE", product);
        Assert.Contains("EdgeBrowser", repo.Read($"{BrowserCreators}/Windows81BrowserCreator.cs"));
    }

    [Theory]
    [InlineData(TUnit)]
    [InlineData(XUnit)]
    public void Desktop_version_gets_platform_and_browser_tests(string project)
    {
        using var repo = Add("Windows", "8", browsers: ["firefox", "internet explorer"]);

        var types = $"{project}/REST/PlatformTypes.cs";
        Assert.Contains("typeof(Windows8Platform)", repo.Member(types, "SupportedPlatformTypes"));
        Assert.Contains("typeof(Windows8Platform)", repo.Member(types, "PlatformsWithBrowsersTypes"));
        Assert.DoesNotContain("Windows8Platform", repo.Member(types, "SupportedRealDeviceTypes"));
        // Chrome is not offered here, so the supported row uses the first browser that is.
        Assert.Contains(
            "new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_CURRENT_MAJOR)",
            repo.Read($"{project}/DesktopFactoryVersionTests.cs"));
    }

    [Fact]
    public void Mac_version_adds_its_constant_and_configurator_entry()
    {
        using var repo = Add("macOS", "27", browsers: ["chrome", "firefox", "MicrosoftEdge", "safari"]);

        Assert.Equal(1, Count(repo.Read("Saucery.Core/Util/SauceryConstants.cs"), "public const string PLATFORM_MAC_27 = \"Mac 27\";"));
        Assert.Contains("SauceryConstants.PLATFORM_MAC_27,", repo.Read("Saucery.Core/Dojo/PlatformConfigurator.cs"));
        Assert.Contains("new Mac27BrowserCreator(sp).Create(\"macOS 27\",", repo.Read("Saucery.Core/Dojo/Browsers/BrowserFactory.cs"));
        Assert.Contains("\"macOS 27\",", repo.Read($"{Products}/PC/Mac27Platform.cs"));
    }

    // ---------------------------------------------------------------- isArmRequired

    [Theory]
    [InlineData("iOS", "16.4", "Apple/IOS164Platform.cs", true)]   // sibling iOS 16.2 has no ARM
    [InlineData("iOS", "27.1", "Apple/IOS271Platform.cs", true)]   // sibling iOS 27.0 has ARM
    [InlineData("macOS", "27", "PC/Mac27Platform.cs", true)]       // sibling Mac 26 has ARM
    [InlineData("macOS", "13", "PC/Mac13Platform.cs", false)]      // sibling Mac 26 has ARM
    [InlineData("Android", "5.0", "Google/Android5Platform.cs", false)]
    public void Arm_requirement_comes_from_the_new_version_not_the_sibling(string family, string version, string product, bool armRequired)
    {
        using var repo = Add(family, version, browsers: family == "macOS" ? ["chrome", "firefox", "MicrosoftEdge", "safari"] : []);

        var source = repo.Read($"{Products}/{product}");
        Assert.Equal(armRequired, Regex.IsMatch(source, @",\s*true\s*\)"));
    }

    // ---------------------------------------------------------------- sync and idempotency

    [Theory]
    [InlineData("iOS", "27.1")]
    [InlineData("iOS", "28.x")]
    [InlineData("Windows", "8")]
    public void Both_test_projects_receive_the_same_tests(string family, string version)
    {
        using var repo = Add(family, version, devices: ["iPhone 17 Pro"], browsers: ["chrome", "firefox", "internet explorer"]);

        Assert.Equal(repo.Read($"{TUnit}/REST/PlatformTypes.cs"), repo.Read($"{XUnit}/REST/PlatformTypes.cs"));
        Assert.Equal(repo.Read($"{TUnit}/DataProviders/PlatformDataClass.cs"), repo.Read($"{XUnit}/DataProviders/PlatformDataClass.cs"));
        Assert.Equal(repo.Read($"{TUnit}/IOSFactoryVersionTests.cs"), repo.Read($"{XUnit}/IOSFactoryVersionTests.cs"));
    }

    [Fact]
    public void Adding_the_same_platform_twice_changes_nothing_the_second_time()
    {
        using var repo = Add("iOS", "16.4");
        var files = Directory.EnumerateFiles(repo.Root, "*.cs", SearchOption.AllDirectories)
            .ToDictionary(f => f, File.ReadAllText);

        AdditionSynthesizer.Apply(repo.Context, Missing("iOS", "16.4"), repo.NewChanges());

        Assert.All(files, f => Assert.Equal(f.Value, File.ReadAllText(f.Key)));
        Assert.Equal(files.Count, Directory.EnumerateFiles(repo.Root, "*.cs", SearchOption.AllDirectories).Count());
    }

    // ---------------------------------------------------------------- helpers

    private static FixtureRepository Add(string family, string version, string[]? devices = null, string[]? browsers = null)
    {
        var repo = new FixtureRepository();
        AdditionSynthesizer.Apply(repo.Context, Missing(family, version, devices, browsers), repo.NewChanges());
        return repo;
    }

    private static CoverageDiscrepancy Missing(string family, string version, string[]? devices = null, string[]? browsers = null)
    {
        var kind = family is "iOS" or "Android"
            ? version.EndsWith(".x", StringComparison.Ordinal) ? PlatformRequirementKind.MobileMajorWildcard : PlatformRequirementKind.MobileExactVersion
            : PlatformRequirementKind.DesktopPlatform;

        return new CoverageDiscrepancy(
            DiscrepancyType.Missing, family, kind, $"{family} {version}", version,
            RepresentativeDevices: devices ?? [],
            SauceBrowsers: browsers ?? []);
    }

    private static int Count(string text, string value) =>
        Regex.Matches(text, Regex.Escape(value)).Count;
}
