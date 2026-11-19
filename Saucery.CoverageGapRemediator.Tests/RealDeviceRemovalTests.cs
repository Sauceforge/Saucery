using Shouldly;
using static Saucery.CoverageGapRemediator.Tests.FixtureRepository;

namespace Saucery.CoverageGapRemediator.Tests;

/// <summary>
/// Removing a real-device major Sauce no longer offers, while its simulator/emulator versions
/// remain, must touch only real-device declarations and the same family's real-device tests,
/// identically in both Core test projects.
/// </summary>
public sealed class RealDeviceRemovalTests
{
    [Test]
    public void Removes_the_real_device_declarations_but_keeps_the_simulator_ones()
    {
        using var repo = Remove("iOS", "14.x");

        var dojo = repo.Read("Saucery.Core/Dojo/DojoExtensions.cs");
        dojo.ShouldNotContain("\"iOS 14\" =>", Case.Sensitive);
        dojo.ShouldContain("\"iOS 14.0\" =>", Case.Sensitive);

        var factory = repo.Read("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs");
        factory.ShouldNotContain("\"14\" =>", Case.Sensitive);
        factory.ShouldContain("\"14.0\" =>", Case.Sensitive);
    }

    [Test]
    public void Keeps_the_platform_classes_simulators_still_use()
    {
        using var repo = Remove("iOS", "14.x");

        repo.Exists("Saucery.Core/Dojo/Platforms/ConcreteProducts/Apple/IOS14Platform.cs").ShouldBeTrue();
        repo.Exists("Saucery.Core/Dojo/Platforms/ConcreteCreators/Apple/IOS14PlatformCreator.cs").ShouldBeTrue();
    }

    [Test]
    [Arguments(TUnitProject)]
    [Arguments(XUnitProject)]
    public void Removes_only_real_device_test_data(string project)
    {
        using var repo = Remove("iOS", "14.x");

        var types = $"{project}/REST/PlatformTypes.cs";
        repo.Member(types, "SupportedRealDeviceTypes").ShouldNotContain("typeof(IOS14Platform)", Case.Sensitive);
        repo.Member(types, "SupportedPlatformTypes").ShouldContain("typeof(IOS14Platform)", Case.Sensitive);

        var realIos = repo.Read($"{project}/RealIOSFactoryVersionTests.cs");
        realIos.ShouldNotContain("\"14\", ", Case.Sensitive);
        realIos.ShouldContain("\"17\", \"27\"", Case.Sensitive);
        realIos.ShouldContain("new IOSRealDevice(\"NonExistent\", \"14\")", Case.Sensitive);

        repo.Read($"{project}/DataProviders/PlatformDataClass.cs")
            .ShouldNotContain("new IOSRealDevice(\"iPhone 12 Pro Max\", \"14\")", Case.Sensitive);
        repo.Read($"{project}/IOSFactoryVersionTests.cs").ShouldContain("\"14.0\"", Case.Sensitive);
    }

    [Test]
    [Arguments(TUnitProject)]
    [Arguments(XUnitProject)]
    public void Removing_an_android_major_never_touches_ios_real_device_tests(string project)
    {
        // iOS and Android majors share digits: removing Android 17.x used to strip "17" from
        // the iOS real-device version list too.
        using var repo = Remove("Android", "17.x");

        repo.Read($"{project}/RealIOSFactoryVersionTests.cs").ShouldContain("\"14\", \"17\", \"27\"", Case.Sensitive);
        repo.Read($"{project}/RealAndroidFactoryVersionTests.cs")
            .ShouldNotContain("new AndroidRealDevice(\"Google Pixel 9\", \"17\")", Case.Sensitive);
        repo.Member($"{project}/REST/PlatformTypes.cs", "SupportedRealDeviceTypes").ShouldNotContain("typeof(Android17Platform)", Case.Sensitive);
        repo.Member($"{project}/REST/PlatformTypes.cs", "SupportedPlatformTypes").ShouldContain("typeof(Android17Platform)", Case.Sensitive);

        var dojo = repo.Read("Saucery.Core/Dojo/DojoExtensions.cs");
        dojo.ShouldNotContain("\"Linux 17\" =>", Case.Sensitive);
        dojo.ShouldContain("\"Linux 17.0\" =>", Case.Sensitive);
    }

    [Test]
    public void Both_test_projects_receive_identical_changes()
    {
        using var repo = Remove("iOS", "14.x");

        repo.Read($"{XUnitProject}/REST/PlatformTypes.cs").ShouldBe(repo.Read($"{TUnitProject}/REST/PlatformTypes.cs"));
        repo.Read($"{XUnitProject}/DataProviders/PlatformDataClass.cs").ShouldBe(repo.Read($"{TUnitProject}/DataProviders/PlatformDataClass.cs"));
    }

    private static FixtureRepository Remove(string family, string version)
    {
        var repo = new FixtureRepository();
        var target = new CoverageDiscrepancy(DiscrepancyType.ObsoleteCandidate, family, PlatformRequirementKind.MobileMajorWildcard, $"{family} {version}", version);

        RealDeviceDeclarations.IsSharedRealDeviceMajor(repo.Context, target).ShouldBeTrue();
        ObsoleteSynthesizer.Apply(repo.Context, null!, target, repo.NewChanges());
        return repo;
    }
}
