using System.Text.RegularExpressions;
using Shouldly;
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

    [Test]
    public void iOS_simulator_version_is_registered_as_a_simulator_only()
    {
        using var repo = Add("iOS", "27.1");

        repo.Read($"{Products}/Apple/IOS271Platform.cs").ShouldContain("public sealed class IOS271Platform", Case.Sensitive);
        repo.Read($"{Creators}/Apple/IOS271PlatformCreator.cs").ShouldContain("new IOS271Platform(Platform)", Case.Sensitive);
        repo.Read(Dojo).ShouldContain("\"iOS 27.1\" => platforms.GetPlatform<IOS271Platform>()", Case.Sensitive);

        var factory = repo.Read("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs");
        factory.ShouldContain("\"27.1\" => new IOS271PlatformCreator(sp).Create(),", Case.Sensitive);
        Count(factory, "IOS271PlatformCreator").ShouldBe(1);
    }

    [Test]
    [Arguments(TUnitProject)]
    [Arguments(XUnitProject)]
    public void iOS_simulator_version_gets_simulator_tests_only(string project)
    {
        using var repo = Add("iOS", "27.1");

        var types = $"{project}/REST/PlatformTypes.cs";
        repo.Member(types, "SupportedPlatformTypes").ShouldContain("typeof(IOS271Platform)", Case.Sensitive);
        repo.Member(types, "SupportedRealDeviceTypes").ShouldNotContain("IOS271Platform", Case.Sensitive);
    }

    [Test]
    [Arguments(TUnitProject)]
    [Arguments(XUnitProject)]
    public void Simulator_version_list_gains_the_version_only_when_it_already_tests_the_sibling(string project)
    {
        // The list tests 16.2 but stops short of 27.0 (later simulators use other device names).
        using var after164 = Add("iOS", "16.4");
        after164.Read($"{project}/IOSFactoryVersionTests.cs").ShouldContain("\"16.0\",\"16.2\",\"16.4\",", Case.Sensitive);

        using var after271 = Add("iOS", "27.1");
        after271.Read($"{project}/IOSFactoryVersionTests.cs").ShouldNotContain("27.1", Case.Sensitive);
    }

    [Test]
    [Arguments(TUnitProject)]
    [Arguments(XUnitProject)]
    public void Android_emulator_version_is_registered_with_a_test_row(string project)
    {
        using var repo = Add("Android", "5.0");

        repo.Exists($"{Products}/Google/Android5Platform.cs").ShouldBeTrue();
        repo.Read(Dojo).ShouldContain("\"Linux 5.0\" => platforms.GetPlatform<Android5Platform>()", Case.Sensitive);
        repo.Read("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs")
            .ShouldContain("\"5.0\" => new Android5PlatformCreator(sp).Create(),", Case.Sensitive);
        repo.Read($"{project}/AndroidFactoryVersionTests.cs")
            .ShouldContain("new AndroidPlatform(\"Google Pixel 3 Emulator\", \"5.0\"", Case.Sensitive);
        repo.Member($"{project}/REST/PlatformTypes.cs", "SupportedPlatformTypes").ShouldContain("typeof(Android5Platform)", Case.Sensitive);
    }

    // ---------------------------------------------------------------- real devices

    [Test]
    public void Real_device_major_is_registered_as_a_real_device_only()
    {
        using var repo = Add("iOS", "28.x", devices: ["iPhone 17 Pro"]);

        repo.Exists($"{Products}/Apple/IOS28Platform.cs").ShouldBeTrue();
        var dojo = repo.Read(Dojo);
        dojo.ShouldContain("\"iOS 28\" => platforms.GetPlatform<IOS28Platform>()", Case.Sensitive);
        dojo.ShouldNotContain("\"iOS 28.0\"", Case.Sensitive);

        var factory = repo.Read("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs");
        factory.ShouldContain("\"28\" => new IOS28PlatformCreator(sp).Create(),", Case.Sensitive);
        factory.ShouldNotContain("\"28.0\"", Case.Sensitive);
    }

    [Test]
    [Arguments(TUnitProject)]
    [Arguments(XUnitProject)]
    public void Real_device_major_gets_real_device_tests_using_a_device_sauce_reports(string project)
    {
        using var repo = Add("iOS", "28.x", devices: ["iPhone 17 Pro"]);

        var types = $"{project}/REST/PlatformTypes.cs";
        repo.Member(types, "SupportedRealDeviceTypes").ShouldContain("typeof(IOS28Platform)", Case.Sensitive);
        repo.Member(types, "SupportedPlatformTypes").ShouldNotContain("IOS28Platform", Case.Sensitive);
        repo.Read($"{project}/RealIOSFactoryVersionTests.cs").ShouldContain("\"27\", \"28\"", Case.Sensitive);
        repo.Read($"{project}/DataProviders/PlatformDataClass.cs")
            .ShouldContain("new IOSRealDevice(\"iPhone 17 Pro\", \"28\")", Case.Sensitive);
    }

    [Test]
    public void Real_device_major_reuses_the_classes_its_simulators_already_have()
    {
        // Re-adding iOS 14 real devices: IOS14Platform already exists for the 14.0 simulator.
        using var repo = new FixtureRepository();
        var remove = new CoverageDiscrepancy(DiscrepancyType.ObsoleteCandidate, "iOS", PlatformRequirementKind.MobileMajorWildcard, "iOS 14.x", "14.x");
        ObsoleteSynthesizer.Apply(repo.Context, null!, remove, repo.NewChanges());
        var product = repo.Read($"{Products}/Apple/IOS14Platform.cs");

        AdditionSynthesizer.Apply(repo.Context, Missing("iOS", "14.x", devices: ["iPhone 12"]), repo.NewChanges());

        repo.Read($"{Products}/Apple/IOS14Platform.cs").ShouldBe(product);
        repo.Read(Dojo).ShouldContain("\"iOS 14\" => platforms.GetPlatform<IOS14Platform>()", Case.Sensitive);
        Count(repo.Read(Dojo), "\"iOS 14.0\"").ShouldBe(1);
    }

    // ---------------------------------------------------------------- desktop

    [Test]
    public void Windows_version_gets_only_the_browsers_sauce_offers_on_it()
    {
        using var repo = Add("Windows", "8", browsers: ["chrome", "firefox", "internet explorer"]);

        var product = repo.Read($"{Products}/PC/Windows8Platform.cs");
        product.ShouldContain("SauceryConstants.PLATFORM_WINDOWS_8,", Case.Sensitive);
        product.ShouldContain("[SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_IE]", Case.Sensitive);
        product.ShouldNotContain("BROWSER_EDGE", Case.Sensitive);

        var browserCreator = repo.Read($"{BrowserCreators}/Windows8BrowserCreator.cs");
        browserCreator.ShouldContain("SauceryConstants.BROWSER_IE => new IEBrowser", Case.Sensitive);
        browserCreator.ShouldNotContain("EdgeBrowser", Case.Sensitive);
    }

    [Test]
    public void Windows_version_is_registered_everywhere_without_redefining_its_constant()
    {
        using var repo = Add("Windows", "8", browsers: ["chrome", "firefox", "internet explorer"]);

        repo.Read(Dojo).ShouldContain("SauceryConstants.PLATFORM_WINDOWS_8 => platforms.GetPlatform<Windows8Platform>()", Case.Sensitive);
        repo.Read("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs")
            .ShouldContain("SauceryConstants.PLATFORM_WINDOWS_8 => new Windows8PlatformCreator(sp).Create(),", Case.Sensitive);
        repo.Read("Saucery.Core/Dojo/Browsers/BrowserFactory.cs")
            .ShouldContain("SauceryConstants.PLATFORM_WINDOWS_8 => new Windows8BrowserCreator(sp).Create(SauceryConstants.PLATFORM_WINDOWS_8,", Case.Sensitive);
        Count(repo.Read("Saucery.Core/Util/SauceryConstants.cs"), "PLATFORM_WINDOWS_8 =").ShouldBe(1);
    }

    [Test]
    public void Desktop_sibling_is_one_that_already_supports_every_browser_sauce_offers()
    {
        // Windows 8 has no Edge; Windows 8.1 does. Cloning 8.1 from the nearer Windows 8 could
        // only narrow its browsers, so it must be modelled on Windows 10.
        using var repo = Add("Windows", "8", browsers: ["chrome", "firefox", "internet explorer"]);
        AdditionSynthesizer.Apply(repo.Context, Missing("Windows", "8.1", browsers: ["chrome", "firefox", "internet explorer", "MicrosoftEdge"]), repo.NewChanges());

        repo.Read($"{Products}/PC/Windows81Platform.cs").ShouldContain("SauceryConstants.BROWSER_EDGE", Case.Sensitive);
        repo.Read($"{BrowserCreators}/Windows81BrowserCreator.cs").ShouldContain("EdgeBrowser", Case.Sensitive);
    }

    [Test]
    [Arguments(TUnitProject)]
    [Arguments(XUnitProject)]
    public void Desktop_version_gets_platform_and_browser_tests(string project)
    {
        using var repo = Add("Windows", "8", browsers: ["firefox", "internet explorer"]);

        var types = $"{project}/REST/PlatformTypes.cs";
        repo.Member(types, "SupportedPlatformTypes").ShouldContain("typeof(Windows8Platform)", Case.Sensitive);
        repo.Member(types, "PlatformsWithBrowsersTypes").ShouldContain("typeof(Windows8Platform)", Case.Sensitive);
        repo.Member(types, "SupportedRealDeviceTypes").ShouldNotContain("Windows8Platform", Case.Sensitive);
        // Chrome is not offered here, so the supported row uses the first browser that is.
        repo.Read($"{project}/DesktopFactoryVersionTests.cs").ShouldContain(
            "new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_8, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_VERSION_CURRENT_MAJOR)",
            Case.Sensitive);
    }

    [Test]
    public void Mac_version_adds_its_constant_and_configurator_entry()
    {
        using var repo = Add("macOS", "27", browsers: ["chrome", "firefox", "MicrosoftEdge", "safari"]);

        Count(repo.Read("Saucery.Core/Util/SauceryConstants.cs"), "public const string PLATFORM_MAC_27 = \"Mac 27\";").ShouldBe(1);
        repo.Read("Saucery.Core/Dojo/PlatformConfigurator.cs").ShouldContain("SauceryConstants.PLATFORM_MAC_27,", Case.Sensitive);
        repo.Read("Saucery.Core/Dojo/Browsers/BrowserFactory.cs").ShouldContain("new Mac27BrowserCreator(sp).Create(\"macOS 27\",", Case.Sensitive);
        repo.Read($"{Products}/PC/Mac27Platform.cs").ShouldContain("\"macOS 27\",", Case.Sensitive);
    }

    // ---------------------------------------------------------------- isArmRequired

    [Test]
    [Arguments("iOS", "16.4", "Apple/IOS164Platform.cs", true)]   // sibling iOS 16.2 has no ARM
    [Arguments("iOS", "27.1", "Apple/IOS271Platform.cs", true)]   // sibling iOS 27.0 has ARM
    [Arguments("macOS", "27", "PC/Mac27Platform.cs", true)]       // sibling Mac 26 has ARM
    [Arguments("macOS", "13", "PC/Mac13Platform.cs", false)]      // sibling Mac 26 has ARM
    [Arguments("Android", "5.0", "Google/Android5Platform.cs", false)]
    public void Arm_requirement_comes_from_the_new_version_not_the_sibling(string family, string version, string product, bool armRequired)
    {
        using var repo = Add(family, version, browsers: family == "macOS" ? ["chrome", "firefox", "MicrosoftEdge", "safari"] : []);

        Regex.IsMatch(repo.Read($"{Products}/{product}"), @",\s*true\s*\)").ShouldBe(armRequired);
    }

    // ---------------------------------------------------------------- sync and idempotency

    [Test]
    [Arguments("iOS", "27.1")]
    [Arguments("iOS", "28.x")]
    [Arguments("Windows", "8")]
    public void Both_test_projects_receive_the_same_tests(string family, string version)
    {
        using var repo = Add(family, version, devices: ["iPhone 17 Pro"], browsers: ["chrome", "firefox", "internet explorer"]);

        repo.Read($"{XUnitProject}/REST/PlatformTypes.cs").ShouldBe(repo.Read($"{TUnitProject}/REST/PlatformTypes.cs"));
        repo.Read($"{XUnitProject}/DataProviders/PlatformDataClass.cs").ShouldBe(repo.Read($"{TUnitProject}/DataProviders/PlatformDataClass.cs"));
        repo.Read($"{XUnitProject}/IOSFactoryVersionTests.cs").ShouldBe(repo.Read($"{TUnitProject}/IOSFactoryVersionTests.cs"));
    }

    [Test]
    public void Adding_the_same_platform_twice_changes_nothing_the_second_time()
    {
        using var repo = Add("iOS", "16.4");
        var files = Directory.EnumerateFiles(repo.Root, "*.cs", SearchOption.AllDirectories)
            .ToDictionary(f => f, File.ReadAllText);

        AdditionSynthesizer.Apply(repo.Context, Missing("iOS", "16.4"), repo.NewChanges());

        foreach (var (path, before) in files)
            File.ReadAllText(path).ShouldBe(before, $"{path} changed on the second addition");
        Directory.EnumerateFiles(repo.Root, "*.cs", SearchOption.AllDirectories).Count().ShouldBe(files.Count);
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
            ? version.EndsWith(".x", StringComparison.Ordinal) 
                ? PlatformRequirementKind.MobileMajorWildcard 
                : PlatformRequirementKind.MobileExactVersion
            : PlatformRequirementKind.DesktopPlatform;

        return new CoverageDiscrepancy(
            DiscrepancyType.Missing, family, kind, $"{family} {version}", version,
            RepresentativeDevices: devices ?? [],
            SauceBrowsers: browsers ?? []);
    }

    private static int Count(string text, string value) =>
        Regex.Matches(text, Regex.Escape(value)).Count;
}
