using Shouldly;
using static Saucery.CoverageGapReporter.Tests.TestData;

namespace Saucery.CoverageGapReporter.Tests;

public sealed class CoverageAnalyzerTests
{
    private const string DojoExtensions = """
        // iOS simulators and iPhone/iPad devices are resolved here.
        public static class DojoExtensions
        {
            public static object? FindIOSBrowser(string label) => label switch
            {
                "iOS 27.0" => platforms.GetPlatform<IOS27Platform>().FirstOrDefault(),
                _ => null
            };
        }
        """;

    [Test]
    public void Version_saucery_does_not_declare_is_a_gap_even_though_the_family_is_mentioned()
    {
        // The family name ("iOS") appears all over Saucery.Core. That used to downgrade every
        // unmapped version to Review, which the remediator ignores, so nothing was ever added.
        var finding = Analyze(IOSSimulator("27.1"));

        finding.CoreStatus.ShouldBe(CoverageStatus.Gap);
    }

    [Test]
    public void Version_saucery_declares_is_covered_by_that_declaration()
    {
        var finding = Analyze(IOSSimulator("27.0"));

        finding.CoreStatus.ShouldBe(CoverageStatus.Covered);
        finding.CoreEvidence.Strength.ShouldBe(EvidenceStrength.Direct);
        finding.CoreEvidence.Match.ShouldContain("\"iOS 27.0\"", Case.Sensitive);
    }

    private static CoverageFinding Analyze(SauceVirtualRow row)
    {
        using var core = new SourceFolder(("Dojo/DojoExtensions.cs", DojoExtensions));
        using var tests = new SourceFolder(("PlatformTypes.cs", "public static class PlatformTypes { }"));
        var requirement = FunctionalPlatformBuilder.Build([row], []).ShouldHaveSingleItem();
        var declaredByKey = SauceryPlatformExtractor.Extract(core.Root)
            .ToDictionary(x => x.CanonicalKey, StringComparer.OrdinalIgnoreCase);

        return CoverageAnalyzer.Analyze(requirement, declaredByKey, SourceCorpus.Load(core.Root), SourceCorpus.Load(tests.Root));
    }
}
