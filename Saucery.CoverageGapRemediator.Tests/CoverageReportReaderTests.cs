using Shouldly;

namespace Saucery.CoverageGapRemediator.Tests;

public sealed class CoverageReportReaderTests
{
    // The gap and obsolete sections exactly as Saucery.CoverageGapReporter writes them: entry
    // headers at column 0, indented details, gap entries with no blank line between them.
    private const string Report = """
        CORE FUNCTIONAL PLATFORM GAPS
        ============================================================================================
        Gap     iOS 27.1
                Sauce rows collapsed into this requirement: 2
                Core evidence: none
                Example devices: iPhone Duo
        Gap     Windows 8
                Sauce rows collapsed into this requirement: 156
                Core evidence: none
                Example devices: Firefox; Google Chrome; Internet Explorer
                Sauce browsers: chrome; firefox; internet explorer
        Gap     Windows 8.1
                Sauce rows collapsed into this requirement: 187
                Core evidence: none
                Sauce browsers: chrome; firefox; internet explorer; MicrosoftEdge
        Gap     Mac Golden Gate (27)
                Sauce rows collapsed into this requirement: 26
                Core evidence: none
        Gap     iOS 28.x
                Sauce rows collapsed into this requirement: 12
                Core evidence: none
                Example devices: iPhone 17 Pro

        CORE REVIEW ITEMS
        ============================================================================================
        None.

        SAUCERY COVERAGE NO LONGER REQUIRED BY CURRENT SAUCE CATALOGUE
        ============================================================================================
        These are explicit Saucery.Core platform/version declarations that could not be matched to
        the current Sauce functional platform set.

        OBSOLETE? Android 9.x
                  Family          : Android
                  Core declaration: Dojo\DojoExtensions.cs
                  Matched source  : line 196: "Linux 9" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),
                  Sauce status    : no matching current functional platform requirement

        OBSOLETE? iOS 14.x
                  Family          : iOS
                  Core declaration: Dojo\DojoExtensions.cs
                  Matched source  : line 254: "iOS 14" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
                  Sauce status    : no matching current functional platform requirement

        REPRESENTATIVE TEST COVERAGE
        ============================================================================================
        """;

    [Test]
    public async Task Each_obsolete_entry_keeps_its_own_family()
    {
        // The reader used to scan 8 lines past each header and take the next entry's
        // "Family : iOS", turning Android 9.x into an iOS discrepancy.
        var obsolete = (await Read()).Where(d => d.Type == DiscrepancyType.ObsoleteCandidate).ToArray();

        obsolete.Select(d => (d.DisplayName, d.Family, d.Version))
            .ShouldBe([("Android 9.x", "Android", "9.x"), ("iOS 14.x", "iOS", "14.x")]);
        obsolete.ShouldAllBe(d => d.Kind == PlatformRequirementKind.MobileMajorWildcard);
        obsolete[0].Evidence.ShouldNotBeNull().ShouldContain("\"Linux 9\"", Case.Sensitive);
    }

    [Test]
    public async Task Each_gap_keeps_its_own_devices_and_browsers()
    {
        var gaps = (await Read()).Where(d => d.Type == DiscrepancyType.Missing).ToDictionary(d => d.DisplayName);

        gaps["iOS 27.1"].Devices.ShouldBe(["iPhone Duo"]);
        gaps["iOS 27.1"].Browsers.ShouldBeEmpty();
        gaps["Windows 8"].Browsers.ShouldBe(["chrome", "firefox", "internet explorer"]);
        gaps["Windows 8"].SauceConfigurationCount.ShouldBe(156);
        gaps["Windows 8.1"].Browsers.ShouldBe(["chrome", "firefox", "internet explorer", "MicrosoftEdge"]);
        gaps["Windows 8.1"].Devices.ShouldBeEmpty();
        gaps["Mac Golden Gate (27)"].Browsers.ShouldBeEmpty();
        gaps["iOS 28.x"].Devices.ShouldBe(["iPhone 17 Pro"]);
    }

    [Test]
    [Arguments("iOS 27.1", "iOS", "27.1", nameof(PlatformRequirementKind.MobileExactVersion))]
    [Arguments("iOS 28.x", "iOS", "28.x", nameof(PlatformRequirementKind.MobileMajorWildcard))]
    [Arguments("Windows 8.1", "Windows", "8.1", nameof(PlatformRequirementKind.DesktopPlatform))]
    [Arguments("Mac Golden Gate (27)", "macOS", "27", nameof(PlatformRequirementKind.DesktopPlatform))]
    public async Task Gaps_are_classified_by_family_version_and_kind(
        string displayName, string family, string version, string kindName)
    {
        var gap = (await Read()).Single(d => d.DisplayName == displayName);

        (gap.Family, gap.Version, gap.Kind).ShouldBe((family, version, Enum.Parse<PlatformRequirementKind>(kindName)));
    }

    private static async Task<IReadOnlyList<CoverageDiscrepancy>> Read()
    {
        var path = Path.Combine(Path.GetTempPath(), $"saucery-report-{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(path, Report);
            return await CoverageReportReader.ReadAsync(path);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
