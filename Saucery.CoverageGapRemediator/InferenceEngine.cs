using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

internal sealed class GenericRemediationEngine
{
    public RemediationPlan BuildPlan(RepositoryContext context, string reportPath, IReadOnlyList<CoverageDiscrepancy> discrepancies, Options options)
    {
        var model = RepositoryModel.Build(context);
        var items = discrepancies.Select(d => PlanOne(model, context, d, options)).ToArray();

        var obsoleteManualReview = items
            .Where(item =>
                item.Discrepancy.Type == DiscrepancyType.ObsoleteCandidate &&
                item.Disposition == RemediationDisposition.ManualReview)
            .ToArray();

        if(obsoleteManualReview.Length > 0)
        {
            throw new InvalidOperationException(
                "Invariant violation: obsolete candidates must never be emitted as ManualReview. " +
                string.Join(", ", obsoleteManualReview.Select(item => item.Discrepancy.DisplayName)));
        }

        return new(context.Root, reportPath, DateTimeOffset.UtcNow, items);
    }

    public async Task<ApplyResult> ApplyAsync(
        RepositoryContext context,
        RemediationPlan plan,
        Options options,
        RepositoryTransaction transaction)
    {
        var collector = new ChangeCollector(context, transaction);
        var pending = plan.Items.Where(x => x.Disposition == RemediationDisposition.AutoFix).Select(x => x.Discrepancy).ToList();
        var messages = new List<string>();

        for(var pass = 1; pass <= options.MaxRepairPasses && pending.Count > 0; pass++)
        {
            Console.WriteLine($"Remediation synthesis pass {pass}/{options.MaxRepairPasses}: {pending.Count} discrepancy(s)");
            var model = RepositoryModel.Build(context);
            foreach(var discrepancy in pending)
            {
                Console.WriteLine($"  Synthesizing {discrepancy.Type}: {discrepancy.DisplayName}");
                if(discrepancy.Type == DiscrepancyType.Missing)
                    MissingSynthesizer.Apply(context, model, discrepancy, collector);
                else
                    ObsoleteSynthesizer.Apply(context, model, discrepancy, collector);
            }

            if(options.SkipValidation) break;

            transaction.Capture(Path.Combine(context.Root, "README.md"));
            transaction.Capture(Path.Combine(context.Root, "saucery-functional-platform-coverage-gaps.txt"));
            var validation = await new RepositoryValidator().ValidateAsync(context, pending, options);
            messages.AddRange(validation.Messages.Select(x => $"Pass {pass}: {x}"));
            foreach(var message in validation.Messages) Console.WriteLine(message);

            if(validation.Success)
            {
                pending.Clear();
                break;
            }

            if(validation.Remaining.Count == 0)
                throw new InvalidOperationException("Generated remediation failed build/test validation. See validation output above.");

            pending = validation.Remaining.ToList();
        }

        if(pending.Count > 0)
            throw new InvalidOperationException($"Unable to eliminate {pending.Count} discrepancy(s) after {options.MaxRepairPasses} synthesis passes: {string.Join(", ", pending.Select(x => x.DisplayName))}");

        return new(
            collector.ChangedFiles.OrderBy(x => x).ToArray(),
            collector.CreatedFiles.OrderBy(x => x).ToArray(),
            collector.DeletedFiles.OrderBy(x => x).ToArray(),
            messages);
    }

    private static RemediationItem PlanOne(RepositoryModel model, RepositoryContext context, CoverageDiscrepancy d, Options options)
    {
        if(d.Type == DiscrepancyType.ObsoleteCandidate && !options.AllowObsoleteRemoval)
        {
            throw new InvalidOperationException(
                $"Obsolete candidate {d.DisplayName} requires --allow-obsolete-removal. " +
                "Obsolete candidates are never downgraded to ManualReview.");
        }

        if(d.Type == DiscrepancyType.ObsoleteCandidate)
        {
            if(ObsoleteDeclarationResolver.IsDeclarationOnlyExactVersion(context, d))
            {
                return new(d, RemediationDisposition.AutoFix, "exact-obsolete-declaration-removal",
                    $"Remove only the exact {d.DisplayName} declarations from BrowserFactory/DojoExtensions and matching unit-test data. Shared major-family product/creator implementations are preserved.",
                    [
                        new("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", "Remove exact declaration", $"Remove exact {d.DisplayName} browser declaration when present."),
                        new("Saucery.Core/Dojo/DojoExtensions.cs", "Remove exact declaration", $"Remove exact {d.DisplayName} Dojo declaration when present.")
                    ],
                    ["Exact-version cleanup never deletes a shared major-family ConcreteProduct or PlatformCreator."]);
            }

            ObsoleteTarget obsolete;
            try
            {
                obsolete = ObsoleteTargetResolver.Resolve(context, d);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to resolve obsolete platform {d.DisplayName}. " +
                    "Obsolete candidates are never downgraded to ManualReview.",
                    ex);
            }

            var changes = new List<FileChange>
            {
                new(obsolete.ProductFile, "Delete", $"Delete exact obsolete ConcreteProduct {obsolete.ProductType}."),
                new("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", "Remove exact declaration", $"Remove exact {d.DisplayName} browser declaration when present."),
                new("Saucery.Core/Dojo/DojoExtensions.cs", "Remove exact declaration", $"Remove exact {d.DisplayName} Dojo declaration when present.")
            };

            if(obsolete.CreatorFile is not null)
                changes.Add(new(obsolete.CreatorFile, "Delete", $"Delete exact obsolete PlatformCreator {obsolete.CreatorType}."));

            if(obsolete.BrowserCreatorFile is not null)
                changes.Add(new(obsolete.BrowserCreatorFile, "Delete", $"Delete exact obsolete BrowserCreator {obsolete.BrowserCreatorType}."));

            return new(d, RemediationDisposition.AutoFix, "exact-obsolete-type-removal",
                $"Delete {obsolete.ProductType}{(obsolete.CreatorType is null ? string.Empty : $" and {obsolete.CreatorType}")}{(obsolete.BrowserCreatorType is null ? string.Empty : $" and {obsolete.BrowserCreatorType}")}, remove exact BrowserFactory/DojoExtensions declarations, repair tests, then validate.",
                changes,
                ["Obsolete removal never uses nearest-version inference or fuzzy repository-wide text matching."]);
        }

        PlatformPrototype analogue;
        try
        {
            analogue = model.FindBestAnalogue(d);
        }
        catch(Exception ex)
        {
            return new(d, RemediationDisposition.ManualReview, "repository-inference",
                "No existing repository implementation can be matched safely.",
                Array.Empty<FileChange>(), [ex.Message]);
        }

        var missingChanges = new List<FileChange>
        {
            new(analogue.ProductFile, "Clone/transform", $"Use {analogue.ProductType} as the structural product analogue.")
        };

        if(analogue.CreatorFile is not null)
            missingChanges.Add(new(analogue.CreatorFile, "Clone/transform", $"Use {analogue.CreatorType} as the creator analogue."));
        if(analogue.BrowserCreatorFile is not null)
            missingChanges.Add(new(analogue.BrowserCreatorFile, "Clone/transform", $"Use {analogue.BrowserCreatorType} as the browser-creator analogue."));

        missingChanges.AddRange(analogue.Registrations.Select(x => new FileChange(x.File, "Infer registration", "Transform an analogous registration/test line when semantically applicable.")));
        missingChanges.AddRange(analogue.HelperMethods.Select(x => new FileChange(x.File, "Infer helper", "Clone a family-specific helper when a new family requires one.")));

        return new(d, RemediationDisposition.AutoFix, "repository-inference",
            $"Infer remediation from repository analogue {analogue.ProductType} ({analogue.Family} {analogue.Version}). No platform-specific remediation class is used.",
            missingChanges.DistinctBy(x => (x.Path, x.ChangeType, x.Description)).ToArray(),
            ["All generated C# is syntax-checked; source changes are retained only after build, tests, and reporter verification pass."]);
    }
}

internal static class MissingSynthesizer
{
    public static void Apply(RepositoryContext context, RepositoryModel model, CoverageDiscrepancy target, ChangeCollector changes)
    {
        var source = model.FindBestAnalogue(target);
        var map = TransformationMap.Create(source, target);

        CloneSymbolDependencies(context, changes, source, map);
        CloneOwnedFile(context, changes, source.ProductFile, source.ProductType, map.TargetProductType, map);
        if(source.CreatorFile is not null && source.CreatorType is not null)
            CloneOwnedFile(context, changes, source.CreatorFile, source.CreatorType, map.TargetCreatorType, map);
        if(source.BrowserCreatorFile is not null && source.BrowserCreatorType is not null && target.Kind == PlatformRequirementKind.DesktopPlatform)
            CloneOwnedFile(context, changes, source.BrowserCreatorFile, source.BrowserCreatorType, map.TargetBrowserCreatorType, map);

        foreach(var group in source.Registrations.GroupBy(x => x.File, StringComparer.OrdinalIgnoreCase))
            InsertTransformedRegistrations(context, changes, group.Key, group.OrderByDescending(x => x.LineIndex).ToArray(), source, target, map);

        if(!SameFamily(source.Family, target.Family))
            foreach(var helper in source.HelperMethods.OrderByDescending(x => x.StartLine))
                CloneHelper(changes, helper, map);
    }

    private static void CloneSymbolDependencies(
        RepositoryContext context,
        ChangeCollector changes,
        PlatformPrototype source,
        TransformationMap map)
    {
        CloneConstantDependencies(context, changes, source, map);
        CloneNamespaceImports(context, changes, source, map);
    }

    private static void CloneConstantDependencies(
        RepositoryContext context,
        ChangeCollector changes,
        PlatformPrototype source,
        TransformationMap map)
    {
        var referencedConstants = new HashSet<string>(StringComparer.Ordinal);
        foreach(var relative in new[] { source.ProductFile, source.CreatorFile, source.BrowserCreatorFile }.Where(x => x is not null)!)
        {
            var text = File.ReadAllText(context.PathFromRoot(relative!));
            foreach(Match match in Regex.Matches(text, @"SauceryConstants\.(?<name>[A-Z][A-Z0-9_]*)"))
                referencedConstants.Add(match.Groups["name"].Value);
        }
        foreach(var registration in source.Registrations)
            foreach(Match match in Regex.Matches(registration.Line, @"SauceryConstants\.(?<name>[A-Z][A-Z0-9_]*)"))
                referencedConstants.Add(match.Groups["name"].Value);

        const string constantsFile = "Saucery.Core/Util/SauceryConstants.cs";
        changes.Modify(constantsFile, text =>
        {
            var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
            foreach(var sourceName in referencedConstants)
            {
                var targetName = map.Transform(sourceName);
                if(targetName == sourceName || Regex.IsMatch(text, $@"\b{Regex.Escape(targetName)}\b")) continue;

                var sourceLine = lines.FindIndex(line => Regex.IsMatch(line, $@"\bconst\s+string\s+{Regex.Escape(sourceName)}\s*="));
                if(sourceLine < 0) continue;

                var transformed = map.Transform(lines[sourceLine]);
                if(transformed == lines[sourceLine] || lines.Any(line => line.Trim() == transformed.Trim())) continue;
                lines.Insert(sourceLine + 1, transformed);
            }
            return string.Join(Environment.NewLine, lines);
        });
    }

    private static void CloneNamespaceImports(
        RepositoryContext context,
        ChangeCollector changes,
        PlatformPrototype source,
        TransformationMap map)
    {
        foreach(var file in source.Registrations.Select(x => x.File).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if(!file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) continue;
            changes.Modify(file, text =>
            {
                var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
                var additions = new List<(int Index, string Line)>();
                for(var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    if(!line.TrimStart().StartsWith("using ", StringComparison.Ordinal)) continue;
                    var transformed = map.Transform(line);
                    if(transformed == line || lines.Any(existing => existing.Trim() == transformed.Trim())) continue;

                    // Only clone imports changed by the analogue transformation. This keeps the
                    // complete type slice coherent without copying unrelated namespace imports.
                    additions.Add((i + 1, transformed));
                }

                foreach(var addition in additions.OrderByDescending(x => x.Index))
                    lines.Insert(addition.Index, addition.Line);
                return string.Join(Environment.NewLine, lines);
            });
        }
    }

    private static void CloneOwnedFile(RepositoryContext context, ChangeCollector changes, string sourceRelative, string sourceType, string targetType, TransformationMap map)
    {
        if(sourceType == targetType) return;

        // Transform the path as well as the file contents. This prevents a newly inferred family
        // from being emitted into the source family's folder merely because that was the analogue.
        var transformedPath = map.Transform(sourceRelative.Replace('\\', '/'));
        var directory = Path.GetDirectoryName(transformedPath)?.Replace('\\', '/') ?? string.Empty;
        var targetRelative = Path.Combine(directory, Path.GetFileName(transformedPath).Replace(sourceType, targetType, StringComparison.Ordinal))
            .Replace('\\', '/');

        if(File.Exists(context.PathFromRoot(targetRelative))) return;
        changes.Write(targetRelative, map.Transform(File.ReadAllText(context.PathFromRoot(sourceRelative))));
    }

    private static void InsertTransformedRegistrations(
        RepositoryContext context,
        ChangeCollector changes,
        string file,
        IReadOnlyList<RegistrationSite> sites,
        PlatformPrototype source,
        CoverageDiscrepancy target,
        TransformationMap map)
    {
        changes.Modify(file, text =>
        {
            var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
            foreach(var site in sites)
            {
                // Earlier edits (especially cloned using directives) can shift every stored line
                // number. Relocate the exact analogue registration in the current source instead
                // of trusting RepositoryModel's original index.
                var currentIndex = lines.FindIndex(line => line.Trim() == site.Line.Trim());
                if(currentIndex < 0) continue;

                var line = lines[currentIndex];
                if(IsMethodDeclaration(line)) continue;
                if(!RegistrationApplies(line, source, target)) continue;
                var transformed = map.Transform(line);
                if(transformed == line || lines.Any(x => x.Trim() == transformed.Trim())) continue;

                // Never commit an inferred text insertion unless the entire containing C# file
                // still parses. This turns a bad inference into a skipped candidate instead of
                // corrupting the repository and aborting the workflow.
                var candidateLines = new List<string>(lines);
                candidateLines.Insert(currentIndex + 1, transformed);
                var candidate = string.Join(Environment.NewLine, candidateLines);
                try
                {
                    CSharpGuard.AssertValid(candidate, context.PathFromRoot(file));
                    lines = candidateLines;
                }
                catch(InvalidDataException ex)
                {
                    Console.WriteLine($"    Skipping unsafe inferred registration in {file}: {ex.Message.Split(Environment.NewLine)[0]}");
                }
            }
            return string.Join(Environment.NewLine, lines);
        });
    }

    private static bool IsMethodDeclaration(string line) =>
        Regex.IsMatch(line, @"^\s*(?:public|private|internal|protected)\s+(?:static\s+)?[A-Za-z_][A-Za-z0-9_<>,?\[\]\s]*\s+[A-Za-z_][A-Za-z0-9_]*\s*\(");

    private static bool RegistrationApplies(string line, PlatformPrototype source, CoverageDiscrepancy target)
    {
        if(target.Kind == PlatformRequirementKind.MobileMajorWildcard)
        {
            if(!string.IsNullOrEmpty(source.Version) && line.Contains($"\"{source.Version}\"", StringComparison.Ordinal)) return false;
            return line.Contains(source.ProductType, StringComparison.Ordinal) ||
                   (source.CreatorType is not null && line.Contains(source.CreatorType, StringComparison.Ordinal)) ||
                   line.Contains("Real", StringComparison.OrdinalIgnoreCase) || line.Contains("Device", StringComparison.OrdinalIgnoreCase);
        }

        if(target.Kind == PlatformRequirementKind.MobileExactVersion)
        {
            var major = Major(source.Version);
            if(!string.IsNullOrEmpty(major) && line.Contains($"\"{major}\"", StringComparison.Ordinal) &&
               !line.Contains($"\"{source.Version}\"", StringComparison.Ordinal)) return false;
        }
        return true;
    }

    private static void CloneHelper(ChangeCollector changes, MethodSite helper, TransformationMap map)
    {
        var transformed = map.Transform(helper.Text);
        if(transformed == helper.Text) return;

        changes.Modify(helper.File, text =>
        {
            var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal);
            var originalHelper = helper.Text.Replace("\r\n", "\n", StringComparison.Ordinal);
            var transformedHelper = transformed.Replace("\r\n", "\n", StringComparison.Ordinal);

            if(normalized.Contains(transformedHelper.Trim(), StringComparison.Ordinal)) return text;

            // Do not use the RepositoryModel line numbers here. Earlier registration edits may
            // already have shifted this file. Locate the exact method in the current source.
            var methodIndex = normalized.IndexOf(originalHelper, StringComparison.Ordinal);
            if(methodIndex < 0)
                throw new InvalidDataException($"Could not relocate analogue helper in {helper.File} after earlier edits.");

            var insert = methodIndex + originalHelper.Length;
            normalized = normalized.Insert(insert, "\n\n" + transformedHelper);
            return normalized.Replace("\n", Environment.NewLine, StringComparison.Ordinal);
        });
    }

    private static bool SameFamily(string a, string b) => Normalize(a) == Normalize(b);
    private static string Normalize(string value) => Regex.Replace(value, "[^A-Za-z0-9]", "").ToUpperInvariant();
    private static string Major(string version) => Regex.Match(version, @"^\d+").Value;
}

internal static class ObsoletePlatformIdentity
{
    public static string EffectiveVersion(CoverageDiscrepancy target)
    {
        if(!string.IsNullOrWhiteSpace(target.Version))
            return target.Version!;

        var displayName = target.DisplayName ?? string.Empty;

        if(target.Family.Equals("Windows", StringComparison.OrdinalIgnoreCase))
        {
            var match = Regex.Match(
                displayName,
                @"\bWindows\s+(?<version>\d+(?:\.\d+)?)\b",
                RegexOptions.IgnoreCase);

            if(match.Success)
                return match.Groups["version"].Value;
        }

        if(target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase))
        {
            var match = Regex.Match(
                displayName,
                @"\biOS\s+(?<version>\d+(?:\.\d+|\.x)?)\b",
                RegexOptions.IgnoreCase);

            if(match.Success)
                return match.Groups["version"].Value;
        }

        if(target.Family.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            var match = Regex.Match(
                displayName,
                @"\bAndroid\s+(?<version>\d+(?:\.\d+|\.x)?)\b",
                RegexOptions.IgnoreCase);

            if(match.Success)
                return match.Groups["version"].Value;
        }

        return string.Empty;
    }

    public static string? Exact(CoverageDiscrepancy target)
    {
        var version = EffectiveVersion(target);

        if(target.Family.Equals("Windows", StringComparison.OrdinalIgnoreCase))
        {
            var digits = Regex.Replace(version, @"\D", "");
            return string.IsNullOrWhiteSpace(digits)
                ? null
                : $"SauceryConstants.PLATFORM_WINDOWS_{digits}";
        }

        return null;
    }
}

internal static class ObsoleteDeclarationResolver
{
    public static bool IsDeclarationOnlyExactVersion(
        RepositoryContext context,
        CoverageDiscrepancy target)
    {
        if(!target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase) &&
           !target.Family.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var version = ObsoletePlatformIdentity.EffectiveVersion(target);
        if(!Regex.IsMatch(version, @"^\d+\.\d+$"))
            return false;

        var major = Regex.Match(version, @"^\d+").Value;
        if(string.IsNullOrWhiteSpace(major))
            return false;

        var prefix = target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase)
            ? "IOS"
            : "Android";

        var expectedSharedType = $"{prefix}{major}Platform";
        var exactDigits = Regex.Replace(version, @"\D", "");
        var exactType = $"{prefix}{exactDigits}Platform";

        // If an exact-version product exists, this is not declaration-only.
        if(HasClass(context, "/ConcreteProducts/", exactType))
            return false;

        // A shared major-family product means the exact version is only a registration/declaration.
        if(HasClass(context, "/ConcreteProducts/", expectedSharedType))
            return true;

        return false;
    }

    private static bool HasClass(
        RepositoryContext context,
        string requiredPathSegment,
        string typeName) =>
        Directory.EnumerateFiles(context.Core, "*.cs", SearchOption.AllDirectories)
            .Where(file => file.Replace('\\', '/')
                .Contains(requiredPathSegment, StringComparison.OrdinalIgnoreCase))
            .Any(file => Regex.IsMatch(
                File.ReadAllText(file),
                $@"\bclass\s+{Regex.Escape(typeName)}\b"));
}

internal sealed record ObsoleteTarget(
    string ProductType,
    string ProductFile,
    string? CreatorType,
    string? CreatorFile,
    string? BrowserCreatorType,
    string? BrowserCreatorFile);

internal static class ObsoleteTargetResolver
{
    public static ObsoleteTarget Resolve(
        RepositoryContext context,
        CoverageDiscrepancy target)
    {
        if(ObsoleteDeclarationResolver.IsDeclarationOnlyExactVersion(context, target))
        {
            return new(
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                null);
        }

        var productType = ExactProductType(target)
            ?? throw new InvalidDataException(
                $"No deterministic ConcreteProduct type rule exists for obsolete platform {target.DisplayName}.");

        var productFile = FindExactClassFile(
            context,
            "/ConcreteProducts/",
            productType,
            required: true)!;

        var creatorType = productType.EndsWith("Platform", StringComparison.Ordinal)
            ? productType[..^"Platform".Length] + "PlatformCreator"
            : throw new InvalidDataException(
                $"ConcreteProduct type {productType} does not end in Platform.");

        var creatorFile = FindExactClassFile(
            context,
            "/ConcreteCreators/",
            creatorType,
            required: true)!;

        var browserCreatorType = FindBrowserCreatorTypeFromRegistration(
            context,
            target);

        string? browserCreatorFile = null;

        if(browserCreatorType is not null)
        {
            browserCreatorFile = FindExactClassFile(
                context,
                "/BrowserCreators/",
                browserCreatorType,
                required: true);
        }
        else if(target.Family.Equals("Windows", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                $"No BrowserCreator registration was found in BrowserFactory.cs for obsolete platform {target.DisplayName}. Refusing partial remediation.");
        }

        return new(
            productType,
            productFile,
            creatorType,
            creatorFile,
            browserCreatorType,
            browserCreatorFile);
    }

    private static string? ExactProductType(CoverageDiscrepancy target)
    {
        var version = ObsoletePlatformIdentity.EffectiveVersion(target);
        var major = Regex.Match(version, @"^\d+").Value;
        var digits = Regex.Replace(version, @"\D", "");

        if(target.Family.Equals("Windows", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(digits)
                ? null
                : $"Windows{digits}Platform";
        }

        if(target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(major)
                ? null
                : $"IOS{major}Platform";
        }

        if(target.Family.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(major)
                ? null
                : $"Android{major}Platform";
        }

        return null;
    }

    private static string? FindBrowserCreatorTypeFromRegistration(
        RepositoryContext context,
        CoverageDiscrepancy target)
    {
        var path = context.PathFromRoot(
            "Saucery.Core/Dojo/Browsers/BrowserFactory.cs");

        if(!File.Exists(path))
            return null;

        foreach(var line in File.ReadLines(path))
        {
            if(!IsExactRegistrationLine(line, target))
                continue;

            var match = Regex.Match(
                line,
                @"\bnew\s+(?<type>[A-Za-z_][A-Za-z0-9_]*BrowserCreator)\s*\(");

            if(match.Success)
                return match.Groups["type"].Value;
        }

        return null;
    }

    private static bool IsExactRegistrationLine(
        string line,
        CoverageDiscrepancy target)
    {
        if(string.IsNullOrWhiteSpace(line))
            return false;

        var identity = ObsoletePlatformIdentity.Exact(target);

        if(!string.IsNullOrWhiteSpace(identity) &&
           Regex.IsMatch(
               line,
               $@"(?<![A-Za-z0-9_]){Regex.Escape(identity)}(?![A-Za-z0-9_])"))
        {
            return true;
        }

        var display = target.DisplayName ?? string.Empty;
        if(!string.IsNullOrWhiteSpace(display) &&
           Regex.IsMatch(
               line,
               $@"[""']{Regex.Escape(display)}[""']",
               RegexOptions.IgnoreCase))
        {
            return true;
        }

        var version = ObsoletePlatformIdentity.EffectiveVersion(target);
        return !string.IsNullOrWhiteSpace(version) &&
               Regex.IsMatch(
                   line,
                   $@"[""'](?:{Regex.Escape(target.Family)}\s+)?{Regex.Escape(version)}[""']",
                   RegexOptions.IgnoreCase);
    }

    private static string? FindExactClassFile(
        RepositoryContext context,
        string requiredPathSegment,
        string typeName,
        bool required)
    {
        var matches = Directory
            .EnumerateFiles(context.Core, "*.cs", SearchOption.AllDirectories)
            .Where(file => file
                .Replace('\\', '/')
                .Contains(requiredPathSegment, StringComparison.OrdinalIgnoreCase))
            .Where(file => Regex.IsMatch(
                File.ReadAllText(file),
                $@"\bclass\s+{Regex.Escape(typeName)}\b"))
            .Select(context.Relative)
            .ToArray();

        if(matches.Length == 1)
            return matches[0];

        if(matches.Length > 1)
        {
            throw new InvalidDataException(
                $"Multiple exact class declarations found for {typeName}: {string.Join(", ", matches)}");
        }

        if(required)
        {
            throw new InvalidDataException(
                $"No exact class declaration was found for {typeName}.");
        }

        return null;
    }
}

internal static class ObsoleteSynthesizer
{
    public static void Apply(
        RepositoryContext context,
        RepositoryModel model,
        CoverageDiscrepancy target,
        ChangeCollector changes)
    {
        _ = model;

        if(ObsoleteDeclarationResolver.IsDeclarationOnlyExactVersion(context, target))
        {
            Console.WriteLine($"    Preserving shared major-family product/creator for exact obsolete version: {target.DisplayName}");
            RemoveExactPlatformDeclarations(context, changes, target);
            RemoveObsoleteUnitTestData(context, changes, target, Array.Empty<string>());
            return;
        }

        var obsolete = ObsoleteTargetResolver.Resolve(context, target);

        Console.WriteLine($"    Deleting obsolete ConcreteProduct: {obsolete.ProductFile}");
        changes.Delete(obsolete.ProductFile);

        if(obsolete.CreatorFile is not null)
        {
            Console.WriteLine($"    Deleting obsolete PlatformCreator: {obsolete.CreatorFile}");
            changes.Delete(obsolete.CreatorFile);
        }

        if(!string.IsNullOrWhiteSpace(obsolete.BrowserCreatorFile))
        {
            Console.WriteLine($"    Deleting obsolete BrowserCreator: {obsolete.BrowserCreatorFile}");
            changes.Delete(obsolete.BrowserCreatorFile);
        }

        var deletedTypes = new[]
            {
                obsolete.ProductType,
                obsolete.CreatorType,
                obsolete.BrowserCreatorType
            }
            .Where(type => !string.IsNullOrWhiteSpace(type))
            .Select(type => type!)
            .ToArray();

        var deletedFiles = new[]
            {
                obsolete.ProductFile,
                obsolete.CreatorFile,
                obsolete.BrowserCreatorFile
            }
            .Where(file => !string.IsNullOrWhiteSpace(file))
            .Select(file => file!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        RemoveExactTypeReferences(context, changes, deletedTypes, deletedFiles);

        // Keep the repository's declaration surface internally consistent with the
        // deleted implementation.
        RemoveExactPlatformDeclarations(context, changes, target);

        RemoveObsoleteUnitTestData(context, changes, target, deletedTypes);
    }

    private static void RemoveExactPlatformDeclarations(
        RepositoryContext context,
        ChangeCollector changes,
        CoverageDiscrepancy target)
    {
        var files = new[]
        {
            "Saucery.Core/Dojo/Browsers/BrowserFactory.cs",
            "Saucery.Core/Dojo/DojoExtensions.cs"
        };

        foreach(var relative in files)
        {
            var path = context.PathFromRoot(relative);
            if(!File.Exists(path))
                continue;

            var source = changes.Read(relative);
            var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
            var changed = false;

            for(var i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if(!IsExactPlatformDeclarationLine(line, target))
                    continue;

                var candidate = new List<string>(lines);
                candidate.RemoveAt(i);
                var candidateText = string.Join(Environment.NewLine, candidate);

                try
                {
                    CSharpGuard.AssertValid(candidateText, path);
                }
                catch(InvalidDataException)
                {
                    throw new InvalidDataException(
                        $"Exact obsolete declaration for {target.DisplayName} was found in {relative}, but removing its line would make the file syntactically invalid. Refusing broader cleanup.");
                }

                Console.WriteLine(
                    $"    Removing exact obsolete declaration from {relative}: {line.Trim()}");

                lines = candidate;
                changed = true;
            }

            if(changed)
                changes.Write(relative, string.Join(Environment.NewLine, lines));
        }
    }

    private static bool IsExactPlatformDeclarationLine(
        string line,
        CoverageDiscrepancy target)
    {
        if(string.IsNullOrWhiteSpace(line))
            return false;

        var identity = ObsoletePlatformIdentity.Exact(target);
        if(!string.IsNullOrWhiteSpace(identity) &&
           ContainsExactIdentifier(line, identity))
            return true;

        var version = ObsoletePlatformIdentity.EffectiveVersion(target);

        if(target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase))
        {
            if(Regex.IsMatch(version, @"^\d+\.\d+$"))
            {
                // Exact version: match only the exact literal, e.g. 26.0.
                return Regex.IsMatch(
                    line,
                    $@"[""'](?:iOS\s+)?{Regex.Escape(version)}[""']",
                    RegexOptions.IgnoreCase);
            }

            var major = Regex.Match(version, @"^\d+").Value;
            if(!string.IsNullOrWhiteSpace(major))
            {
                return Regex.IsMatch(
                    line,
                    $@"[""']iOS\s+{Regex.Escape(major)}[""']",
                    RegexOptions.IgnoreCase);
            }
        }

        if(target.Family.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            if(Regex.IsMatch(version, @"^\d+\.\d+$"))
            {
                return Regex.IsMatch(
                    line,
                    $@"[""'](?:Android\s+)?{Regex.Escape(version)}[""']",
                    RegexOptions.IgnoreCase);
            }

            var major = Regex.Match(version, @"^\d+").Value;
            if(!string.IsNullOrWhiteSpace(major))
            {
                return Regex.IsMatch(
                    line,
                    $@"[""']Android\s+{Regex.Escape(major)}[""']",
                    RegexOptions.IgnoreCase);
            }
        }

        return false;
    }

    private static void RemoveExactTypeReferences(
        RepositoryContext context,
        ChangeCollector changes,
        IReadOnlyList<string> deletedTypes,
        IReadOnlySet<string> deletedFiles)
    {
        var roots = new[] { context.Core }.Concat(context.TestRoots);

        foreach(var path in roots
            .Where(Directory.Exists)
            .SelectMany(root => Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)))
        {
            var relative = context.Relative(path);
            if(deletedFiles.Contains(relative))
                continue;

            var source = File.ReadAllText(path);
            if(!deletedTypes.Any(type => ContainsExactIdentifier(source, type)))
                continue;

            var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
            var changed = false;

            for(var i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if(!deletedTypes.Any(type => ContainsExactIdentifier(line, type)))
                    continue;

                var candidateLines = new List<string>(lines);
                candidateLines.RemoveAt(i);
                var candidateText = string.Join(Environment.NewLine, candidateLines);

                try
                {
                    CSharpGuard.AssertValid(candidateText, path);
                }
                catch(InvalidDataException)
                {
                    continue;
                }

                Console.WriteLine($"    Removing direct reference from {relative}: {line.Trim()}");
                lines = candidateLines;
                changed = true;
            }

            if(changed)
                changes.Write(relative, string.Join(Environment.NewLine, lines));
        }
    }

    private static void RemoveObsoleteUnitTestData(
        RepositoryContext context,
        ChangeCollector changes,
        CoverageDiscrepancy target,
        IReadOnlyList<string> deletedTypes)
    {
        var platformIdentity = ObsoletePlatformIdentity.Exact(target);

        foreach(var testRoot in context.TestRoots)
        {
            var removedInventoryRows = 0;

            foreach(var path in Directory.EnumerateFiles(testRoot, "*.cs", SearchOption.AllDirectories))
            {
                var relative = context.Relative(path);
                var original = File.ReadAllText(path);
                var lines = original.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
                var changed = false;

                for(var i = lines.Count - 1; i >= 0; i--)
                {
                    var line = lines[i];

                    if(!IsExactObsoleteTestRow(line, target, platformIdentity, deletedTypes))
                        continue;

                    // PlatformExpansionTests use platform rows only as generic expansion fixtures.
                    // Removing the row changes the arithmetic under test (for example 103 -> 52).
                    // Retarget the row to a supported desktop identity instead.
                    if(relative.EndsWith("/PlatformExpansionTests.cs", StringComparison.OrdinalIgnoreCase) &&
                       !string.IsNullOrWhiteSpace(platformIdentity) &&
                       target.Family.Equals("Windows", StringComparison.OrdinalIgnoreCase))
                    {
                        const string supportedWindowsIdentity = "SauceryConstants.PLATFORM_WINDOWS_10";

                        var updatedLine = ReplaceExactIdentifier(
                            line,
                            platformIdentity,
                            supportedWindowsIdentity);

                        if(updatedLine != line)
                        {
                            Console.WriteLine(
                                $"    Retargeting expansion fixture in {relative}: {platformIdentity} -> {supportedWindowsIdentity}");
                            lines[i] = updatedLine;
                            changed = true;
                        }

                        continue;
                    }

                    var candidateLines = new List<string>(lines);
                    candidateLines.RemoveAt(i);
                    var candidateText = string.Join(Environment.NewLine, candidateLines);

                    try
                    {
                        CSharpGuard.AssertValid(candidateText, path);
                    }
                    catch(InvalidDataException)
                    {
                        continue;
                    }

                    Console.WriteLine($"    Removing obsolete test data from {relative}: {line.Trim()}");

                    if(relative.Contains("/DataProviders/", StringComparison.OrdinalIgnoreCase))
                        removedInventoryRows++;

                    lines = candidateLines;
                    changed = true;
                }

                if(changed)
                    changes.Write(relative, string.Join(Environment.NewLine, lines));
            }

            if(removedInventoryRows > 0)
                AdjustPlatformCountAssertions(
                    context,
                    changes,
                    testRoot,
                    target,
                    removedInventoryRows);
        }
    }

    private static string ReplaceExactIdentifier(
        string source,
        string oldIdentifier,
        string newIdentifier) =>
        Regex.Replace(
            source,
            $@"(?<![A-Za-z0-9_]){Regex.Escape(oldIdentifier)}(?![A-Za-z0-9_])",
            newIdentifier);


    private static bool IsExactObsoleteTestRow(
        string line,
        CoverageDiscrepancy target,
        string? platformIdentity,
        IReadOnlyList<string> deletedTypes)
    {
        if(string.IsNullOrWhiteSpace(line))
            return false;

        if(deletedTypes.Any(type => ContainsExactIdentifier(line, type)))
            return true;

        if(!string.IsNullOrWhiteSpace(platformIdentity) &&
           ContainsExactIdentifier(line, platformIdentity))
            return true;

        var version = ObsoletePlatformIdentity.EffectiveVersion(target);
        var major = Regex.Match(version, @"^\d+").Value;

        if(target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase) &&
           !string.IsNullOrWhiteSpace(major) &&
           (line.Contains("new IOSPlatform(", StringComparison.Ordinal) ||
            line.Contains("new IOSRealDevice(", StringComparison.Ordinal)))
        {
            if(Regex.IsMatch(version, @"^\d+\.\d+$"))
            {
                return Regex.IsMatch(
                    line,
                    $@"[""']{Regex.Escape(version)}[""']",
                    RegexOptions.IgnoreCase);
            }

            return Regex.IsMatch(
                line,
                $@"[""']{Regex.Escape(major)}(?:\.\d+|\.x)?[""']",
                RegexOptions.IgnoreCase);
        }

        if(target.Family.Equals("Android", StringComparison.OrdinalIgnoreCase) &&
           !string.IsNullOrWhiteSpace(major) &&
           (line.Contains("new AndroidPlatform(", StringComparison.Ordinal) ||
            line.Contains("new AndroidRealDevice(", StringComparison.Ordinal)))
        {
            if(Regex.IsMatch(version, @"^\d+\.\d+$"))
            {
                return Regex.IsMatch(
                    line,
                    $@"[""']{Regex.Escape(version)}[""']",
                    RegexOptions.IgnoreCase);
            }

            return Regex.IsMatch(
                line,
                $@"[""']{Regex.Escape(major)}(?:\.\d+|\.x)?[""']",
                RegexOptions.IgnoreCase);
        }

        return false;
    }

    private static void AdjustPlatformCountAssertions(
        RepositoryContext context,
        ChangeCollector changes,
        string testRoot,
        CoverageDiscrepancy target,
        int removedRows)
    {
        foreach(var methodName in CountTestMethodNames(target))
            AdjustSingleCountAssertion(
                context,
                changes,
                testRoot,
                methodName,
                removedRows);
    }

    private static void AdjustSingleCountAssertion(
        RepositoryContext context,
        ChangeCollector changes,
        string testRoot,
        string methodName,
        int removedRows)
    {
        foreach(var path in Directory.EnumerateFiles(testRoot, "*.cs", SearchOption.AllDirectories))
        {
            var relative = context.Relative(path);

            // ChangeCollector.Write writes immediately through the repository transaction,
            // so Read returns the latest remediated content for this file.
            var source = changes.Read(relative);

            if(!source.Contains(methodName, StringComparison.Ordinal))
                continue;

            var pattern =
                $@"(?<prefix>\b{Regex.Escape(methodName)}\s*\(\s*\)\s*\{{(?:(?!\n\s*\}}).)*?Items\.Count\(\)\.ShouldBe\()(?<count>\d+)(?<suffix>\))";

            var match = Regex.Match(source, pattern, RegexOptions.Singleline);

            if(!match.Success)
                continue;

            var oldCount = int.Parse(match.Groups["count"].Value);
            var newCount = oldCount - removedRows;

            if(newCount < 0)
                throw new InvalidDataException(
                    $"Cannot adjust {methodName} in {relative} from {oldCount} by {removedRows}.");

            var updated =
                source[..match.Groups["count"].Index] +
                newCount.ToString() +
                source[(match.Groups["count"].Index + match.Groups["count"].Length)..];

            Console.WriteLine(
                $"    Updating {relative} {methodName} expected count: {oldCount} -> {newCount}");

            changes.Write(relative, updated);
            return;
        }

        Console.WriteLine(
            $"    No {methodName} count assertion was found under {context.Relative(testRoot)}; no count adjustment was required.");
    }

    private static IReadOnlyList<string> CountTestMethodNames(CoverageDiscrepancy target)
    {
        if(target.Family.Equals("Windows", StringComparison.OrdinalIgnoreCase) ||
           target.Family.Equals("macOS", StringComparison.OrdinalIgnoreCase) ||
           target.Family.Equals("Mac", StringComparison.OrdinalIgnoreCase) ||
           target.Family.Equals("Linux", StringComparison.OrdinalIgnoreCase) ||
           target.Family.Equals("ChromiumOS", StringComparison.OrdinalIgnoreCase))
            return ["DesktopPlatformsTest", "AllPlatformsTest"];

        if(target.Family.Equals("iOS", StringComparison.OrdinalIgnoreCase))
            return ["IOSPlatformsTest"];

        if(target.Family.Equals("Android", StringComparison.OrdinalIgnoreCase))
            return ["AndroidPlatformsTest"];

        return Array.Empty<string>();
    }

    private static bool ContainsExactIdentifier(string text, string identifier) =>
        Regex.IsMatch(
            text,
            $@"(?<![A-Za-z0-9_]){Regex.Escape(identifier)}(?![A-Za-z0-9_])");
}


internal sealed class TransformationMap
{
    private readonly IReadOnlyList<(string From, string To)> replacements;
    private TransformationMap(IReadOnlyList<(string From, string To)> replacements, string product, string creator, string browser)
    {
        this.replacements = replacements;
        TargetProductType = product;
        TargetCreatorType = creator;
        TargetBrowserCreatorType = browser;
    }

    public string TargetProductType { get; }
    public string TargetCreatorType { get; }
    public string TargetBrowserCreatorType { get; }

    public static TransformationMap Create(PlatformPrototype source, CoverageDiscrepancy target)
    {
        var sameFamily = Normalize(source.Family) == Normalize(target.Family);
        var targetPrefix = sameFamily ? source.IdentifierPrefix : Identifier(target.Family);
        var targetDigits = RepositoryModel.VersionDigits(target.Version);
        var sourceDigits = RepositoryModel.VersionDigits(source.Version);
        var sourceVersioned = Regex.IsMatch(source.ProductType, @"\d+Platform$");
        var targetProduct = targetPrefix + (sourceVersioned || !string.IsNullOrEmpty(target.Version) ? targetDigits : "") + "Platform";
        var targetCreator = targetProduct + "Creator";
        var targetBrowser = targetPrefix + "BrowserCreator";

        var list = new List<(string, string)>
        {
            (source.ProductType, targetProduct)
        };
        if(source.CreatorType is not null) list.Add((source.CreatorType, targetCreator));
        if(source.BrowserCreatorType is not null) list.Add((source.BrowserCreatorType, targetBrowser));

        if(!sameFamily)
        {
            list.Add((source.IdentifierPrefix, targetPrefix));
            list.Add((source.Family, target.Family));
            list.Add((ConstantToken(source.Family), ConstantToken(target.Family)));
        }

        if(!string.IsNullOrEmpty(source.Version) && !string.IsNullOrEmpty(target.Version))
        {
            var sourceExact = source.Version.Replace(".x", ".0", StringComparison.OrdinalIgnoreCase);
            var targetExact = target.Version.Replace(".x", ".0", StringComparison.OrdinalIgnoreCase);
            list.Add((sourceExact, targetExact));
            var sourceMajor = Regex.Match(source.Version, @"^\d+").Value;
            var targetMajor = Regex.Match(target.Version, @"^\d+").Value;
            if(sourceMajor != sourceExact && !string.IsNullOrEmpty(sourceMajor)) list.Add(($"\"{sourceMajor}\"", $"\"{targetMajor}\""));
            if(!string.IsNullOrEmpty(sourceDigits) && !string.IsNullOrEmpty(targetDigits))
                list.Add((source.IdentifierPrefix + sourceDigits, targetPrefix + targetDigits));
        }

        return new TransformationMap(list.Where(x => x.Item1 != x.Item2 && !string.IsNullOrEmpty(x.Item1)).Distinct().OrderByDescending(x => x.Item1.Length).ToArray(), targetProduct, targetCreator, targetBrowser);
    }

    public string Transform(string input)
    {
        foreach(var (from, to) in replacements) input = input.Replace(from, to, StringComparison.Ordinal);
        return input;
    }

    private static string Identifier(string family)
    {
        var id = Regex.Replace(family, "[^A-Za-z0-9]", "");
        if(id.Equals("iOS", StringComparison.OrdinalIgnoreCase)) return "IOS";
        if(id.Equals("macOS", StringComparison.OrdinalIgnoreCase)) return "Mac";
        return id.Length == 0 ? "Platform" : char.ToUpperInvariant(id[0]) + id[1..];
    }

    private static string ConstantToken(string family)
    {
        var identifier = Identifier(family);
        var token = Regex.Replace(identifier, @"(?<=[a-z0-9])(?=[A-Z])", "_");
        token = Regex.Replace(token, @"(?<=[A-Z])(?=[A-Z][a-z])", "_");
        return "PLATFORM_" + Regex.Replace(token, "[^A-Za-z0-9]+", "_").Trim('_').ToUpperInvariant();
    }
    private static string Normalize(string value) => Regex.Replace(value, "[^A-Za-z0-9]", "").ToUpperInvariant();
}
