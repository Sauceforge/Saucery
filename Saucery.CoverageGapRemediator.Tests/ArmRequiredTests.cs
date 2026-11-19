using Shouldly;

namespace Saucery.CoverageGapRemediator.Tests;

public sealed class ArmRequiredTests
{
    [Test]
    [Arguments("Mac", "13", false)]
    [Arguments("Mac", "14", true)]
    [Arguments("Mac", "27", true)]
    [Arguments("IOS", "16.2", false)]
    [Arguments("IOS", "16.4", true)]
    [Arguments("IOS", "27.1", true)]
    [Arguments("IOS", "28", true)]
    public void Mac_14_and_later_and_iOS_16_4_and_later_require_arm(string prefix, string version, bool expected)
    {
        var target = new PlatformIdentity(PlatformRequirementKind.MobileExactVersion, "x", prefix, version, version.Replace(".", string.Empty));

        AdditionSynthesizer.ArmRequired(target).ShouldBe(expected);
    }

    [Test]
    [Arguments("Windows", "8.1")]
    [Arguments("Android", "17.0")]
    public void Other_families_have_no_arm_rule(string prefix, string version)
    {
        var target = new PlatformIdentity(PlatformRequirementKind.MobileExactVersion, "x", prefix, version, version.Replace(".", string.Empty));

        AdditionSynthesizer.ArmRequired(target).ShouldBeNull();
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

    [Test]
    public void Requiring_arm_pads_a_short_mobile_call_to_the_six_argument_overload() =>
        AdditionSynthesizer.SetArmRequired(ShortMobile, required: true).ShouldBe(ArmMobile);

    [Test]
    public void Not_requiring_arm_trims_a_mobile_call_back_to_the_short_overload() =>
        AdditionSynthesizer.SetArmRequired(ArmMobile, required: false).ShouldBe(ShortMobile);

    [Test]
    public void Requiring_arm_appends_it_on_its_own_line_in_a_multi_line_desktop_call() =>
        Normalize(AdditionSynthesizer.SetArmRequired(DesktopWithoutArm, required: true)).ShouldBe(Normalize(DesktopWithArm));

    [Test]
    public void Not_requiring_arm_removes_it_from_a_multi_line_desktop_call() =>
        Normalize(AdditionSynthesizer.SetArmRequired(DesktopWithArm, required: false)).ShouldBe(Normalize(DesktopWithoutArm));

    [Test]
    [Arguments(ArmMobile, true)]
    [Arguments(ShortMobile, false)]
    [Arguments(DesktopWithArm, true)]
    [Arguments(DesktopWithoutArm, false)]
    public void Already_matching_calls_are_unchanged(string source, bool required) =>
        Normalize(AdditionSynthesizer.SetArmRequired(source, required)).ShouldBe(Normalize(source));

    // Raw string literals take the source file's line endings, which depend on the checkout
    // (CRLF on Windows with autocrlf); the remediator writes Environment.NewLine.
    private static string Normalize(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal);
}
