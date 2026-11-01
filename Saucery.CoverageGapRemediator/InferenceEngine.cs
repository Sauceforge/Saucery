using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

internal sealed class GenericRemediationEngine
{
    public RemediationPlan BuildPlan(RepositoryContext context, string reportPath, IReadOnlyList<CoverageDiscrepancy> discrepancies, Options options)
    {
        var model = RepositoryModel.Build(context);
        var items = discrepancies.Select(d => PlanOne(model, context, d, options)).ToArray();
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
            return new(d, RemediationDisposition.ManualReview, "repository-inference",
                "The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed.",
                Array.Empty<FileChange>(), ["Obsolete-candidate removal is opt-in because a Sauce catalogue can be incomplete."]);

        PlatformPrototype analogue;
        try
        {
            analogue = d.Type == DiscrepancyType.ObsoleteCandidate ? model.FindObsoletePrototype(d) : model.FindBestAnalogue(d);
        }
        catch(Exception ex)
        {
            return new(d, RemediationDisposition.ManualReview, "repository-inference",
                "No existing repository implementation can be matched safely.",
                Array.Empty<FileChange>(), [ex.Message]);
        }

        var changes = new List<FileChange>();
        if(d.Type == DiscrepancyType.Missing)
        {
            changes.Add(new(analogue.ProductFile, "Clone/transform", $"Use {analogue.ProductType} as the structural product analogue."));
            if(analogue.CreatorFile is not null) changes.Add(new(analogue.CreatorFile, "Clone/transform", $"Use {analogue.CreatorType} as the creator analogue."));
            if(analogue.BrowserCreatorFile is not null) changes.Add(new(analogue.BrowserCreatorFile, "Clone/transform", $"Use {analogue.BrowserCreatorType} as the browser-creator analogue."));
            changes.AddRange(analogue.Registrations.Select(x => new FileChange(x.File, "Infer registration", "Transform an analogous registration/test line when semantically applicable.")));
            changes.AddRange(analogue.HelperMethods.Select(x => new FileChange(x.File, "Infer helper", "Clone a family-specific helper when a new family requires one.")));
        }
        else
        {
            changes.Add(new(analogue.ProductFile, "Delete if unreferenced", $"Remove {analogue.ProductType} only after obsolete registrations are removed and no external references remain."));
            if(analogue.CreatorFile is not null) changes.Add(new(analogue.CreatorFile, "Delete if unreferenced", $"Remove {analogue.CreatorType} only if no surviving registration still uses it."));
            if(analogue.BrowserCreatorFile is not null) changes.Add(new(analogue.BrowserCreatorFile, "Delete if unreferenced", $"Remove {analogue.BrowserCreatorType} only if no surviving registration still uses it."));
            changes.AddRange(analogue.Registrations.Select(x => new FileChange(x.File, "Remove matching registration", "Remove only registrations/tests whose version semantics match the obsolete requirement.")));
        }

        return new(d, RemediationDisposition.AutoFix, "repository-inference",
            $"Infer remediation from repository analogue {analogue.ProductType} ({analogue.Family} {analogue.Version}). No platform-specific remediation class is used.",
            changes.DistinctBy(x => (x.Path, x.ChangeType, x.Description)).ToArray(),
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

internal static class ObsoleteSynthesizer
{
    public static void Apply(RepositoryContext context, RepositoryModel model, CoverageDiscrepancy target, ChangeCollector changes)
    {
        var prototype = model.FindObsoletePrototype(target);
        var ownedFiles = new[] { prototype.ProductFile, prototype.CreatorFile, prototype.BrowserCreatorFile }
            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!).Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var symbols = new[] { prototype.ProductType, prototype.CreatorType, prototype.BrowserCreatorType }
            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!).Distinct(StringComparer.Ordinal).ToArray();
        var constants = DiscoverPrototypeConstants(context, prototype);

        foreach(var relative in EnumerateRemovalCandidates(context, prototype, target))
        {
            if(ownedFiles.Contains(relative)) continue;
            var path = context.PathFromRoot(relative);
            if(!File.Exists(path)) continue;

            var original = File.ReadAllText(path);
            var edited = RemoveTargetReferences(original, relative, target, symbols, constants);
            if(edited != original) changes.Write(relative, edited);
        }

        RemoveExactEvidenceLine(context, changes, target, ownedFiles);

        if(!HasExternalReferences(context, ownedFiles, symbols))
            foreach(var file in ownedFiles) changes.Delete(file);

        RemoveUnusedConstants(context, changes, constants);
    }

    private static IEnumerable<string> EnumerateRemovalCandidates(
        RepositoryContext context,
        PlatformPrototype prototype,
        CoverageDiscrepancy target)
    {
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Production edits are restricted to files the repository model already identified
        // as registrations for this exact prototype. Never scan unrelated platform classes.
        foreach(var registration in prototype.Registrations)
            files.Add(registration.File);

        if(!string.IsNullOrWhiteSpace(target.EvidenceFile))
            files.Add(target.EvidenceFile);

        // Tests are data-driven and may not reference the platform product type directly,
        // so scan all recognized test projects for matching family/version test data.
        foreach(var root in context.TestRoots.Where(Directory.Exists))
            foreach(var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
                files.Add(context.Relative(file));

        return files;
    }

    private static IEnumerable<string> EnumerateAllSourceFiles(RepositoryContext context)
    {
        var roots = new[] { context.Core }.Concat(context.TestRoots);
        return roots.Where(Directory.Exists)
            .SelectMany(root => Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
            .Select(context.Relative)
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<string> DiscoverPrototypeConstants(
        RepositoryContext context,
        PlatformPrototype prototype)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        var structuralSymbols = new[]
        {
            prototype.ProductType,
            prototype.CreatorType,
            prototype.BrowserCreatorType
        }
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Select(x => x!)
        .ToArray();

        foreach(var registration in prototype.Registrations)
        {
            if(!structuralSymbols.Any(symbol =>
                   registration.Line.Contains(symbol, StringComparison.Ordinal)))
                continue;

            foreach(Match match in Regex.Matches(
                        registration.Line,
                        @"SauceryConstants\.(?<name>PLATFORM_[A-Z0-9_]+)"))
            {
                result.Add(match.Groups["name"].Value);
            }
        }

        return result.ToArray();
    }

    private static string RemoveTargetReferences(string source, string relative, CoverageDiscrepancy target, IReadOnlyList<string> symbols, IReadOnlyList<string> constants)
    {
        var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
        var changed = false;
        for(var i = lines.Count - 1; i >= 0; i--)
        {
            if(!ShouldRemoveLine(lines[i], relative, target, symbols, constants)) continue;
            var candidate = new List<string>(lines);
            candidate.RemoveAt(i);
            var candidateText = string.Join(Environment.NewLine, candidate);
            try
            {
                CSharpGuard.AssertValid(candidateText, relative);
                lines = candidate;
                changed = true;
            }
            catch(InvalidDataException)
            {
                // Never partially delete multi-line syntax or class bodies.
            }
        }
        return changed ? string.Join(Environment.NewLine, lines) : source;
    }

    private static bool ShouldRemoveLine(string line, string relative, CoverageDiscrepancy target, IReadOnlyList<string> symbols, IReadOnlyList<string> constants)
    {
        if(string.IsNullOrWhiteSpace(line) || IsDeclarationLine(line)) return false;
        var evidence = EvidenceText(target);
        if(evidence is not null && line.Trim() == evidence) return true;

        var hasSymbol = symbols.Any(symbol => line.Contains(symbol, StringComparison.Ordinal));
        var hasConstant = constants.Any(name => line.Contains($"SauceryConstants.{name}", StringComparison.Ordinal));
        var isTest = relative.StartsWith("Saucery.Core.Tests", StringComparison.OrdinalIgnoreCase);

        if(target.Kind == PlatformRequirementKind.MobileExactVersion)
            return ExactVersionMatches(line, target) && (hasSymbol || hasConstant || (isTest && HasFamilyContext(line, target.Family)));

        if(target.Kind == PlatformRequirementKind.MobileMajorWildcard)
        {
            var majorMatches = MajorMatches(line, target.Version);
            return ((hasSymbol || hasConstant) && majorMatches) || (isTest && majorMatches && HasFamilyContext(line, target.Family));
        }

        if(target.Kind == PlatformRequirementKind.DesktopPlatform)
        {
            if(hasSymbol || hasConstant) return true;
            return isTest && ExactVersionMatches(line, target) && HasFamilyContext(line, target.Family);
        }

        return false;
    }

    private static bool IsDeclarationLine(string line) =>
        Regex.IsMatch(line, @"^\s*(?:public|internal|private|protected)?\s*(?:sealed\s+|abstract\s+|static\s+|partial\s+)*class\s+") ||
        Regex.IsMatch(line, @"^\s*namespace\s+");

    private static string? EvidenceText(CoverageDiscrepancy target)
    {
        if(string.IsNullOrWhiteSpace(target.Evidence)) return null;
        return Regex.Replace(target.Evidence, @"^line\s+\d+:\s*", "", RegexOptions.IgnoreCase).Trim();
    }

    private static bool ExactVersionMatches(string line, CoverageDiscrepancy target)
    {
        if(string.IsNullOrWhiteSpace(target.Version)) return true;
        var version = target.Version.Replace(".x", "", StringComparison.OrdinalIgnoreCase);
        var family = Regex.Escape(target.Family);
        var escaped = Regex.Escape(version);
        return Regex.IsMatch(line, $"\"(?:{family}\\s+)?{escaped}\"", RegexOptions.IgnoreCase) ||
               line.Contains($"\"{version}\"", StringComparison.OrdinalIgnoreCase);
    }

    private static bool MajorMatches(string line, string version)
    {
        var major = Regex.Match(version ?? string.Empty, @"^\d+").Value;
        if(string.IsNullOrEmpty(major)) return false;
        return Regex.IsMatch(line, $"\"(?:iOS|Android|Linux)?\\s*{Regex.Escape(major)}(?:\\.0|\\.x)?\"", RegexOptions.IgnoreCase);
    }

    private static bool HasFamilyContext(string line, string family)
    {
        if(line.Contains(family, StringComparison.OrdinalIgnoreCase)) return true;
        return family switch
        {
            "iOS" => line.Contains("IOS", StringComparison.OrdinalIgnoreCase) || line.Contains("Apple", StringComparison.OrdinalIgnoreCase),
            "Android" => line.Contains("Android", StringComparison.OrdinalIgnoreCase),
            "macOS" => line.Contains("Mac", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static void RemoveExactEvidenceLine(RepositoryContext context, ChangeCollector changes, CoverageDiscrepancy target, IReadOnlySet<string> ownedFiles)
    {
        if(string.IsNullOrWhiteSpace(target.EvidenceFile) || string.IsNullOrWhiteSpace(target.Evidence) || ownedFiles.Contains(target.EvidenceFile)) return;
        var path = context.PathFromRoot(target.EvidenceFile);
        if(!File.Exists(path)) return;
        var evidence = EvidenceText(target);
        if(evidence is null) return;
        changes.Modify(target.EvidenceFile, source =>
        {
            var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
            var index = lines.FindIndex(line => line.Trim() == evidence);
            if(index < 0) return source;
            var candidate = new List<string>(lines);
            candidate.RemoveAt(index);
            var candidateText = string.Join(Environment.NewLine, candidate);
            CSharpGuard.AssertValid(candidateText, path);
            return candidateText;
        });
    }

    private static bool HasExternalReferences(RepositoryContext context, IReadOnlySet<string> ownedFiles, IReadOnlyList<string> symbols)
    {
        foreach(var relative in EnumerateAllSourceFiles(context))
        {
            if(ownedFiles.Contains(relative)) continue;
            var text = File.ReadAllText(context.PathFromRoot(relative));
            if(symbols.Any(symbol => Regex.IsMatch(text, $@"\b{Regex.Escape(symbol)}\b"))) return true;
        }
        return false;
    }

    private static void RemoveUnusedConstants(RepositoryContext context, ChangeCollector changes, IReadOnlyList<string> constants)
    {
        const string constantsFile = "Saucery.Core/Util/SauceryConstants.cs";
        if(!File.Exists(context.PathFromRoot(constantsFile))) return;
        foreach(var constant in constants)
        {
            if(IsConstantReferencedElsewhere(context, constantsFile, constant)) continue;
            changes.Modify(constantsFile, source =>
            {
                var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();
                var index = lines.FindIndex(line => Regex.IsMatch(line, $@"\bconst\s+string\s+{Regex.Escape(constant)}\s*="));
                if(index < 0) return source;
                lines.RemoveAt(index);
                return string.Join(Environment.NewLine, lines);
            });
        }
    }

    private static bool IsConstantReferencedElsewhere(RepositoryContext context, string constantsFile, string constant)
    {
        foreach(var relative in EnumerateAllSourceFiles(context))
        {
            if(relative.Equals(constantsFile, StringComparison.OrdinalIgnoreCase)) continue;
            var text = File.ReadAllText(context.PathFromRoot(relative));
            if(text.Contains($"SauceryConstants.{constant}", StringComparison.Ordinal)) return true;
        }
        return false;
    }
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
