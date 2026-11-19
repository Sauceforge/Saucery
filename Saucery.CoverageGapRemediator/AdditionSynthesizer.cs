using System.Globalization;
using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

/// <summary>
/// The identity of a platform declaration in Saucery.Core: its version, the digits used in its
/// type names (27.1 -> IOS271Platform, 8.1 -> Windows81Platform, 5.0 -> Android5Platform) and,
/// for desktop platforms, its SauceryConstants name.
/// </summary>
internal sealed record PlatformIdentity(
    PlatformRequirementKind Kind,
    string Family,
    string TypePrefix,
    string Version,
    string Digits)
{
    public string ProductType => $"{TypePrefix}{Digits}Platform";
    public string CreatorType => $"{ProductType}Creator";
    public string BrowserCreatorType => $"{TypePrefix}{Digits}BrowserCreator";
    public string? Constant => Kind == PlatformRequirementKind.DesktopPlatform
        ? $"PLATFORM_{TypePrefix.ToUpperInvariant()}_{Digits}"
        : null;

    public double Number => double.TryParse(Version, NumberStyles.Number, CultureInfo.InvariantCulture, out var n) ? n : 0;

    /// <summary>The quoted literal Saucery.Core keys this platform by: "27.1", "27", or "26" for Mac 26.</summary>
    public string Literal => Version;
}

/// <summary>
/// Adds a platform Sauce Labs offers but Saucery.Core does not declare, by cloning the nearest
/// existing platform of the same kind at every site Saucery.Core and its tests register one:
///
/// Simulator/emulator X.Y: product + creator, CreatePlatform "X.Y" =>, FindIOSBrowser/FindAndroidBrowser
///   "iOS X.Y"/"Linux X.Y" =>; tests: SupportedPlatformTypes typeof, IOS version list or Android rows.
/// Real-device major X: product + creator when new, CreateRealPlatform "X" =>, FindIOSPlatform/
///   FindAndroidPlatform "iOS X"/"Linux X" =>; tests: SupportedRealDeviceTypes typeof, Real* version
///   lists/rows, PlatformDataClass real-device rows.
/// Desktop (Windows, macOS): constant, product + creator + browser creator, DesktopPlatformFactory,
///   FindDesktopBrowser, BrowserFactory, PlatformConfigurator; tests: SupportedPlatformTypes and
///   PlatformsWithBrowsersTypes typeof, a DesktopFactoryVersionTests supported row.
/// </summary>
internal static class AdditionSynthesizer
{
    private const string DojoExtensions = "Saucery.Core/Dojo/DojoExtensions.cs";
    private const string Constants = "Saucery.Core/Util/SauceryConstants.cs";

    private static readonly string[] PreferredBrowsers =
        ["BROWSER_CHROME", "BROWSER_FIREFOX", "BROWSER_EDGE", "BROWSER_SAFARI", "BROWSER_IE"];

    /// <summary>Returns null when the discrepancy can be added automatically, otherwise why not.</summary>
    public static string? WhyUnsupported(RepositoryContext context, CoverageDiscrepancy target)
    {
        try
        {
            var identity = TargetIdentity(target);
            if (identity is null)
                return $"No Saucery.Core declaration pattern exists for {target.Family} {target.Version}; only versioned iOS, Android, Windows and macOS platforms are synthesized.";

            _ = FindSibling(context, identity, target);
            return null;
        }
        catch (InvalidDataException ex)
        {
            return ex.Message;
        }
    }

    public static IReadOnlyList<FileChange> DescribeChanges(RepositoryContext context, CoverageDiscrepancy target)
    {
        var identity = TargetIdentity(target)!;
        var sibling = FindSibling(context, identity, target);
        var changes = new List<FileChange>
        {
            new("Saucery.Core", "Clone", $"Model {target.DisplayName} on {sibling.ProductType} ({sibling.Family} {sibling.Version}).")
        };

        foreach (var file in SiblingCoreLines(context, identity, sibling).Select(x => x.Relative).Distinct())
            changes.Add(new(file, "Register", $"Clone {sibling.ProductType} registration for {identity.ProductType}."));

        changes.Add(new("Saucery.Core.Tests*", "Add tests", $"Add {target.DisplayName} to the same test lists/rows that cover {sibling.Family} {sibling.Version}."));
        return changes;
    }

    public static void Apply(RepositoryContext context, CoverageDiscrepancy target, ChangeCollector changes)
    {
        var identity = TargetIdentity(target)
            ?? throw new InvalidDataException($"No Saucery.Core declaration pattern exists for {target.DisplayName}.");
        var sibling = FindSibling(context, identity, target);
        Console.WriteLine($"    Modelling {target.DisplayName} as {identity.ProductType} on {sibling.ProductType} ({sibling.Version}).");

        CloneClassFiles(context, changes, identity, sibling, target);
        CloneCoreRegistrations(context, changes, identity, sibling);
        AddTests(context, changes, identity, sibling, target);
    }

    // ---------------------------------------------------------------- identity

    internal static PlatformIdentity? TargetIdentity(CoverageDiscrepancy target)
    {
        var version = target.Version?.Trim() ?? string.Empty;

        if (target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase) ||
            target.Family.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            var prefix = target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase) ? "IOS" : "Android";
            var family = prefix == "IOS" ? "iOS" : "Android";

            var wildcard = Regex.Match(version, @"^(?<major>\d+)\.x$", RegexOptions.IgnoreCase);
            if (wildcard.Success)
            {
                var major = wildcard.Groups["major"].Value;
                return new(PlatformRequirementKind.MobileMajorWildcard, family, prefix, major, major);
            }

            if (Regex.IsMatch(version, @"^\d+\.\d+$"))
                return new(PlatformRequirementKind.MobileExactVersion, family, prefix, version, RepositoryModel.VersionDigits(version));

            return null;
        }

        if (target.Family.Equals("Windows", StringComparison.OrdinalIgnoreCase) &&
            Regex.IsMatch(version, @"^\d+(?:\.\d+)?$"))
            return new(PlatformRequirementKind.DesktopPlatform, "Windows", "Windows", version, version.Replace(".", string.Empty));

        if ((target.Family.Equals("macOS", StringComparison.OrdinalIgnoreCase) ||
             target.Family.Equals("Mac", StringComparison.OrdinalIgnoreCase)) &&
            Regex.IsMatch(version, @"^\d+$"))
            return new(PlatformRequirementKind.DesktopPlatform, "macOS", "Mac", version, version);

        return null;
    }

    /// <summary>Existing declarations of the same kind and family, read from DojoExtensions.</summary>
    internal static IReadOnlyList<PlatformIdentity> Declarations(RepositoryContext context, PlatformIdentity like)
    {
        var path = context.PathFromRoot(DojoExtensions);
        var result = new List<PlatformIdentity>();
        var osLabel = like.TypePrefix == "IOS" ? "iOS" : "Linux";

        foreach (var line in File.ReadLines(path))
        {
            Match m;
            switch (like.Kind)
            {
                case PlatformRequirementKind.MobileExactVersion:
                    m = Regex.Match(line, $@"^\s*""{osLabel} (?<v>\d+\.\d+)""\s*=>\s*platforms\.GetPlatform<{like.TypePrefix}(?<d>\d+)Platform>");
                    if (m.Success) result.Add(like with { Version = m.Groups["v"].Value, Digits = m.Groups["d"].Value });
                    break;

                case PlatformRequirementKind.MobileMajorWildcard:
                    m = Regex.Match(line, $@"^\s*""{osLabel} (?<v>\d+)""\s*=>\s*platforms\.GetPlatform<{like.TypePrefix}(?<d>\d+)Platform>");
                    if (m.Success) result.Add(like with { Version = m.Groups["v"].Value, Digits = m.Groups["d"].Value });
                    break;

                case PlatformRequirementKind.DesktopPlatform:
                    m = Regex.Match(line, $@"^\s*SauceryConstants\.PLATFORM_{like.TypePrefix.ToUpperInvariant()}_(?<d>\d+)\s*=>\s*platforms\.GetPlatform<{like.TypePrefix}(?<d2>\d+)Platform>");
                    if (m.Success && m.Groups["d"].Value == m.Groups["d2"].Value)
                    {
                        var digits = m.Groups["d"].Value;
                        var version = like.TypePrefix == "Windows" && digits.Length == 2 && digits != "10" && digits != "11"
                            ? $"{digits[0]}.{digits[1]}"
                            : digits;
                        result.Add(like with { Version = version, Digits = digits });
                    }
                    break;
            }
        }

        return result;
    }

    internal static PlatformIdentity FindSibling(RepositoryContext context, PlatformIdentity target, CoverageDiscrepancy discrepancy)
    {
        var siblings = Declarations(context, target)
            .Where(x => !x.ProductType.Equals(target.ProductType, StringComparison.Ordinal) || x.Version != target.Version)
            .ToArray();

        if (siblings.Length == 0)
            throw new InvalidDataException($"No existing {target.Family} {target.Kind} declaration in {DojoExtensions} to model {target.Family} {target.Version} on.");

        // A cloned desktop platform's browsers can only be narrowed to what Sauce offers, never
        // widened, so prefer a sibling that already supports every browser Sauce offers.
        var offered = target.Kind == PlatformRequirementKind.DesktopPlatform
            ? OfferedBrowserConstants(context, discrepancy)
            : null;

        return siblings
            .OrderByDescending(x => offered is null || offered.IsSubsetOf(SiblingBrowsers(context, x)))
            .ThenBy(x => Math.Abs(x.Number - target.Number))
            .ThenByDescending(x => x.Number)
            .First();
    }

    private static HashSet<string> SiblingBrowsers(RepositoryContext context, PlatformIdentity sibling)
    {
        var file = FindClassFile(context, sibling.ProductType, "/Platforms/ConcreteProducts/");
        return file is null
            ? []
            : Regex.Matches(File.ReadAllText(context.PathFromRoot(file)), @"SauceryConstants\.(?<n>BROWSER_[A-Z_]+)")
                .Select(m => m.Groups["n"].Value)
                .ToHashSet(StringComparer.Ordinal);
    }

    // ---------------------------------------------------------------- transformation

    private static string Transform(string text, PlatformIdentity target, PlatformIdentity sibling)
    {
        // Type names: Mac26Platform/Mac26PlatformCreator/Mac26BrowserCreator -> Mac27...
        text = Regex.Replace(
            text,
            $@"\b{Regex.Escape(sibling.TypePrefix)}{Regex.Escape(sibling.Digits)}(?=Platform\b|PlatformCreator\b|BrowserCreator\b)",
            $"{target.TypePrefix}{target.Digits}");

        if (sibling.Constant is not null && target.Constant is not null)
            text = Regex.Replace(text, $@"\b{Regex.Escape(sibling.Constant)}\b", target.Constant);

        // Quoted version labels: "27.0" / "iOS 27.0" / "Linux 5.1" / "macOS 26" / "Mac 26".
        text = Regex.Replace(
            text,
            $@"""(?<pre>(?:iOS |Linux |Android |macOS |Mac |Windows )?){Regex.Escape(sibling.Literal)}""",
            m => $"\"{m.Groups["pre"].Value}{target.Literal}\"");

        return text;
    }

    // ---------------------------------------------------------------- classes

    private static void CloneClassFiles(
        RepositoryContext context,
        ChangeCollector changes,
        PlatformIdentity target,
        PlatformIdentity sibling,
        CoverageDiscrepancy discrepancy)
    {
        var pairs = new List<(string SiblingType, string TargetType, string Segment)>
        {
            (sibling.ProductType, target.ProductType, "/Platforms/ConcreteProducts/"),
            (sibling.CreatorType, target.CreatorType, "/Platforms/ConcreteCreators/")
        };

        if (target.Kind == PlatformRequirementKind.DesktopPlatform)
            pairs.Add((sibling.BrowserCreatorType, target.BrowserCreatorType, "/Browsers/ConcreteCreators/"));

        var offered = OfferedBrowserConstants(context, discrepancy);

        foreach (var (siblingType, targetType, segment) in pairs)
        {
            if (FindClassFile(context, targetType, segment) is not null)
            {
                Console.WriteLine($"    {targetType} already exists; reusing it.");
                continue;
            }

            var siblingFile = FindClassFile(context, siblingType, segment)
                ?? throw new InvalidDataException($"Sibling class {siblingType} was not found under {segment}.");

            var targetFile = Path.Combine(
                    Path.GetDirectoryName(siblingFile)!,
                    Path.GetFileName(siblingFile).Replace(siblingType, targetType, StringComparison.Ordinal))
                .Replace('\\', '/');

            var source = Transform(changes.Read(siblingFile), target, sibling);
            if (offered is not null)
                source = RestrictBrowsers(source, offered);

            Console.WriteLine($"    Creating {targetFile} from {siblingFile}.");
            changes.Write(targetFile, source);
        }
    }

    /// <summary>
    /// SauceryConstants BROWSER_* names Sauce offers on the target, from the report's
    /// "Sauce browsers" line. Null when the report did not say, which leaves the sibling's set.
    /// </summary>
    private static HashSet<string>? OfferedBrowserConstants(RepositoryContext context, CoverageDiscrepancy target)
    {
        if (target.Browsers.Count == 0)
            return null;

        var offered = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in Regex.Matches(
                     File.ReadAllText(context.PathFromRoot(Constants)),
                     @"const\s+string\s+(?<name>BROWSER_[A-Z_]+)\s*=\s*""(?<value>[^""]+)"""))
        {
            // Exact match: Sauce api_names equal the primary constants ("MicrosoftEdge"), while
            // aliases such as BROWSER_EDGE_LOWER differ only by case and no platform lists them.
            if (target.Browsers.Any(b => b.Equals(m.Groups["value"].Value, StringComparison.Ordinal)))
                offered.Add(m.Groups["name"].Value);
        }

        return offered;
    }

    private static string RestrictBrowsers(string source, HashSet<string> offered)
    {
        // Browser lists in a product: [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_EDGE, ...]
        source = Regex.Replace(source, @"\[(?<items>\s*SauceryConstants\.BROWSER_[A-Z_]+(?:\s*,\s*SauceryConstants\.BROWSER_[A-Z_]+)*\s*)\]", m =>
        {
            var kept = Regex.Matches(m.Groups["items"].Value, @"SauceryConstants\.(?<n>BROWSER_[A-Z_]+)")
                .Select(x => x.Groups["n"].Value)
                .Where(offered.Contains)
                .Select(n => $"SauceryConstants.{n}");
            return $"[{string.Join(", ", kept)}]";
        });

        // Browser-creator switch arms: SauceryConstants.BROWSER_EDGE => new EdgeBrowser(...),
        var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n')
            .Where(line =>
            {
                var arm = Regex.Match(line, @"^\s*SauceryConstants\.(?<n>BROWSER_[A-Z_]+)\s*=>");
                return !arm.Success || offered.Contains(arm.Groups["n"].Value);
            });

        return string.Join(Environment.NewLine, lines);
    }

    private static string? FindClassFile(RepositoryContext context, string typeName, string segment) =>
        RealDeviceDeclarations.CoreSourceFiles(context)
            .Where(f => f.Replace('\\', '/').Contains(segment, StringComparison.OrdinalIgnoreCase))
            .Where(f => Regex.IsMatch(File.ReadAllText(f), $@"\bclass\s+{Regex.Escape(typeName)}\b"))
            .Select(context.Relative)
            .FirstOrDefault();

    // ---------------------------------------------------------------- Core registrations

    private sealed record SiblingLine(string Relative, int Index, string Line);

    private static IEnumerable<SiblingLine> SiblingCoreLines(RepositoryContext context, PlatformIdentity target, PlatformIdentity sibling)
    {
        foreach (var path in RealDeviceDeclarations.CoreSourceFiles(context))
        {
            var relative = context.Relative(path);
            if (relative.Contains("/ConcreteProducts/", StringComparison.OrdinalIgnoreCase) ||
                relative.Contains("/ConcreteCreators/", StringComparison.OrdinalIgnoreCase))
                continue;

            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i++)
            {
                if (IsSiblingRegistration(lines[i], target, sibling))
                    yield return new(relative, i, lines[i]);
            }
        }
    }

    private static bool IsSiblingRegistration(string line, PlatformIdentity target, PlatformIdentity sibling)
    {
        if (line.TrimStart().StartsWith("//", StringComparison.Ordinal) ||
            line.TrimStart().StartsWith("using ", StringComparison.Ordinal))
            return false;

        var referencesSibling =
            RealDeviceDeclarations.ContainsIdentifier(line, sibling.ProductType) ||
            RealDeviceDeclarations.ContainsIdentifier(line, sibling.CreatorType) ||
            RealDeviceDeclarations.ContainsIdentifier(line, sibling.BrowserCreatorType) ||
            (sibling.Constant is not null && RealDeviceDeclarations.ContainsIdentifier(line, sibling.Constant));

        if (!referencesSibling)
            return false;

        return target.Kind switch
        {
            // Simulator/emulator registrations are keyed by the exact version, e.g. "27.0" or "iOS 27.0".
            PlatformRequirementKind.MobileExactVersion =>
                Regex.IsMatch(line, $@"^\s*""(?:(?:iOS|Linux|Android)\s+)?{Regex.Escape(sibling.Literal)}""\s*=>"),
            // Real-device registrations are keyed by the bare major, e.g. "27" or "iOS 27".
            PlatformRequirementKind.MobileMajorWildcard =>
                Regex.IsMatch(line, $@"^\s*""(?:(?:iOS|Linux|Android)\s+)?{Regex.Escape(sibling.Literal)}""\s*=>"),
            _ => true
        };
    }

    private static void CloneCoreRegistrations(RepositoryContext context, ChangeCollector changes, PlatformIdentity target, PlatformIdentity sibling)
    {
        foreach (var group in SiblingCoreLines(context, target, sibling).GroupBy(x => x.Relative))
        {
            var lines = changes.Read(group.Key).Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
            var changed = false;

            foreach (var site in group.OrderByDescending(x => x.Index))
            {
                var index = lines.FindIndex(l => l == site.Line.TrimEnd('\r'));
                if (index < 0)
                    continue;

                var transformed = Transform(lines[index], target, sibling);
                if (transformed == lines[index])
                    continue;

                var key = RegistrationKey(transformed);
                if (lines.Any(l => RegistrationKey(l) == key))
                {
                    Console.WriteLine($"    {group.Key} already registers {key}; leaving it.");
                    continue;
                }

                // Keep descending/ascending order: newer versions sit above older siblings.
                var insertAt = target.Number > sibling.Number ? index : index + 1;
                var candidate = new List<string>(lines);
                candidate.Insert(insertAt, transformed);

                try
                {
                    CSharpGuard.AssertValid(string.Join(Environment.NewLine, candidate), context.PathFromRoot(group.Key));
                }
                catch (InvalidDataException)
                {
                    throw new InvalidDataException($"Cloning {sibling.ProductType}'s registration in {group.Key} for {target.ProductType} would not parse: {transformed.Trim()}");
                }

                Console.WriteLine($"    Registering in {group.Key}: {transformed.Trim()}");
                lines = candidate;
                changed = true;
            }

            if (changed)
                changes.Write(group.Key, string.Join(Environment.NewLine, lines));
        }
    }

    /// <summary>What a registration line registers: its switch arm selector, constant name, or list item.</summary>
    private static string RegistrationKey(string line)
    {
        var trimmed = line.Trim();
        var arrow = trimmed.IndexOf("=>", StringComparison.Ordinal);
        if (arrow > 0)
            return trimmed[..arrow].Trim();

        var constant = Regex.Match(trimmed, @"\bconst\s+\w+\s+(?<name>\w+)\s*=");
        if (constant.Success)
            return constant.Groups["name"].Value;

        return Regex.Replace(trimmed, @"(?:,|\s+or)\s*$", string.Empty).Trim();
    }

    // ---------------------------------------------------------------- tests

    private static void AddTests(
        RepositoryContext context,
        ChangeCollector changes,
        PlatformIdentity target,
        PlatformIdentity sibling,
        CoverageDiscrepancy discrepancy)
    {
        foreach (var testRoot in context.TestRoots)
        {
            foreach (var path in Directory.EnumerateFiles(testRoot, "*.cs", SearchOption.AllDirectories)
                         .Where(p => !RealDeviceDeclarations.IsBuildOutput(context, p)))
            {
                var relative = context.Relative(path);
                var fileName = Path.GetFileName(path);
                var lines = changes.Read(relative).Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
                var before = string.Join("\n", lines);

                switch (target.Kind)
                {
                    case PlatformRequirementKind.MobileExactVersion:
                        AddTypeOf(lines, "SupportedPlatformTypes", target, sibling);
                        if (fileName.StartsWith($"{target.TypePrefix}FactoryVersionTests", StringComparison.Ordinal))
                        {
                            AddVersionLiteral(lines, target, sibling, requireDot: true);
                            AddCloneRow(lines, "SupportedTestCases", target.TypePrefix == "IOS" ? "new IOSPlatform(" : "new AndroidPlatform(", target, device: null, templateVersion: sibling.Literal);
                        }
                        break;

                    case PlatformRequirementKind.MobileMajorWildcard:
                        AddTypeOf(lines, "SupportedRealDeviceTypes", target, sibling);
                        var realCtor = target.TypePrefix == "IOS" ? "new IOSRealDevice(" : "new AndroidRealDevice(";
                        var device = discrepancy.Devices.FirstOrDefault();
                        if (fileName.StartsWith($"Real{target.TypePrefix}FactoryVersionTests", StringComparison.Ordinal))
                        {
                            AddVersionLiteral(lines, target, sibling, requireDot: false);
                            AddCloneRow(lines, "SupportedTestCases", realCtor, target, device);
                        }
                        if (fileName == "PlatformDataClass.cs")
                            AddCloneRow(lines, $"Real{target.TypePrefix}Devices", realCtor, target, device);
                        break;

                    case PlatformRequirementKind.DesktopPlatform:
                        AddTypeOf(lines, "SupportedPlatformTypes", target, sibling);
                        AddTypeOf(lines, "PlatformsWithBrowsersTypes", target, sibling);
                        if (fileName.StartsWith("DesktopFactoryVersionTests", StringComparison.Ordinal))
                            AddDesktopRow(context, lines, target, discrepancy);
                        break;
                }

                var after = string.Join("\n", lines);
                if (after == before)
                    continue;

                var text = string.Join(Environment.NewLine, lines);
                try
                {
                    CSharpGuard.AssertValid(text, path);
                }
                catch (InvalidDataException ex)
                {
                    throw new InvalidDataException($"Adding {target.ProductType} tests to {relative} would not parse. {ex.Message}");
                }

                Console.WriteLine($"    Adding {target.Family} {target.Version} tests to {relative}.");
                changes.Write(relative, text);
            }
        }
    }

    /// <summary>Line indexes inside a static member, from its declaration to its closing semicolon.</summary>
    private static (int Start, int End)? MemberRange(IReadOnlyList<string> lines, string memberName)
    {
        for (var i = 0; i < lines.Count; i++)
        {
            if (!Regex.IsMatch(lines[i], $@"\b(?:public|internal|private)\b.*\b{Regex.Escape(memberName)}\b\s*(?:=>|=|\{{|$)"))
                continue;

            for (var j = i + 1; j < lines.Count; j++)
            {
                if (lines[j].TrimEnd().EndsWith(';'))
                    return (i + 1, j);
            }
        }

        return null;
    }

    private static void AddTypeOf(List<string> lines, string memberName, PlatformIdentity target, PlatformIdentity sibling)
    {
        if (MemberRange(lines, memberName) is not { } range)
            return;

        var members = Enumerable.Range(range.Start, range.End - range.Start + 1).ToArray();
        if (members.Any(i => !lines[i].TrimStart().StartsWith("//", StringComparison.Ordinal) &&
                             lines[i].Contains($"typeof({target.ProductType})", StringComparison.Ordinal)))
            return;

        // Anchor on the sibling's entry (even a commented-out one); the sibling is only in lists
        // whose coverage the new platform should share.
        var anchor = members.FirstOrDefault(i => lines[i].Contains($"typeof({sibling.ProductType})", StringComparison.Ordinal), -1);
        if (anchor < 0)
            return;

        var indent = Regex.Match(lines[anchor], @"^\s*").Value;
        lines.Insert(anchor, $"{indent}typeof({target.ProductType}),");
    }

    private static readonly Regex VersionListLine = new(@"^\s*(?:""\d+(?:\.\d+)?""\s*,?\s*)+$");

    /// <summary>
    /// Adds the version to string-literal version lists such as ["15", "16"] or "17.0","17.5",.
    /// Only lists that already test the sibling's version: a list that stops short of the
    /// sibling (iOS simulators past 18.x use other device names) must not gain the target.
    /// </summary>
    private static void AddVersionLiteral(List<string> lines, PlatformIdentity target, PlatformIdentity sibling, bool requireDot)
    {
        var literalPattern = requireDot ? @"""(?<v>\d+\.\d+)""" : @"""(?<v>\d+)""";
        var candidates = new List<(int Line, string Literal, double Number)>();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var isList = VersionListLine.IsMatch(line) ||
                         Regex.IsMatch(line, @"(?:\[|new\[\]\s*\{)\s*""\d") && Regex.Matches(line, @"""[^""]*""").All(m => Regex.IsMatch(m.Value, @"^""\d+(?:\.\d+)?""$"));
            if (!isList)
                continue;

            if (line.Contains($"\"{target.Literal}\"", StringComparison.Ordinal))
                return;

            foreach (Match m in Regex.Matches(line, literalPattern))
                candidates.Add((i, m.Value, double.Parse(m.Groups["v"].Value, CultureInfo.InvariantCulture)));
        }

        if (candidates.All(c => c.Literal != $"\"{sibling.Literal}\""))
            return;

        var nearest = candidates.OrderBy(c => Math.Abs(c.Number - target.Number)).ThenByDescending(c => c.Number).First();
        var lineText = lines[nearest.Line];
        var separator = lineText.Contains("\", \"", StringComparison.Ordinal) ? ", " : ",";
        var replacement = target.Number > nearest.Number
            ? $"{nearest.Literal}{separator}\"{target.Literal}\""
            : $"\"{target.Literal}\"{separator}{nearest.Literal}";

        var at = lineText.IndexOf(nearest.Literal, StringComparison.Ordinal);
        lines[nearest.Line] = lineText[..at] + replacement + lineText[(at + nearest.Literal.Length)..];
    }

    /// <summary>
    /// Clones a row of the given constructor inside a member, swapping in the target version and,
    /// for real devices, a device Sauce reports for that version. Without a reported device the
    /// row keeps its template's device, so it must be the sibling's own row (templateVersion).
    /// </summary>
    private static void AddCloneRow(List<string> lines, string memberName, string constructor, PlatformIdentity target, string? device, string? templateVersion = null)
    {
        if (MemberRange(lines, memberName) is not { } range)
            return;

        var rows = new List<(int Index, string Version, double Number)>();
        for (var i = range.Start; i <= range.End; i++)
        {
            var line = lines[i];
            if (line.TrimStart().StartsWith("//", StringComparison.Ordinal) ||
                !line.Contains(constructor, StringComparison.Ordinal) ||
                line.Contains("\"NonExistent\"", StringComparison.Ordinal))
                continue;

            var version = Regex.Matches(line, @"""(?<v>\d+(?:\.\d+)?)""").LastOrDefault();
            if (version is null)
                continue;

            if (version.Groups["v"].Value == target.Literal)
                return;

            rows.Add((i, version.Groups["v"].Value, double.Parse(version.Groups["v"].Value, CultureInfo.InvariantCulture)));
        }

        if (templateVersion is not null)
            rows.RemoveAll(r => r.Version != templateVersion);

        if (rows.Count == 0)
            return;

        var template = rows.OrderBy(r => Math.Abs(r.Number - target.Number)).ThenByDescending(r => r.Number).First();
        var row = lines[template.Index];
        var versionAt = row.LastIndexOf($"\"{template.Version}\"", StringComparison.Ordinal);
        row = row[..versionAt] + $"\"{target.Literal}\"" + row[(versionAt + template.Version.Length + 2)..];

        if (!string.IsNullOrWhiteSpace(device))
        {
            row = Regex.Replace(
                row,
                $@"{Regex.Escape(constructor)}""[^""]*""",
                $"{constructor}\"{device.Replace("\"", string.Empty)}\"");
        }

        lines.Insert(template.Index, EnsureTrailingComma(row));
    }

    private static void AddDesktopRow(RepositoryContext context, List<string> lines, PlatformIdentity target, CoverageDiscrepancy discrepancy)
    {
        if (MemberRange(lines, "SupportedTestCases") is not { } range)
            return;

        var familyConstant = $"PLATFORM_{target.TypePrefix.ToUpperInvariant()}_";
        var rows = Enumerable.Range(range.Start, range.End - range.Start + 1)
            .Where(i => lines[i].Contains("new DesktopPlatform(", StringComparison.Ordinal) &&
                        !lines[i].TrimStart().StartsWith("//", StringComparison.Ordinal))
            .ToArray();

        if (rows.Any(i => RealDeviceDeclarations.ContainsIdentifier(lines[i], target.Constant!)))
            return;

        var template = rows.FirstOrDefault(i => lines[i].Contains(familyConstant, StringComparison.Ordinal), rows.FirstOrDefault(-1));
        if (template < 0)
            return;

        var offered = OfferedBrowserConstants(context, discrepancy);
        var browser = PreferredBrowsers.FirstOrDefault(b => offered is null || offered.Contains(b)) ?? "BROWSER_CHROME";
        var call = $"new DesktopPlatform(SauceryConstants.{target.Constant}, SauceryConstants.{browser}, SauceryConstants.BROWSER_VERSION_CURRENT_MAJOR)";
        var row = Regex.Replace(lines[template], @"new DesktopPlatform\(.*\)(?=\]?\s*,?\s*$)", call);

        lines.Insert(template, EnsureTrailingComma(row));
    }

    private static string EnsureTrailingComma(string row) =>
        row.TrimEnd().EndsWith(',') ? row : row.TrimEnd() + ",";
}
