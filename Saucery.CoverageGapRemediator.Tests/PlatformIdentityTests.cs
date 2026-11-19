using Xunit;

namespace Saucery.CoverageGapRemediator.Tests;

public sealed class PlatformIdentityTests
{
    [Theory]
    [InlineData("iOS", "27.1", nameof(PlatformRequirementKind.MobileExactVersion), "IOS271Platform", null)]
    [InlineData("iOS", "27.0", nameof(PlatformRequirementKind.MobileExactVersion), "IOS27Platform", null)]
    [InlineData("iOS", "28.x", nameof(PlatformRequirementKind.MobileMajorWildcard), "IOS28Platform", null)]
    [InlineData("Android", "5.0", nameof(PlatformRequirementKind.MobileExactVersion), "Android5Platform", null)]
    [InlineData("Android", "8.1", nameof(PlatformRequirementKind.MobileExactVersion), "Android81Platform", null)]
    [InlineData("Android", "18.x", nameof(PlatformRequirementKind.MobileMajorWildcard), "Android18Platform", null)]
    [InlineData("Windows", "8", nameof(PlatformRequirementKind.DesktopPlatform), "Windows8Platform", "PLATFORM_WINDOWS_8")]
    [InlineData("Windows", "8.1", nameof(PlatformRequirementKind.DesktopPlatform), "Windows81Platform", "PLATFORM_WINDOWS_81")]
    [InlineData("macOS", "27", nameof(PlatformRequirementKind.DesktopPlatform), "Mac27Platform", "PLATFORM_MAC_27")]
    public void Target_names_follow_saucery_core_conventions(
        string family, string version, string kindName, string productType, string? constant)
    {
        var identity = AdditionSynthesizer.TargetIdentity(Missing(family, version))!;

        Assert.Equal(Enum.Parse<PlatformRequirementKind>(kindName), identity.Kind);
        Assert.Equal(productType, identity.ProductType);
        Assert.Equal(productType + "Creator", identity.CreatorType);
        Assert.Equal(constant, identity.Constant);
    }

    [Theory]
    [InlineData("Linux", "")]
    [InlineData("ChromiumOS", "")]
    public void Families_with_no_versioned_declaration_pattern_are_not_synthesized(string family, string version) =>
        Assert.Null(AdditionSynthesizer.TargetIdentity(Missing(family, version)));

    [Fact]
    public void Unsupported_additions_are_explained_for_manual_review()
    {
        using var repo = new FixtureRepository();

        Assert.NotNull(AdditionSynthesizer.WhyUnsupported(repo.Context, Missing("ChromiumOS", "")));
    }

    [Theory]
    [InlineData("\"iOS 14\" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),", "iOS", "14.x", true)]
    [InlineData("\"14\" => new IOS14PlatformCreator(sp).Create(),", "iOS", "14.x", true)]
    [InlineData("\"Linux 9\" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),", "Android", "9.x", true)]
    [InlineData("\"iOS 14.0\" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),", "iOS", "14.x", false)]
    [InlineData("\"14.0\" => new IOS14PlatformCreator(sp).Create(),", "iOS", "14.x", false)]
    [InlineData("\"iOS 14\" => platforms.GetPlatform<IOS15Platform>().FirstOrDefault(),", "iOS", "14.x", false)]
    public void Only_bare_major_lines_for_the_target_type_are_real_device_declarations(
        string line, string family, string version, bool expected)
    {
        var target = new CoverageDiscrepancy(DiscrepancyType.ObsoleteCandidate, family, PlatformRequirementKind.MobileMajorWildcard, $"{family} {version}", version);

        Assert.Equal(expected, RealDeviceDeclarations.IsDeclarationLine(line, target));
    }

    private static CoverageDiscrepancy Missing(string family, string version) =>
        new(DiscrepancyType.Missing, family, PlatformRequirementKind.DesktopPlatform, $"{family} {version}".Trim(), version);
}
