using Shouldly;

namespace Saucery.CoverageGapRemediator.Tests;

public sealed class PlatformIdentityTests
{
    [Test]
    [Arguments("iOS", "27.1", nameof(PlatformRequirementKind.MobileExactVersion), "IOS271Platform", null)]
    [Arguments("iOS", "27.0", nameof(PlatformRequirementKind.MobileExactVersion), "IOS27Platform", null)]
    [Arguments("iOS", "28.x", nameof(PlatformRequirementKind.MobileMajorWildcard), "IOS28Platform", null)]
    [Arguments("Android", "5.0", nameof(PlatformRequirementKind.MobileExactVersion), "Android5Platform", null)]
    [Arguments("Android", "8.1", nameof(PlatformRequirementKind.MobileExactVersion), "Android81Platform", null)]
    [Arguments("Android", "18.x", nameof(PlatformRequirementKind.MobileMajorWildcard), "Android18Platform", null)]
    [Arguments("Windows", "8", nameof(PlatformRequirementKind.DesktopPlatform), "Windows8Platform", "PLATFORM_WINDOWS_8")]
    [Arguments("Windows", "8.1", nameof(PlatformRequirementKind.DesktopPlatform), "Windows81Platform", "PLATFORM_WINDOWS_81")]
    [Arguments("macOS", "27", nameof(PlatformRequirementKind.DesktopPlatform), "Mac27Platform", "PLATFORM_MAC_27")]
    public void Target_names_follow_saucery_core_conventions(
        string family, string version, string kindName, string productType, string? constant)
    {
        var identity = AdditionSynthesizer.TargetIdentity(Missing(family, version)).ShouldNotBeNull();

        identity.Kind.ShouldBe(Enum.Parse<PlatformRequirementKind>(kindName));
        identity.ProductType.ShouldBe(productType);
        identity.CreatorType.ShouldBe(productType + "Creator");
        identity.Constant.ShouldBe(constant);
    }

    [Test]
    [Arguments("Linux", "")]
    [Arguments("ChromiumOS", "")]
    public void Families_with_no_versioned_declaration_pattern_are_not_synthesized(string family, string version) =>
        AdditionSynthesizer.TargetIdentity(Missing(family, version)).ShouldBeNull();

    [Test]
    public void Unsupported_additions_are_explained_for_manual_review()
    {
        using var repo = new FixtureRepository();

        AdditionSynthesizer.WhyUnsupported(repo.Context, Missing("ChromiumOS", "")).ShouldNotBeNull();
    }

    [Test]
    [Arguments("\"iOS 14\" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),", "iOS", "14.x", true)]
    [Arguments("\"14\" => new IOS14PlatformCreator(sp).Create(),", "iOS", "14.x", true)]
    [Arguments("\"Linux 9\" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),", "Android", "9.x", true)]
    [Arguments("\"iOS 14.0\" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),", "iOS", "14.x", false)]
    [Arguments("\"14.0\" => new IOS14PlatformCreator(sp).Create(),", "iOS", "14.x", false)]
    [Arguments("\"iOS 14\" => platforms.GetPlatform<IOS15Platform>().FirstOrDefault(),", "iOS", "14.x", false)]
    public void Only_bare_major_lines_for_the_target_type_are_real_device_declarations(
        string line, string family, string version, bool expected)
    {
        var target = new CoverageDiscrepancy(DiscrepancyType.ObsoleteCandidate, family, PlatformRequirementKind.MobileMajorWildcard, $"{family} {version}", version);

        RealDeviceDeclarations.IsDeclarationLine(line, target).ShouldBe(expected);
    }

    private static CoverageDiscrepancy Missing(string family, string version) =>
        new(DiscrepancyType.Missing, family, PlatformRequirementKind.DesktopPlatform, $"{family} {version}".Trim(), version);
}
