using Xunit;
using Reporter = Saucery.CoverageGapReporter;

namespace Saucery.CoverageGapRemediator.Tests;

/// <summary>The remediator must read back exactly what the reporter writes.</summary>
public sealed class ReportContractTests
{
    [Fact]
    public async Task Remediator_reads_the_gaps_and_obsolete_candidates_the_reporter_writes()
    {
        var root = Path.Combine(Path.GetTempPath(), "saucery-contract-tests", Guid.NewGuid().ToString("N"));
        var core = Path.Combine(root, "core");
        Directory.CreateDirectory(core);

        try
        {
            // Saucery.Core declares real-device iOS 14 and Android 9, which Sauce no longer has.
            await File.WriteAllTextAsync(Path.Combine(core, "DojoExtensions.cs"), """
                public static class DojoExtensions
                {
                    public static object? FindIOSPlatform(string label) => label switch
                    {
                        "iOS 15" => platforms.GetPlatform<IOS15Platform>().FirstOrDefault(),
                        "iOS 14" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
                        _ => null
                    };

                    public static object? FindAndroidPlatform(string label) => label switch
                    {
                        "Linux 10" => platforms.GetPlatform<Android10Platform>().FirstOrDefault(),
                        "Linux 9" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),
                        _ => null
                    };
                }
                """, TestContext.Current.CancellationToken);

            Reporter.SauceVirtualRow Desktop(string os, string browser) =>
                new("webdriver", browser, browser, string.Empty, os, "latest", "latest", string.Empty);

            var requirements = Reporter.FunctionalPlatformBuilder.Build(
                [
                    new("appium", "iphone", "iPhone Duo", "iPhone Duo", "iOS", "27.1", "27.1", "2.0"),
                    Desktop("Windows 2012", "chrome"),
                    Desktop("Windows 2012", "internet explorer"),
                    Desktop("Windows 2012 R2", "chrome"),
                    Desktop("Windows 2012 R2", "MicrosoftEdge")
                ],
                [
                    new("iPhone_15", "iPhone 15", "IOS", "15.8", "Apple"),
                    new("Pixel_8", "Google Pixel 8", "ANDROID", "10", "Google")
                ]);

            var declared = Reporter.SauceryPlatformExtractor.Extract(core);
            var declaredByKey = declared.ToDictionary(x => x.CanonicalKey, StringComparer.OrdinalIgnoreCase);
            var corpus = Reporter.SourceCorpus.Load(core);
            var findings = requirements.Select(r => Reporter.CoverageAnalyzer.Analyze(r, declaredByKey, corpus, corpus)).ToList();
            var lifecycle = Reporter.PlatformLifecycleAnalyzer.Analyze(requirements, declared, realDeviceInventoryRead: true);

            var report = Reporter.ReportWriter.Write(
                root,
                new Reporter.Options(root, "report.txt", "us-west-1", true, null, null),
                new Reporter.DependencyAuditResult([]),
                virtualRowCount: 5,
                realDeviceCount: 2,
                findings,
                lifecycle,
                realDeviceWarning: null);

            var reportPath = Path.Combine(root, "report.txt");
            await File.WriteAllTextAsync(reportPath, report, TestContext.Current.CancellationToken);

            var discrepancies = await CoverageReportReader.ReadAsync(reportPath);

            var missing = discrepancies.Where(d => d.Type == DiscrepancyType.Missing).ToDictionary(d => d.DisplayName);
            Assert.Equal(["Windows 8", "Windows 8.1", "iOS 27.1"], missing.Keys.Order(StringComparer.Ordinal));
            Assert.Equal(["chrome", "internet explorer"], missing["Windows 8"].Browsers);
            Assert.Equal(["chrome", "MicrosoftEdge"], missing["Windows 8.1"].Browsers);
            Assert.Equal(("Windows", "8.1", PlatformRequirementKind.DesktopPlatform), (missing["Windows 8.1"].Family, missing["Windows 8.1"].Version, missing["Windows 8.1"].Kind));
            Assert.Equal(("iOS", "27.1", PlatformRequirementKind.MobileExactVersion), (missing["iOS 27.1"].Family, missing["iOS 27.1"].Version, missing["iOS 27.1"].Kind));
            Assert.Equal(["iPhone Duo"], missing["iOS 27.1"].Devices);

            var obsolete = discrepancies.Where(d => d.Type == DiscrepancyType.ObsoleteCandidate)
                .Select(d => (d.DisplayName, d.Family, d.Kind))
                .Order();
            Assert.Equal(
                [("Android 9.x", "Android", PlatformRequirementKind.MobileMajorWildcard), ("iOS 14.x", "iOS", PlatformRequirementKind.MobileMajorWildcard)],
                obsolete);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
