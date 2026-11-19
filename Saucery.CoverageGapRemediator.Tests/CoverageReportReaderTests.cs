using Xunit;

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

    [Fact]
    public async Task Each_obsolete_entry_keeps_its_own_family()
    {
        // The reader used to scan 8 lines past each header and take the next entry's
        // "Family : iOS", turning Android 9.x into an iOS discrepancy.
        var obsolete = (await Read()).Where(d => d.Type == DiscrepancyType.ObsoleteCandidate).ToArray();

        Assert.Equal(
            [("Android 9.x", "Android", "9.x"), ("iOS 14.x", "iOS", "14.x")],
            obsolete.Select(d => (d.DisplayName, d.Family, d.Version)));
        Assert.All(obsolete, d => Assert.Equal(PlatformRequirementKind.MobileMajorWildcard, d.Kind));
        Assert.Contains("\"Linux 9\"", obsolete[0].Evidence);
    }

    [Fact]
    public async Task Each_gap_keeps_its_own_devices_and_browsers()
    {
        var gaps = (await Read()).Where(d => d.Type == DiscrepancyType.Missing).ToDictionary(d => d.DisplayName);

        Assert.Equal(["iPhone Duo"], gaps["iOS 27.1"].Devices);
        Assert.Empty(gaps["iOS 27.1"].Browsers);
        Assert.Equal(["chrome", "firefox", "internet explorer"], gaps["Windows 8"].Browsers);
        Assert.Equal(156, gaps["Windows 8"].SauceConfigurationCount);
        Assert.Equal(["chrome", "firefox", "internet explorer", "MicrosoftEdge"], gaps["Windows 8.1"].Browsers);
        Assert.Empty(gaps["Windows 8.1"].Devices);
        Assert.Empty(gaps["Mac Golden Gate (27)"].Browsers);
        Assert.Equal(["iPhone 17 Pro"], gaps["iOS 28.x"].Devices);
    }

    [Theory]
    [InlineData("iOS 27.1", "iOS", "27.1", nameof(PlatformRequirementKind.MobileExactVersion))]
    [InlineData("iOS 28.x", "iOS", "28.x", nameof(PlatformRequirementKind.MobileMajorWildcard))]
    [InlineData("Windows 8.1", "Windows", "8.1", nameof(PlatformRequirementKind.DesktopPlatform))]
    [InlineData("Mac Golden Gate (27)", "macOS", "27", nameof(PlatformRequirementKind.DesktopPlatform))]
    public async Task Gaps_are_classified_by_family_version_and_kind(
        string displayName, string family, string version, string kindName)
    {
        var gap = (await Read()).Single(d => d.DisplayName == displayName);

        Assert.Equal((family, version, Enum.Parse<PlatformRequirementKind>(kindName)), (gap.Family, gap.Version, gap.Kind));
    }

    private static async Task<IReadOnlyList<CoverageDiscrepancy>> Read()
    {
        var path = Path.Combine(Path.GetTempPath(), $"saucery-report-{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(path, Report, TestContext.Current.CancellationToken);
            return await CoverageReportReader.ReadAsync(path);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
