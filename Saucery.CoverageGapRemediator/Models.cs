using System.Text.Json.Serialization;

namespace Saucery.CoverageGapRemediator;

internal enum DiscrepancyType
{
    Missing,
    ObsoleteCandidate
}

internal enum PlatformRequirementKind
{
    DesktopPlatform,
    MobileExactVersion,
    MobileMajorWildcard
}

internal enum RemediationDisposition
{
    AutoFix,
    ManualReview,
    NoOp
}

internal sealed record CoverageDiscrepancy(
    DiscrepancyType Type,
    string Family,
    PlatformRequirementKind Kind,
    string DisplayName,
    string Version,
    int SauceConfigurationCount = 0,
    IReadOnlyList<string>? RepresentativeDevices = null,
    string? EvidenceFile = null,
    string? Evidence = null)
{
    [JsonIgnore]
    public IReadOnlyList<string> Devices => RepresentativeDevices ?? Array.Empty<string>();

    [JsonIgnore]
    public string Identity => $"{Type}|{Family}|{Kind}|{DisplayName}|{Version}";
}

internal sealed record FileChange(
    string Path,
    string ChangeType,
    string Description);

internal sealed record RemediationItem(
    CoverageDiscrepancy Discrepancy,
    RemediationDisposition Disposition,
    string Strategy,
    string Summary,
    IReadOnlyList<FileChange> PlannedChanges,
    IReadOnlyList<string> Warnings);

internal sealed record RemediationPlan(
    string RepositoryRoot,
    string ReportPath,
    DateTimeOffset GeneratedUtc,
    IReadOnlyList<RemediationItem> Items)
{
    public int AutoFixCount => Items.Count(x => x.Disposition == RemediationDisposition.AutoFix);
    public int ManualReviewCount => Items.Count(x => x.Disposition == RemediationDisposition.ManualReview);
}

internal sealed record ApplyResult(
    IReadOnlyList<string> ChangedFiles,
    IReadOnlyList<string> CreatedFiles,
    IReadOnlyList<string> DeletedFiles,
    IReadOnlyList<string> ValidationMessages);

internal sealed record ReporterJsonDocument(
    IReadOnlyList<ReporterJsonDiscrepancy>? Missing,
    IReadOnlyList<ReporterJsonDiscrepancy>? ObsoleteCandidates);

internal sealed record ReporterJsonDiscrepancy(
    string? Family,
    string? Kind,
    string? Version,
    string? DisplayName,
    int SauceConfigurationCount,
    IReadOnlyList<string>? RepresentativeDevices,
    string? EvidenceFile,
    string? Evidence);
