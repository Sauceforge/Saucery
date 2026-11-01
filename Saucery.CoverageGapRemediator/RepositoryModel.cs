using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

internal sealed class RepositoryModel
{
    private readonly RepositoryContext context;
    private readonly Dictionary<string, string[]> lines;

    private RepositoryModel(RepositoryContext context, Dictionary<string, string[]> lines, IReadOnlyList<PlatformPrototype> prototypes)
    {
        this.context = context;
        this.lines = lines;
        Prototypes = prototypes;
    }

    public IReadOnlyList<PlatformPrototype> Prototypes { get; }

    public static RepositoryModel Build(RepositoryContext context)
    {
        var files = Directory.EnumerateFiles(context.Core, "*.cs", SearchOption.AllDirectories)
            .Concat(Directory.EnumerateFiles(context.Tests, "*.cs", SearchOption.AllDirectories))
            .ToArray();
        var lines = files.ToDictionary(context.Relative, File.ReadAllLines, StringComparer.OrdinalIgnoreCase);
        var products = files.Where(x => x.Replace('\\', '/').Contains("/ConcreteProducts/", StringComparison.OrdinalIgnoreCase)).ToArray();
        var prototypes = new List<PlatformPrototype>();

        foreach(var productFile in products)
        {
            var source = File.ReadAllText(productFile);
            var typeMatch = Regex.Match(source, @"\bclass\s+(?<type>[A-Za-z_][A-Za-z0-9_]*Platform)\b");
            if(!typeMatch.Success) continue;

            var productType = typeMatch.Groups["type"].Value;
            var stem = productType[..^"Platform".Length];
            var stemMatch = Regex.Match(stem, @"^(?<prefix>[A-Za-z_]+?)(?<digits>\d+)?$");
            if(!stemMatch.Success) continue;

            var prefix = stemMatch.Groups["prefix"].Value;
            var digits = stemMatch.Groups["digits"].Value;
            var family = FamilyFromPrefix(prefix);
            var creatorType = productType + "Creator";
            var creatorFile = FindFileContaining(files, $"class {creatorType}");
            var browserCreatorType = string.IsNullOrEmpty(digits) ? prefix + "BrowserCreator" : null;
            var browserCreatorFile = browserCreatorType is null ? null : FindFileContaining(files, $"class {browserCreatorType}");
            var refs = FindRegistrations(lines, productType, creatorType, browserCreatorType, family, digits);
            var version = InferVersion(refs, prefix, digits);
            var kind = prefix.Equals("IOS", StringComparison.OrdinalIgnoreCase) || prefix.Equals("Android", StringComparison.OrdinalIgnoreCase)
                ? PlatformRequirementKind.MobileExactVersion
                : PlatformRequirementKind.DesktopPlatform;
            var helpers = FindHelpers(lines, prefix, family);

            prototypes.Add(new PlatformPrototype(
                family, prefix, version, kind, productType, context.Relative(productFile),
                creatorFile is null ? null : creatorType, creatorFile is null ? null : context.Relative(creatorFile),
                browserCreatorFile is null ? null : browserCreatorType, browserCreatorFile is null ? null : context.Relative(browserCreatorFile),
                refs, helpers, VersionNumber(version)));
        }

        return new RepositoryModel(context, lines, prototypes);
    }

    public PlatformPrototype FindBestAnalogue(CoverageDiscrepancy target)
    {
        var candidates = Prototypes.Where(p => KindCompatible(p, target)).ToArray();
        if(candidates.Length == 0) throw new InvalidDataException($"No repository prototype can model {target.DisplayName}.");

        var targetNumber = VersionNumber(target.Version);
        return candidates
            .OrderByDescending(p => FamilyEquivalent(p.Family, target.Family))
            .ThenBy(p => targetNumber == 0 || p.VersionNumber == 0 ? 0 : Math.Abs(p.VersionNumber - targetNumber))
            .ThenByDescending(p => p.Registrations.Count)
            .First();
    }

    public IEnumerable<RegistrationSite> FindTargetSites(CoverageDiscrepancy discrepancy)
    {
        var tokens = TargetTokens(discrepancy).Where(x => x.Length >= 2).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        foreach(var (file, fileLines) in lines)
        {
            for(var i = 0; i < fileLines.Length; i++)
                if(tokens.Any(t => fileLines[i].Contains(t, StringComparison.OrdinalIgnoreCase)))
                    yield return new RegistrationSite(file, i, fileLines[i]);
        }
    }

    private static bool KindCompatible(PlatformPrototype p, CoverageDiscrepancy target)
    {
        if(target.Kind == PlatformRequirementKind.DesktopPlatform) return p.Kind == PlatformRequirementKind.DesktopPlatform;
        return p.Family is "Android" or "iOS" || p.Kind == PlatformRequirementKind.MobileExactVersion;
    }

    private static bool FamilyEquivalent(string a, string b) => Normalize(a) == Normalize(b);
    private static string Normalize(string value) => Regex.Replace(value, "[^A-Za-z0-9]", "").ToUpperInvariant();

    private static string FamilyFromPrefix(string prefix) => prefix.ToUpperInvariant() switch
    {
        "IOS" => "iOS",
        "MAC" or "MACOS" => "macOS",
        _ => prefix
    };

    private static string? FindFileContaining(IEnumerable<string> files, string needle) =>
        files.FirstOrDefault(f => File.ReadAllText(f).Contains(needle, StringComparison.Ordinal));

    private static IReadOnlyList<RegistrationSite> FindRegistrations(
        IReadOnlyDictionary<string, string[]> files,
        string productType,
        string creatorType,
        string? browserCreatorType,
        string family,
        string digits)
    {
        var result = new List<RegistrationSite>();
        foreach(var (file, lines) in files)
        {
            if(file.Contains("/ConcreteProducts/", StringComparison.OrdinalIgnoreCase) ||
               file.Contains("/ConcreteCreators/", StringComparison.OrdinalIgnoreCase) ||
               file.Contains("/Browsers/ConcreteCreators/", StringComparison.OrdinalIgnoreCase)) continue;

            for(var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var direct = line.Contains(productType, StringComparison.Ordinal) || line.Contains(creatorType, StringComparison.Ordinal) ||
                             (browserCreatorType is not null && line.Contains(browserCreatorType, StringComparison.Ordinal));
                var semantic = line.Contains(family, StringComparison.OrdinalIgnoreCase) &&
                               (line.Contains("Platform", StringComparison.OrdinalIgnoreCase) || line.Contains("Browser", StringComparison.OrdinalIgnoreCase));
                var versionedTest = file.StartsWith("Saucery.Core.Tests/", StringComparison.OrdinalIgnoreCase) &&
                                    !string.IsNullOrEmpty(digits) && line.Any(char.IsDigit) &&
                                    (line.Contains("Platform", StringComparison.OrdinalIgnoreCase) || line.Contains("Device", StringComparison.OrdinalIgnoreCase));
                if(direct || semantic || versionedTest) result.Add(new RegistrationSite(file, i, line));
            }
        }
        return result;
    }

    private static string InferVersion(IReadOnlyList<RegistrationSite> refs, string prefix, string digits)
    {
        if(string.IsNullOrEmpty(digits)) return string.Empty;
        var quoted = refs.SelectMany(r => Regex.Matches(r.Line, "\\\"(?<v>\\d+(?:\\.\\d+)?)(?:\\.x)?\\\"").Select(m => m.Groups["v"].Value))
            .Where(v => VersionDigits(v) == digits || v.Split('.')[0] == digits)
            .OrderByDescending(v => v.Contains('.'))
            .FirstOrDefault();
        if(quoted is not null) return quoted.Contains('.') ? quoted : quoted + (prefix is "IOS" or "Android" ? ".0" : "");
        return DecodeDigits(prefix, digits);
    }

    private static string DecodeDigits(string prefix, string digits)
    {
        if(prefix is "IOS" or "Android")
        {
            if(digits.Length >= 3) return digits[..^1] + "." + digits[^1];
            if(digits is "51" or "71" or "81") return digits[0] + "." + digits[1];
            return digits + ".0";
        }
        if(prefix.Equals("Windows", StringComparison.OrdinalIgnoreCase) && digits == "81") return "8.1";
        return digits;
    }

    internal static string VersionDigits(string version)
    {
        var clean = version.Trim().Replace(".x", "", StringComparison.OrdinalIgnoreCase);
        var parts = clean.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if(parts.Length == 0) return string.Empty;
        return parts.Length == 1 || parts[1] == "0" ? parts[0] : parts[0] + parts[1];
    }

    internal static double VersionNumber(string version)
    {
        var clean = version.Replace(".x", ".0", StringComparison.OrdinalIgnoreCase);
        return double.TryParse(clean, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var value) ? value : 0;
    }

    private static IReadOnlyList<MethodSite> FindHelpers(IReadOnlyDictionary<string, string[]> files, string prefix, string family)
    {
        var result = new List<MethodSite>();
        foreach(var (file, lines) in files.Where(x => x.Key.StartsWith("Saucery.Core/", StringComparison.OrdinalIgnoreCase)))
        {
            for(var i = 0; i < lines.Length; i++)
            {
                if(!lines[i].Contains(prefix, StringComparison.OrdinalIgnoreCase) && !lines[i].Contains(family, StringComparison.OrdinalIgnoreCase)) continue;
                if(!Regex.IsMatch(lines[i], @"\b(?:private|internal|public)\s+static\b") || !lines[i].Contains('(')) continue;
                var end = FindMethodEnd(lines, i);
                result.Add(new MethodSite(file, i, end, string.Join(Environment.NewLine, lines[i..(end + 1)])));
                i = end;
            }
        }
        return result;
    }

    private static int FindMethodEnd(string[] lines, int start)
    {
        if(lines[start].Contains("=>"))
        {
            for(var i = start; i < lines.Length; i++) if(lines[i].TrimEnd().EndsWith(';')) return i;
            return start;
        }

        var depth = 0;
        var opened = false;
        for(var i = start; i < lines.Length; i++)
        {
            depth += lines[i].Count(c => c == '{');
            if(lines[i].Contains('{')) opened = true;
            depth -= lines[i].Count(c => c == '}');
            if(opened && depth <= 0) return i;
        }
        return start;
    }

    private static IEnumerable<string> TargetTokens(CoverageDiscrepancy d)
    {
        yield return d.Family;
        yield return d.DisplayName;
        if(!string.IsNullOrEmpty(d.Version)) yield return d.Version.Replace(".x", "", StringComparison.OrdinalIgnoreCase);
        var id = Regex.Replace(d.Family, "[^A-Za-z0-9]", "");
        if(id.Length > 0) yield return id;
    }
}
