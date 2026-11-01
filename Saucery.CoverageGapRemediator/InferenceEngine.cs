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

        PlatformPrototype? analogue = null;
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
            changes.Add(new(analogue!.ProductFile, "Clone/transform", $"Use {analogue!.ProductType} as the structural product analogue."));
            if(analogue!.CreatorFile is not null) changes.Add(new(analogue!.CreatorFile, "Clone/transform", $"Use {analogue!.CreatorType} as the creator analogue."));
            if(analogue!.BrowserCreatorFile is not null) changes.Add(new(analogue!.BrowserCreatorFile, "Clone/transform", $"Use {analogue!.BrowserCreatorType} as the browser-creator analogue."));
            changes.AddRange(analogue.Registrations.Select(x => new FileChange(x.File, "Infer registration", "Transform an analogous registration/test line when semantically applicable.")));
            changes.AddRange(analogue.HelperMethods.Select(x => new FileChange(x.File, "Infer helper", "Clone a family-specific helper when a new family requires one.")));
        }
        else
        {
            changes.Add(new(analogue!.ProductFile, "Delete if unreferenced", $"Remove {analogue!.ProductType} only after obsolete registrations are removed and no external references remain."));
            if(analogue!.CreatorFile is not null) changes.Add(new(analogue!.CreatorFile, "Delete if unreferenced", $"Remove {analogue!.CreatorType} only if no surviving registration still uses it."));
            if(analogue!.BrowserCreatorFile is not null) changes.Add(new(analogue!.BrowserCreatorFile, "Delete if unreferenced", $"Remove {analogue!.BrowserCreatorType} only if no surviving registration still uses it."));
            changes.AddRange(analogue.Registrations.Select(x => new FileChange(x.File, "Remove matching registration", "Remove only registrations/tests whose version semantics match the obsolete requirement.")));
        }

        return new(d, RemediationDisposition.AutoFix, "repository-inference",
            $"Infer remediation from repository analogue {analogue!.ProductType} ({analogue.Family} {analogue.Version}). No platform-specific remediation class is used.",
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
    public static void Apply(
        RepositoryContext context,
        RepositoryModel model,
        CoverageDiscrepancy target,
        ChangeCollector changes)
    {
        var prototype = model.FindObsoletePrototype(target);

        DeleteRequiredConcreteProduct(context, target, changes, prototype.ProductFile);

        if(!string.IsNullOrWhiteSpace(prototype.CreatorFile))
            DeleteRequiredPlatformCreator(context, target, changes, prototype.CreatorFile);
    }

    private static void DeleteRequiredConcreteProduct(
        RepositoryContext context,
        CoverageDiscrepancy target,
        ChangeCollector changes,
        string? relative)
    {
        if(string.IsNullOrWhiteSpace(relative))
            throw new InvalidDataException(
                $"No ConcreteProduct source file was identified for obsolete platform {target.DisplayName}.");

        var normalized = relative.Replace('\\', '/');

        if(!normalized.Contains("/ConcreteProducts/", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException(
                $"Refusing obsolete removal because {relative} is not a ConcreteProducts source file.");

        var path = context.PathFromRoot(relative);
        if(!File.Exists(path))
            throw new FileNotFoundException(
                $"ConcreteProduct source file for obsolete platform {target.DisplayName} was not found.",
                path);

        Console.WriteLine($"    Deleting obsolete ConcreteProduct: {relative}");
        changes.Delete(relative);
    }

    private static void DeleteRequiredPlatformCreator(
        RepositoryContext context,
        CoverageDiscrepancy target,
        ChangeCollector changes,
        string relative)
    {
        var normalized = relative.Replace('\\', '/');

        if(!normalized.Contains("/ConcreteCreators/", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException(
                $"Refusing obsolete removal because {relative} is not a ConcreteCreators source file.");

        var path = context.PathFromRoot(relative);
        if(!File.Exists(path))
            throw new FileNotFoundException(
                $"PlatformCreator source file for obsolete platform {target.DisplayName} was not found.",
                path);

        Console.WriteLine($"    Deleting obsolete PlatformCreator: {relative}");
        changes.Delete(relative);
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
