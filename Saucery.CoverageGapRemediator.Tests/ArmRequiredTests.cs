using Xunit;

namespace Saucery.CoverageGapRemediator.Tests;

public sealed class ArmRequiredTests
{
    [Theory]
    [InlineData("Mac", "13", false)]
    [InlineData("Mac", "14", true)]
    [InlineData("Mac", "27", true)]
    [InlineData("IOS", "16.2", false)]
    [InlineData("IOS", "16.4", true)]
    [InlineData("IOS", "27.1", true)]
    [InlineData("IOS", "28", true)]
    public void Mac_14_and_later_and_iOS_16_4_and_later_require_arm(string prefix, string version, bool expected)
    {
        var target = new PlatformIdentity(PlatformRequirementKind.MobileExactVersion, "x", prefix, version, version.Replace(".", string.Empty));

        Assert.Equal(expected, AdditionSynthesizer.ArmRequired(target));
    }

    [Theory]
    [InlineData("Windows", "8.1")]
    [InlineData("Android", "17.0")]
    public void Other_families_have_no_arm_rule(string prefix, string version)
    {
        var target = new PlatformIdentity(PlatformRequirementKind.MobileExactVersion, "x", prefix, version, version.Replace(".", string.Empty));

        Assert.Null(AdditionSynthesizer.ArmRequired(target));
    }

    private const string ShortMobile =
        "public sealed class IOS164Platform(SupportedPlatform sp) : PlatformBase(sp, SauceryConstants.PLATFORM_IOS, [])";

    private const string ArmMobile =
        "public sealed class IOS164Platform(SupportedPlatform sp) : PlatformBase(sp, SauceryConstants.PLATFORM_IOS, [], null!, null!, true)";

    // A multi-line desktop product as it appears in Saucery.Core, without and with ARM.
    private const string DesktopWithoutArm = """
        public sealed class Mac13Platform(SupportedPlatform sp)
            : PlatformBase(sp,
                           "macOS 13",
                           [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_SAFARI],
                           [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_SAFARI],
                           [SauceryConstants.SCREENRES_1024_768,
                            SauceryConstants.SCREENRES_2048_1536 ])
        {
        }
        """;

    private const string DesktopWithArm = """
        public sealed class Mac13Platform(SupportedPlatform sp)
            : PlatformBase(sp,
                           "macOS 13",
                           [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_SAFARI],
                           [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_SAFARI],
                           [SauceryConstants.SCREENRES_1024_768,
                            SauceryConstants.SCREENRES_2048_1536 ],
                           true)
        {
        }
        """;

    [Fact]
    public void Requiring_arm_pads_a_short_mobile_call_to_the_six_argument_overload() =>
        Assert.Equal(ArmMobile, AdditionSynthesizer.SetArmRequired(ShortMobile, required: true));

    [Fact]
    public void Not_requiring_arm_trims_a_mobile_call_back_to_the_short_overload() =>
        Assert.Equal(ShortMobile, AdditionSynthesizer.SetArmRequired(ArmMobile, required: false));

    [Fact]
    public void Requiring_arm_appends_it_on_its_own_line_in_a_multi_line_desktop_call() =>
        Assert.Equal(Normalize(DesktopWithArm), Normalize(AdditionSynthesizer.SetArmRequired(DesktopWithoutArm, required: true)));

    [Fact]
    public void Not_requiring_arm_removes_it_from_a_multi_line_desktop_call() =>
        Assert.Equal(Normalize(DesktopWithoutArm), Normalize(AdditionSynthesizer.SetArmRequired(DesktopWithArm, required: false)));

    [Theory]
    [InlineData(ArmMobile, true)]
    [InlineData(ShortMobile, false)]
    [InlineData(DesktopWithArm, true)]
    [InlineData(DesktopWithoutArm, false)]
    public void Already_matching_calls_are_unchanged(string source, bool required) =>
        Assert.Equal(Normalize(source), Normalize(AdditionSynthesizer.SetArmRequired(source, required)));

    // Raw string literals take the source file's line endings, which depend on the checkout
    // (CRLF on Windows with autocrlf); the remediator writes Environment.NewLine.
    private static string Normalize(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal);
}
