using Xunit;
using static Saucery.CoverageGapRemediator.Tests.FixtureRepository;

namespace Saucery.CoverageGapRemediator.Tests;

/// <summary>
/// Removing a real-device major Sauce no longer offers, while its simulator/emulator versions
/// remain, must touch only real-device declarations and the same family's real-device tests,
/// identically in both Core test projects.
/// </summary>
public sealed class RealDeviceRemovalTests
{
    [Fact]
    public void Removes_the_real_device_declarations_but_keeps_the_simulator_ones()
    {
        using var repo = Remove("iOS", "14.x");

        var dojo = repo.Read("Saucery.Core/Dojo/DojoExtensions.cs");
        Assert.DoesNotContain("\"iOS 14\" =>", dojo);
        Assert.Contains("\"iOS 14.0\" =>", dojo);

        var factory = repo.Read("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs");
        Assert.DoesNotContain("\"14\" =>", factory);
        Assert.Contains("\"14.0\" =>", factory);
    }

    [Fact]
    public void Keeps_the_platform_classes_simulators_still_use()
    {
        using var repo = Remove("iOS", "14.x");

        Assert.True(repo.Exists("Saucery.Core/Dojo/Platforms/ConcreteProducts/Apple/IOS14Platform.cs"));
        Assert.True(repo.Exists("Saucery.Core/Dojo/Platforms/ConcreteCreators/Apple/IOS14PlatformCreator.cs"));
    }

    [Theory]
    [InlineData(TUnit)]
    [InlineData(XUnit)]
    public void Removes_only_real_device_test_data(string project)
    {
        using var repo = Remove("iOS", "14.x");

        var types = $"{project}/REST/PlatformTypes.cs";
        Assert.DoesNotContain("typeof(IOS14Platform)", repo.Member(types, "SupportedRealDeviceTypes"));
        Assert.Contains("typeof(IOS14Platform)", repo.Member(types, "SupportedPlatformTypes"));

        var realIos = repo.Read($"{project}/RealIOSFactoryVersionTests.cs");
        Assert.DoesNotContain("\"14\", ", realIos);
        Assert.Contains("\"17\", \"27\"", realIos);
        Assert.Contains("new IOSRealDevice(\"NonExistent\", \"14\")", realIos);

        Assert.DoesNotContain("new IOSRealDevice(\"iPhone 12 Pro Max\", \"14\")", repo.Read($"{project}/DataProviders/PlatformDataClass.cs"));
        Assert.Contains("\"14.0\"", repo.Read($"{project}/IOSFactoryVersionTests.cs"));
    }

    [Theory]
    [InlineData(TUnit)]
    [InlineData(XUnit)]
    public void Removing_an_android_major_never_touches_ios_real_device_tests(string project)
    {
        // iOS and Android majors share digits: removing Android 17.x used to strip "17" from
        // the iOS real-device version list too.
        using var repo = Remove("Android", "17.x");

        Assert.Contains("\"14\", \"17\", \"27\"", repo.Read($"{project}/RealIOSFactoryVersionTests.cs"));
        Assert.DoesNotContain("new AndroidRealDevice(\"Google Pixel 9\", \"17\")", repo.Read($"{project}/RealAndroidFactoryVersionTests.cs"));
        Assert.DoesNotContain("typeof(Android17Platform)", repo.Member($"{project}/REST/PlatformTypes.cs", "SupportedRealDeviceTypes"));
        Assert.Contains("typeof(Android17Platform)", repo.Member($"{project}/REST/PlatformTypes.cs", "SupportedPlatformTypes"));

        var dojo = repo.Read("Saucery.Core/Dojo/DojoExtensions.cs");
        Assert.DoesNotContain("\"Linux 17\" =>", dojo);
        Assert.Contains("\"Linux 17.0\" =>", dojo);
    }

    [Fact]
    public void Both_test_projects_receive_identical_changes()
    {
        using var repo = Remove("iOS", "14.x");

        Assert.Equal(
            repo.Read($"{TUnit}/REST/PlatformTypes.cs"),
            repo.Read($"{XUnit}/REST/PlatformTypes.cs"));
        Assert.Equal(
            repo.Read($"{TUnit}/DataProviders/PlatformDataClass.cs"),
            repo.Read($"{XUnit}/DataProviders/PlatformDataClass.cs"));
    }

    private static FixtureRepository Remove(string family, string version)
    {
        var repo = new FixtureRepository();
        var target = new CoverageDiscrepancy(DiscrepancyType.ObsoleteCandidate, family, PlatformRequirementKind.MobileMajorWildcard, $"{family} {version}", version);

        Assert.True(RealDeviceDeclarations.IsSharedRealDeviceMajor(repo.Context, target));
        ObsoleteSynthesizer.Apply(repo.Context, null!, target, repo.NewChanges());
        return repo;
    }
}
