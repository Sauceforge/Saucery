using System.Text.Json;
using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

internal static class CoverageReportReader
{
    public static async Task<IReadOnlyList<CoverageDiscrepancy>> ReadAsync(string path)
    {
        return Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase)
            ? await ReadJsonAsync(path)
            : await ReadTextAsync(path);
    }

    private static async Task<IReadOnlyList<CoverageDiscrepancy>> ReadJsonAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var document = await JsonSerializer.DeserializeAsync<ReporterJsonDocument>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (document is null)
        {
            throw new InvalidDataException("The reporter JSON document could not be parsed.");
        }

        var result = new List<CoverageDiscrepancy>();
        foreach (var item in document.Missing ?? Array.Empty<ReporterJsonDiscrepancy>())
        {
            result.Add(ConvertJson(item, DiscrepancyType.Missing));
        }

        foreach (var item in document.ObsoleteCandidates ?? Array.Empty<ReporterJsonDiscrepancy>())
        {
            result.Add(ConvertJson(item, DiscrepancyType.ObsoleteCandidate));
        }

        return result;
    }

    private static CoverageDiscrepancy ConvertJson(ReporterJsonDiscrepancy item, DiscrepancyType type)
    {
        var displayName = item.DisplayName?.Trim();
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new InvalidDataException("Reporter JSON discrepancy has no displayName.");
        }

        var family = item.Family?.Trim();
        if (string.IsNullOrWhiteSpace(family))
        {
            family = InferFamily(displayName);
        }

        var version = item.Version?.Trim() ?? InferVersion(displayName);
        var kind = ParseKind(item.Kind, family, version);

        return new CoverageDiscrepancy(
            type,
            family,
            kind,
            displayName,
            version,
            item.SauceConfigurationCount,
            item.RepresentativeDevices,
            item.EvidenceFile,
            item.Evidence);
    }

    private static async Task<IReadOnlyList<CoverageDiscrepancy>> ReadTextAsync(string path)
    {
        var lines = await File.ReadAllLinesAsync(path);
        var result = new List<CoverageDiscrepancy>();

        var gapStart = FindHeading(lines, "CORE FUNCTIONAL PLATFORM GAPS");
        var reviewStart = FindHeading(lines, "CORE REVIEW ITEMS");
        if (gapStart >= 0)
        {
            var gapEnd = reviewStart > gapStart ? reviewStart : lines.Length;
            ParseGaps(lines, gapStart + 1, gapEnd, result);
        }

        var obsoleteStart = FindHeading(lines, "SAUCERY COVERAGE NO LONGER REQUIRED BY CURRENT SAUCE CATALOGUE");
        var representativeStart = FindHeading(lines, "REPRESENTATIVE TEST COVERAGE");
        if (obsoleteStart >= 0)
        {
            var obsoleteEnd = representativeStart > obsoleteStart ? representativeStart : lines.Length;
            ParseObsolete(lines, obsoleteStart + 1, obsoleteEnd, result);
        }

        return [.. result
            .GroupBy(x => x.Identity, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())];
    }

    private static void ParseGaps(
        IReadOnlyList<string> lines,
        int start,
        int end,
        ICollection<CoverageDiscrepancy> result)
    {
        for (var i = start; i < end; i++)
        {
            var match = Regex.Match(lines[i], @"^\s*Gap\s+(?<name>.+?)\s*$", RegexOptions.CultureInvariant);
            if (!match.Success)
            {
                continue;
            }

            var displayName = match.Groups["name"].Value.Trim();
            var count = 0;
            var devices = new List<string>();

            for (var j = i + 1; j < end && j <= i + 6; j++)
            {
                var countMatch = Regex.Match(lines[j], @"Sauce rows collapsed into this requirement:\s*(?<count>\d+)");
                if (countMatch.Success)
                {
                    _ = int.TryParse(countMatch.Groups["count"].Value, out count);
                }

                var deviceMatch = Regex.Match(lines[j], @"Example devices:\s*(?<devices>.+)$");
                if (deviceMatch.Success)
                {
                    devices.AddRange(deviceMatch.Groups["devices"].Value
                        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }
            }

            var family = InferFamily(displayName);
            var version = InferVersion(displayName);
            result.Add(new CoverageDiscrepancy(
                DiscrepancyType.Missing,
                family,
                InferKind(family, version),
                displayName,
                version,
                count,
                devices));
        }
    }

    private static void ParseObsolete(
        IReadOnlyList<string> lines,
        int start,
        int end,
        ICollection<CoverageDiscrepancy> result)
    {
        for (var i = start; i < end; i++)
        {
            var match = Regex.Match(lines[i], @"^\s*OBSOLETE\?\s+(?<name>.+?)\s*$", RegexOptions.CultureInvariant);
            if (!match.Success)
            {
                continue;
            }

            var displayName = match.Groups["name"].Value.Trim();
            var family = InferFamily(displayName);
            string? file = null;
            string? evidence = null;

            for (var j = i + 1; j < end && j <= i + 8; j++)
            {
                var familyMatch = Regex.Match(lines[j], @"Family\s*:\s*(?<family>.+)$");
                if (familyMatch.Success)
                {
                    family = familyMatch.Groups["family"].Value.Trim();
                }

                var fileMatch = Regex.Match(lines[j], @"Core declaration\s*:\s*(?<file>.+)$");
                if (fileMatch.Success)
                {
                    file = fileMatch.Groups["file"].Value.Trim();
                }

                var evidenceMatch = Regex.Match(lines[j], @"Matched source\s*:\s*(?<evidence>.+)$");
                if (evidenceMatch.Success)
                {
                    evidence = evidenceMatch.Groups["evidence"].Value.Trim();
                }
            }

            var version = InferVersion(displayName);
            result.Add(new CoverageDiscrepancy(
                DiscrepancyType.ObsoleteCandidate,
                family,
                InferKind(family, version),
                displayName,
                version,
                EvidenceFile: file,
                Evidence: evidence));
        }
    }

    private static int FindHeading(IReadOnlyList<string> lines, string heading)
    {
        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim().Equals(heading, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
    }

    private static string InferFamily(string displayName)
    {
        if (displayName.StartsWith("Android ", StringComparison.OrdinalIgnoreCase)) return "Android";
        if (displayName.StartsWith("iOS ", StringComparison.OrdinalIgnoreCase)) return "iOS";
        if (displayName.StartsWith("Mac ", StringComparison.OrdinalIgnoreCase) ||
            displayName.StartsWith("macOS ", StringComparison.OrdinalIgnoreCase)) return "macOS";
        if (displayName.StartsWith("Windows ", StringComparison.OrdinalIgnoreCase)) return "Windows";
        if (displayName.Equals("Linux", StringComparison.OrdinalIgnoreCase) ||
            displayName.StartsWith("Linux ", StringComparison.OrdinalIgnoreCase)) return "Linux";
        if (displayName.Equals("ChromiumOS", StringComparison.OrdinalIgnoreCase) ||
            displayName.Equals("ChromeOS", StringComparison.OrdinalIgnoreCase)) return "ChromiumOS";
        return Regex.Match(displayName, @"^\S+").Value;
    }

    private static string InferVersion(string displayName)
    {
        var wildcard = Regex.Match(displayName, @"(?<version>\d+\.x)\s*$", RegexOptions.IgnoreCase);
        if (wildcard.Success) return wildcard.Groups["version"].Value;

        var parenthesized = Regex.Match(displayName, @"\((?<version>\d+(?:\.\d+)*)\)\s*$");
        if (parenthesized.Success) return parenthesized.Groups["version"].Value;

        var trailing = Regex.Match(displayName, @"(?<version>\d+(?:\.\d+)*)\s*$");
        return trailing.Success ? trailing.Groups["version"].Value : string.Empty;
    }

    private static PlatformRequirementKind ParseKind(string? raw, string family, string version)
    {
        if (!string.IsNullOrWhiteSpace(raw) && Enum.TryParse<PlatformRequirementKind>(raw, true, out var kind))
        {
            return kind;
        }

        return InferKind(family, version);
    }

    private static PlatformRequirementKind InferKind(string family, string version)
    {
        if (family is "Android" or "iOS")
        {
            return version.EndsWith(".x", StringComparison.OrdinalIgnoreCase)
                ? PlatformRequirementKind.MobileMajorWildcard
                : PlatformRequirementKind.MobileExactVersion;
        }

        return PlatformRequirementKind.DesktopPlatform;
    }
}
