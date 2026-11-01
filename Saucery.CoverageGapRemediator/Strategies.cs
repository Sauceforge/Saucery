using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator;

internal sealed class ChromiumOsMissingStrategy : IRemediationStrategy
{
    public string Name => "missing-chromiumos";

    public bool CanHandle(CoverageDiscrepancy discrepancy) =>
        discrepancy.Type == DiscrepancyType.Missing &&
        discrepancy.Family.Equals("ChromiumOS", StringComparison.OrdinalIgnoreCase);

    public RemediationItem Plan(RepositoryContext context, CoverageDiscrepancy discrepancy, Options options) =>
        new(
            discrepancy,
            RemediationDisposition.AutoFix,
            Name,
            "Add ChromiumOS as a first-class Selenium desktop platform with Chrome support and one representative test sample.",
            [
                Change("Saucery.Core/Util/SauceryConstants.cs", "Modify", "Add PLATFORM_CHROMIUM_OS."),
                Change("Saucery.Core/Dojo/Platforms/ConcreteProducts/Google/ChromiumOSPlatform.cs", "Create", "Add ChromiumOS platform product."),
                Change("Saucery.Core/Dojo/Platforms/ConcreteCreators/Google/ChromiumOSPlatformCreator.cs", "Create", "Add ChromiumOS platform creator."),
                Change("Saucery.Core/Dojo/Browsers/ConcreteCreators/Google/ChromiumOSBrowserCreator.cs", "Create", "Add Chrome-only ChromiumOS browser creator."),
                Change("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", "Modify", "Dispatch ChromiumOS platform creation."),
                Change("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", "Modify", "Dispatch ChromiumOS browser creation."),
                Change("Saucery.Core/Dojo/DojoExtensions.cs", "Modify", "Resolve ChromiumOS during desktop browser lookup."),
                Change("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", "Modify", "Add one representative ChromiumOS/Chrome sample.")
            ],
            []);

    public Task ApplyAsync(
        RepositoryContext context,
        CoverageDiscrepancy discrepancy,
        Options options,
        RepositoryTransaction transaction,
        ChangeCollector changes)
    {
        changes.Modify("Saucery.Core/Util/SauceryConstants.cs", source =>
            CSharpTextEditor.AddConstantAfter(
                source,
                "public const string PLATFORM_LINUX = \"Linux\";",
                "    public const string PLATFORM_CHROMIUM_OS = \"ChromiumOS\";",
                "PLATFORM_CHROMIUM_OS"));

        changes.Write(
            "Saucery.Core/Dojo/Platforms/ConcreteProducts/Google/ChromiumOSPlatform.cs",
            """
            using Saucery.Core.Dojo.Platforms.Base;
            using Saucery.Core.RestAPI;
            using Saucery.Core.Util;

            namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;

            public sealed class ChromiumOSPlatform(SupportedPlatform sp)
                : PlatformBase(
                    sp,
                    SauceryConstants.PLATFORM_CHROMIUM_OS,
                    [SauceryConstants.BROWSER_CHROME],
                    [SauceryConstants.BROWSER_CHROME],
                    [
                        SauceryConstants.SCREENRES_800_600,
                        SauceryConstants.SCREENRES_1024_768,
                        SauceryConstants.SCREENRES_1152_864,
                        SauceryConstants.SCREENRES_1280_768,
                        SauceryConstants.SCREENRES_1280_800,
                        SauceryConstants.SCREENRES_1280_960,
                        SauceryConstants.SCREENRES_1280_1024,
                        SauceryConstants.SCREENRES_1400_1050,
                        SauceryConstants.SCREENRES_1440_900,
                        SauceryConstants.SCREENRES_1600_1200,
                        SauceryConstants.SCREENRES_1680_1050,
                        SauceryConstants.SCREENRES_1920_1080,
                        SauceryConstants.SCREENRES_1920_1200
                    ])
            {
            }
            """);

        changes.Write(
            "Saucery.Core/Dojo/Platforms/ConcreteCreators/Google/ChromiumOSPlatformCreator.cs",
            """
            using Saucery.Core.Dojo.Platforms.Base;
            using Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;
            using Saucery.Core.RestAPI;

            namespace Saucery.Core.Dojo.Platforms.ConcreteCreators.Google;

            internal class ChromiumOSPlatformCreator(SupportedPlatform sp) : PlatformCreator(sp)
            {
                public override PlatformBase Create() => new ChromiumOSPlatform(Platform);
            }
            """);

        changes.Write(
            "Saucery.Core/Dojo/Browsers/ConcreteCreators/Google/ChromiumOSBrowserCreator.cs",
            """
            using Saucery.Core.Dojo.Browsers.Base;
            using Saucery.Core.Dojo.Browsers.ConcreteProducts.PC;
            using Saucery.Core.RestAPI;
            using Saucery.Core.Util;

            namespace Saucery.Core.Dojo.Browsers.ConcreteCreators.Google;

            internal class ChromiumOSBrowserCreator(SupportedPlatform sp) : BrowserCreator(sp)
            {
                public override BrowserBase? Create(
                    string platformNameForOption,
                    List<string> screenResolutions,
                    bool isArmRequired = false) =>
                    Platform.api_name switch
                    {
                        SauceryConstants.BROWSER_CHROME =>
                            new ChromeBrowser(Platform, screenResolutions, platformNameForOption, isArmRequired),
                        _ => null,
                    };
            }
            """);

        changes.Modify("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", source =>
        {
            var updated = CSharpTextEditor.AddUsingAfter(
                source,
                "using Saucery.Core.Dojo.Platforms.ConcreteCreators.Linux;",
                "using Saucery.Core.Dojo.Platforms.ConcreteCreators.Google;");
            return CSharpTextEditor.AddSwitchArmBeforeDefault(
                updated,
                "CreatePlatform(SupportedPlatform sp)",
                "SauceryConstants.PLATFORM_CHROMIUM_OS => new ChromiumOSPlatformCreator(sp).Create(),",
                "PLATFORM_CHROMIUM_OS =>");
        });

        changes.Modify("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "sp.Os switch",
                "SauceryConstants.PLATFORM_CHROMIUM_OS => new ChromiumOSBrowserCreator(sp).Create(SauceryConstants.PLATFORM_CHROMIUM_OS, screenResolutions, isArmRequired),",
                "PLATFORM_CHROMIUM_OS =>"));

        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "FindDesktopBrowser",
                "SauceryConstants.PLATFORM_CHROMIUM_OS => platforms.GetPlatform<ChromiumOSPlatform>().FirstOrDefault(),",
                "PLATFORM_CHROMIUM_OS =>"));

        changes.Modify("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", source =>
            InsertDesktopTestSample(
                source,
                "new DesktopPlatform(SauceryConstants.PLATFORM_CHROMIUM_OS, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_LATEST)"));

        return Task.CompletedTask;
    }

    private static string InsertDesktopTestSample(string source, string item)
    {
        if (source.Contains("PLATFORM_CHROMIUM_OS", StringComparison.Ordinal)) return source;

        var marker = "new DesktopPlatform(SauceryConstants.PLATFORM_LINUX";
        var index = source.IndexOf(marker, StringComparison.Ordinal);
        if (index < 0)
        {
            throw new InvalidDataException("Could not locate the Linux representative desktop test sample.");
        }

        var lineStart = source.LastIndexOf('\n', index);
        lineStart = lineStart < 0 ? 0 : lineStart + 1;
        var lineEnd = source.IndexOf('\n', index);
        if (lineEnd < 0) lineEnd = source.Length;
        var line = source[lineStart..lineEnd];
        var indent = line[..(line.Length - line.TrimStart().Length)];
        var insertAt = lineEnd < source.Length ? lineEnd + 1 : lineEnd;
        return source.Insert(insertAt, indent + item + "," + Environment.NewLine);
    }

    private static FileChange Change(string path, string type, string description) => new(path, type, description);
}

internal sealed class MobileMissingStrategy : IRemediationStrategy
{
    public string Name => "missing-mobile-platform";

    public bool CanHandle(CoverageDiscrepancy discrepancy) =>
        discrepancy.Type == DiscrepancyType.Missing &&
        discrepancy.Family is "Android" or "iOS" &&
        discrepancy.Kind is PlatformRequirementKind.MobileExactVersion or PlatformRequirementKind.MobileMajorWildcard;

    public RemediationItem Plan(RepositoryContext context, CoverageDiscrepancy discrepancy, Options options)
    {
        var exact = discrepancy.Kind == PlatformRequirementKind.MobileExactVersion;
        var family = discrepancy.Family;
        var summary = exact
            ? $"Add explicit {family} {discrepancy.Version} platform support by cloning the nearest existing {family} platform class, wiring factories/lookups, and adding a sparse test case."
            : $"Add {family} {discrepancy.Version} real-device major-family support using the existing major-version platform class (creating it from the nearest class only when necessary) and add a representative real-device test.";

        return new RemediationItem(
            discrepancy,
            RemediationDisposition.AutoFix,
            Name,
            summary,
            BuildPlannedChanges(discrepancy),
            ["Generated platform classes are cloned from the nearest existing version and validated by Roslyn/build/tests before changes are retained."]);
    }

    public Task ApplyAsync(
        RepositoryContext context,
        CoverageDiscrepancy discrepancy,
        Options options,
        RepositoryTransaction transaction,
        ChangeCollector changes)
    {
        if (discrepancy.Family == "iOS")
        {
            if (discrepancy.Kind == PlatformRequirementKind.MobileExactVersion)
            {
                AddIosExact(context, discrepancy, changes);
            }
            else
            {
                AddIosWildcard(context, discrepancy, changes);
            }
        }
        else
        {
            if (discrepancy.Kind == PlatformRequirementKind.MobileExactVersion)
            {
                AddAndroidExact(context, discrepancy, changes);
            }
            else
            {
                AddAndroidWildcard(context, discrepancy, changes);
            }
        }

        return Task.CompletedTask;
    }

    private static IReadOnlyList<FileChange> BuildPlannedChanges(CoverageDiscrepancy d)
    {
        var files = new List<FileChange>();
        var factory = d.Family == "iOS" ? "ApplePlatformFactory.cs" : "AndroidPlatformFactory.cs";
        files.Add(new($"Saucery.Core/Dojo/Platforms/{factory}", "Modify", "Wire platform factory mapping."));
        files.Add(new("Saucery.Core/Dojo/DojoExtensions.cs", "Modify", "Wire platform/browser lookup mapping."));

        if (d.Kind == PlatformRequirementKind.MobileExactVersion)
        {
            files.Add(new("Saucery.Core/Dojo/Platforms/ConcreteCreators", "Create if required", "Create version-specific platform creator."));
            files.Add(new("Saucery.Core/Dojo/Platforms/ConcreteProducts", "Create if required", "Create version-specific platform product."));
            files.Add(new(
                d.Family == "iOS" ? "Saucery.Core.Tests/IOSFactoryVersionTests.cs" : "Saucery.Core.Tests/AndroidFactoryVersionTests.cs",
                "Modify",
                "Add one supported emulator/simulator test case."));
        }
        else
        {
            files.Add(new(
                d.Family == "iOS" ? "Saucery.Core.Tests/RealIOSFactoryVersionTests.cs" : "Saucery.Core.Tests/DataProviders/PlatformDataClass.cs",
                "Modify",
                "Add one representative real-device major-version sample."));
        }

        return files;
    }

    private static void AddIosExact(RepositoryContext context, CoverageDiscrepancy d, ChangeCollector changes)
    {
        var version = PlatformNaming.ExactVersion(d.Version);
        var digits = PlatformNaming.ClassDigits(version);
        EnsureMobilePlatformClasses(context, changes, "IOS", digits, "Apple", version);

        changes.Modify("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "CreatePlatform(SupportedPlatform sp)",
                $"\"{version}\" => new IOS{digits}PlatformCreator(sp).Create(),",
                $"\"{version}\" =>"));

        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "FindIOSBrowser",
                $"\"iOS {version}\" => platforms.GetPlatform<IOS{digits}Platform>().FirstOrDefault(),",
                $"\"iOS {version}\" =>"));

        changes.Modify("Saucery.Core.Tests/IOSFactoryVersionTests.cs", source =>
            AddQuotedVersionToStringArray(source, "IOSPlatformVersions", version));
    }

    private static void AddIosWildcard(RepositoryContext context, CoverageDiscrepancy d, ChangeCollector changes)
    {
        var major = PlatformNaming.WildcardMajor(d.Version);
        var digits = major;
        EnsureMobilePlatformClasses(context, changes, "IOS", digits, "Apple", $"{major}.0");

        changes.Modify("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "CreateRealPlatform(SupportedPlatform sp)",
                $"\"{major}\" => new IOS{digits}PlatformCreator(sp).Create(),",
                $"\"{major}\" => new IOS{digits}PlatformCreator"));

        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "FindIOSPlatform",
                $"\"iOS {major}\" => platforms.GetPlatform<IOS{digits}Platform>().FirstOrDefault(),",
                $"\"iOS {major}\" =>"));

        changes.Modify("Saucery.Core.Tests/RealIOSFactoryVersionTests.cs", source =>
            AddQuotedVersionToInlineNewArray(source, "SupportedTestCases", major));
    }

    private static void AddAndroidExact(RepositoryContext context, CoverageDiscrepancy d, ChangeCollector changes)
    {
        var version = PlatformNaming.ExactVersion(d.Version);
        var digits = PlatformNaming.ClassDigits(version);
        EnsureMobilePlatformClasses(context, changes, "Android", digits, "Google", version);

        changes.Modify("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "CreatePlatform(SupportedPlatform sp)",
                $"\"{version}\" => new Android{digits}PlatformCreator(sp).Create(),",
                $"\"{version}\" =>"));

        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "FindAndroidBrowser",
                $"\"Linux {version}\" => platforms.GetPlatform<Android{digits}Platform>().FirstOrDefault(),",
                $"\"Linux {version}\" =>"));

        changes.Modify("Saucery.Core.Tests/AndroidFactoryVersionTests.cs", source =>
        {
            if (source.Contains($"\"{version}\"", StringComparison.Ordinal)) return source;
            var sample = d.Devices.FirstOrDefault(x => x.Contains("Emulator", StringComparison.OrdinalIgnoreCase))
                         ?? "Google Pixel Emulator";
            var item = $"new AndroidPlatform(\"{EscapeString(sample)}\", \"{version}\", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)";
            return CSharpTextEditor.AddArrayItemBeforeClosing(
                source,
                "SupportedTestCases",
                item,
                $"\"{version}\"");
        });
    }

    private static void AddAndroidWildcard(RepositoryContext context, CoverageDiscrepancy d, ChangeCollector changes)
    {
        var major = PlatformNaming.WildcardMajor(d.Version);
        EnsureMobilePlatformClasses(context, changes, "Android", major, "Google", $"{major}.0");

        changes.Modify("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "CreateRealPlatform(SupportedPlatform sp)",
                $"\"{major}\" => new Android{major}PlatformCreator(sp).Create(),",
                $"\"{major}\" => new Android{major}PlatformCreator"));

        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "FindAndroidPlatform",
                $"\"Linux {major}\" => platforms.GetPlatform<Android{major}Platform>().FirstOrDefault(),",
                $"\"Linux {major}\" =>"));

        changes.Modify("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", source =>
        {
            if (Regex.IsMatch(source, $"new\\s+AndroidRealDevice\\([^\\n]+,\\s*\"{Regex.Escape(major)}\"\\)"))
            {
                return source;
            }

            var sample = d.Devices.FirstOrDefault(x => !x.Contains("Emulator", StringComparison.OrdinalIgnoreCase))
                         ?? "Google.*";
            var item = $"new AndroidRealDevice(\"{EscapeString(sample)}\", \"{major}\")";
            return CSharpTextEditor.AddArrayItemBeforeClosing(
                source,
                "RealAndroidDevices",
                item,
                $"\"{major}\"");
        });
    }

    private static void EnsureMobilePlatformClasses(
        RepositoryContext context,
        ChangeCollector changes,
        string prefix,
        string digits,
        string namespaceFolder,
        string targetVersion)
    {
        var creatorRelative = $"Saucery.Core/Dojo/Platforms/ConcreteCreators/{namespaceFolder}/{prefix}{digits}PlatformCreator.cs";
        var productRelative = $"Saucery.Core/Dojo/Platforms/ConcreteProducts/{namespaceFolder}/{prefix}{digits}Platform.cs";

        if (!File.Exists(context.PathFromRoot(productRelative)))
        {
            var source = FindNearestVersionedFile(
                context.PathFromRoot($"Saucery.Core/Dojo/Platforms/ConcreteProducts/{namespaceFolder}"),
                prefix,
                "Platform.cs",
                targetVersion);
            var oldDigits = ExtractDigits(Path.GetFileName(source), prefix, "Platform.cs");
            var cloned = CSharpTextEditor.CloneAndRename(
                File.ReadAllText(source),
                ($"{prefix}{oldDigits}Platform", $"{prefix}{digits}Platform"));
            changes.Write(productRelative, cloned);
        }

        if (!File.Exists(context.PathFromRoot(creatorRelative)))
        {
            var source = FindNearestVersionedFile(
                context.PathFromRoot($"Saucery.Core/Dojo/Platforms/ConcreteCreators/{namespaceFolder}"),
                prefix,
                "PlatformCreator.cs",
                targetVersion);
            var oldDigits = ExtractDigits(Path.GetFileName(source), prefix, "PlatformCreator.cs");
            var cloned = CSharpTextEditor.CloneAndRename(
                File.ReadAllText(source),
                ($"{prefix}{oldDigits}PlatformCreator", $"{prefix}{digits}PlatformCreator"),
                ($"{prefix}{oldDigits}Platform", $"{prefix}{digits}Platform"));
            changes.Write(creatorRelative, cloned);
        }
    }

    private static string FindNearestVersionedFile(string directory, string prefix, string suffix, string targetVersion)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException(directory);
        }

        var candidates = Directory.EnumerateFiles(directory, $"{prefix}*{suffix}")
            .Select(path => new
            {
                Path = path,
                Digits = ExtractDigits(Path.GetFileName(path), prefix, suffix)
            })
            .Where(x => int.TryParse(x.Digits, out _))
            .OrderBy(x => Math.Abs(MobileVersionKey(prefix, x.Digits) - VersionKey(targetVersion)))
            .ThenByDescending(x => MobileVersionKey(prefix, x.Digits))
            .ToArray();

        return candidates.FirstOrDefault()?.Path
               ?? throw new InvalidDataException($"Could not find a {prefix}*{suffix} template under {directory}.");
    }

    private static int MobileVersionKey(string prefix, string digits)
    {
        if (!int.TryParse(digits, out var numeric)) return 0;

        if (prefix == "IOS" && digits.Length == 3)
        {
            return int.Parse(digits[..2]) * 100 + int.Parse(digits[2..]);
        }

        if (prefix == "Android" && (digits is "51" or "71" or "81"))
        {
            // Existing single-digit Android minor classes use names such as Android51/71/81.
            return int.Parse(digits[..1]) * 100 + int.Parse(digits[1..]);
        }

        return numeric * 100;
    }

    private static int VersionKey(string version)
    {
        var exact = PlatformNaming.ExactVersion(version);
        var parts = exact.Split('.');
        return int.Parse(parts[0]) * 100 + int.Parse(parts[1]);
    }

    private static string ExtractDigits(string fileName, string prefix, string suffix)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var suffixWithoutExtension = Path.GetFileNameWithoutExtension(suffix);
        var match = Regex.Match(name, $@"^{Regex.Escape(prefix)}(?<digits>\d+){Regex.Escape(suffixWithoutExtension)}$");
        return match.Success ? match.Groups["digits"].Value : string.Empty;
    }

    private static string AddQuotedVersionToStringArray(string source, string marker, string version)
    {
        if (Regex.IsMatch(source, $"\"{Regex.Escape(version)}\"")) return source;
        return CSharpTextEditor.AddArrayItemBeforeClosing(source, marker, $"\"{version}\"", $"\"{version}\"");
    }

    private static string AddQuotedVersionToInlineNewArray(string source, string marker, string version)
    {
        if (Regex.IsMatch(source, $"\"{Regex.Escape(version)}\"")) return source;

        var markerIndex = source.IndexOf(marker, StringComparison.Ordinal);
        var newArrayIndex = source.IndexOf("new[]", markerIndex, StringComparison.Ordinal);
        if (markerIndex < 0 || newArrayIndex < 0)
        {
            throw new InvalidDataException($"Could not locate inline array for {marker}.");
        }

        var open = source.IndexOf('{', newArrayIndex);
        var close = source.IndexOf('}', open);
        if (open < 0 || close < 0) throw new InvalidDataException($"Could not locate inline array body for {marker}.");
        var body = source[(open + 1)..close].Trim();
        var newBody = string.IsNullOrWhiteSpace(body) ? $" \"{version}\" " : body + $", \"{version}\" ";
        return source[..(open + 1)] + newBody + source[close..];
    }

    private static string EscapeString(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
}

internal sealed class DesktopVersionMissingStrategy : IRemediationStrategy
{
    public string Name => "missing-desktop-version";

    public bool CanHandle(CoverageDiscrepancy discrepancy) =>
        discrepancy.Type == DiscrepancyType.Missing &&
        discrepancy.Kind == PlatformRequirementKind.DesktopPlatform &&
        discrepancy.Family is "macOS" or "Windows";

    public RemediationItem Plan(RepositoryContext context, CoverageDiscrepancy d, Options options)
    {
        var disposition = string.IsNullOrWhiteSpace(d.Version)
            ? RemediationDisposition.ManualReview
            : RemediationDisposition.AutoFix;

        return new RemediationItem(
            d,
            disposition,
            Name,
            disposition == RemediationDisposition.AutoFix
                ? $"Clone the nearest {d.Family} desktop implementation for {d.DisplayName}, wire all desktop dispatch points, and add one representative browser test."
                : $"{d.DisplayName} has no parseable desktop version, so a deterministic version-clone remediation cannot be generated.",
            disposition == RemediationDisposition.AutoFix
                ? [
                    new("Saucery.Core/Util/SauceryConstants.cs", "Modify", "Add platform constant."),
                    new("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", "Modify", "Wire platform creator."),
                    new("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", "Modify", "Wire browser creator."),
                    new("Saucery.Core/Dojo/DojoExtensions.cs", "Modify", "Wire desktop lookup."),
                    new("Saucery.Core/Dojo/Platforms/ConcreteProducts/PC", "Create", "Clone version-specific platform product."),
                    new("Saucery.Core/Dojo/Platforms/ConcreteCreators/PC", "Create", "Clone version-specific platform creator."),
                    new("Saucery.Core/Dojo/Browsers/ConcreteCreators", "Create", "Clone version-specific browser creator."),
                    new("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", "Modify", "Add one representative browser sample.")
                ]
                : [],
            disposition == RemediationDisposition.AutoFix
                ? ["The cloned product preserves the nearest platform's browser/resolution model; build/tests/reporter verification must pass before the transaction is committed."]
                : ["Add an explicit remediation strategy for this unversioned desktop platform."]);
    }

    public Task ApplyAsync(
        RepositoryContext context,
        CoverageDiscrepancy d,
        Options options,
        RepositoryTransaction transaction,
        ChangeCollector changes)
    {
        if (string.IsNullOrWhiteSpace(d.Version)) return Task.CompletedTask;
        if (d.Family == "macOS") AddMac(context, d, changes);
        else AddWindows(context, d, changes);
        return Task.CompletedTask;
    }

    private static void AddMac(RepositoryContext context, CoverageDiscrepancy d, ChangeCollector changes)
    {
        var major = PlatformNaming.Major(d.Version).ToString();
        var constant = $"PLATFORM_MAC_{major}";
        var constantValue = $"Mac {major}";

        changes.Modify("Saucery.Core/Util/SauceryConstants.cs", source =>
            CSharpTextEditor.AddConstantAfter(
                source,
                "public const string PLATFORM_MAC_26 = \"Mac 26\";",
                $"    public const string {constant} = \"{constantValue}\";",
                constant));

        CloneDesktopType(context, changes, "Mac", major, "Platform", "Saucery.Core/Dojo/Platforms/ConcreteProducts/PC");
        CloneDesktopType(context, changes, "Mac", major, "PlatformCreator", "Saucery.Core/Dojo/Platforms/ConcreteCreators/PC");
        CloneBrowserCreator(context, changes, "Mac", major);

        // Ensure the cloned platform's visible platform option reflects the new major.
        var product = $"Saucery.Core/Dojo/Platforms/ConcreteProducts/PC/Mac{major}Platform.cs";
        changes.Modify(product, source => Regex.Replace(source, @"macOS\s+\d+(?:\.\d+)?", $"macOS {major}"));

        changes.Modify("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "CreatePlatform(SupportedPlatform sp)",
                $"SauceryConstants.{constant} => new Mac{major}PlatformCreator(sp).Create(),",
                $"{constant} =>"));

        changes.Modify("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "sp.Os switch",
                $"SauceryConstants.{constant} => new Mac{major}BrowserCreator(sp).Create(\"macOS {major}\", screenResolutions, isArmRequired),",
                $"{constant} =>"));

        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "FindDesktopBrowser",
                $"SauceryConstants.{constant} => platforms.GetPlatform<Mac{major}Platform>().FirstOrDefault(),",
                $"{constant} =>"));

        changes.Modify("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", source =>
            AddDesktopSample(source, constant, "SauceryConstants.BROWSER_CHROME"));
    }

    private static void AddWindows(RepositoryContext context, CoverageDiscrepancy d, ChangeCollector changes)
    {
        var version = d.Version;
        var digits = version.Replace(".", string.Empty, StringComparison.Ordinal);
        var constantSuffix = version.Replace('.', '_');
        var constant = $"PLATFORM_WINDOWS_{constantSuffix}";

        changes.Modify("Saucery.Core/Util/SauceryConstants.cs", source =>
            CSharpTextEditor.AddConstantAfter(
                source,
                "public const string PLATFORM_WINDOWS_11 = \"Windows 11\";",
                $"    public const string {constant} = \"Windows {version}\";",
                constant));

        CloneDesktopType(context, changes, "Windows", digits, "Platform", "Saucery.Core/Dojo/Platforms/ConcreteProducts/PC");
        CloneDesktopType(context, changes, "Windows", digits, "PlatformCreator", "Saucery.Core/Dojo/Platforms/ConcreteCreators/PC");
        CloneBrowserCreator(context, changes, "Windows", digits);

        var product = $"Saucery.Core/Dojo/Platforms/ConcreteProducts/PC/Windows{digits}Platform.cs";
        changes.Modify(product, source =>
        {
            var updated = Regex.Replace(
                source,
                @"SauceryConstants\.PLATFORM_WINDOWS_(?:11|10|81|8)",
                $"SauceryConstants.{constant}");
            return Regex.Replace(updated, @"Windows\s+\d+(?:\.\d+)?", $"Windows {version}");
        });

        changes.Modify("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "CreatePlatform(SupportedPlatform sp)",
                $"SauceryConstants.{constant} => new Windows{digits}PlatformCreator(sp).Create(),",
                $"{constant} =>"));

        changes.Modify("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "sp.Os switch",
                $"SauceryConstants.{constant} => new Windows{digits}BrowserCreator(sp).Create(\"Windows {version}\", screenResolutions, isArmRequired),",
                $"{constant} =>"));

        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.AddSwitchArmBeforeDefault(
                source,
                "FindDesktopBrowser",
                $"SauceryConstants.{constant} => platforms.GetPlatform<Windows{digits}Platform>().FirstOrDefault(),",
                $"{constant} =>"));

        changes.Modify("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", source =>
            AddDesktopSample(source, constant, "SauceryConstants.BROWSER_CHROME"));
    }

    private static void CloneDesktopType(
        RepositoryContext context,
        ChangeCollector changes,
        string prefix,
        string newDigits,
        string suffix,
        string relativeDirectory)
    {
        var destination = $"{relativeDirectory}/{prefix}{newDigits}{suffix}.cs";
        if (File.Exists(context.PathFromRoot(destination))) return;

        var directory = context.PathFromRoot(relativeDirectory);
        var pattern = $"{prefix}*{suffix}.cs";
        var templates = Directory.EnumerateFiles(directory, pattern)
            .Select(path => new
            {
                Path = path,
                Match = Regex.Match(Path.GetFileName(path), $@"^{prefix}(?<digits>\d+){suffix}\.cs$")
            })
            .Where(x => x.Match.Success)
            .OrderByDescending(x => DesktopVersionRank(prefix, x.Match.Groups["digits"].Value))
            .ToArray();

        var template = templates.FirstOrDefault()
                       ?? throw new InvalidDataException($"No template matching {pattern} was found in {directory}.");
        var oldDigits = template.Match.Groups["digits"].Value;
        var cloned = CSharpTextEditor.CloneAndRename(
            File.ReadAllText(template.Path),
            ($"{prefix}{oldDigits}{suffix}", $"{prefix}{newDigits}{suffix}"),
            ($"{prefix}{oldDigits}Platform", $"{prefix}{newDigits}Platform"));
        changes.Write(destination, cloned);
    }

    private static void CloneBrowserCreator(RepositoryContext context, ChangeCollector changes, string prefix, string newDigits)
    {
        var root = context.PathFromRoot("Saucery.Core/Dojo/Browsers/ConcreteCreators");
        var templates = Directory.EnumerateFiles(root, $"{prefix}*BrowserCreator.cs", SearchOption.AllDirectories)
            .Select(path => new
            {
                Path = path,
                Match = Regex.Match(Path.GetFileName(path), $@"^{prefix}(?<digits>\d+)BrowserCreator\.cs$")
            })
            .Where(x => x.Match.Success)
            .OrderByDescending(x => DesktopVersionRank(prefix, x.Match.Groups["digits"].Value))
            .ToArray();

        var template = templates.FirstOrDefault()
                       ?? throw new InvalidDataException($"No {prefix} browser creator template was found.");
        var oldDigits = template.Match.Groups["digits"].Value;
        var destinationPath = Path.Combine(Path.GetDirectoryName(template.Path)!, $"{prefix}{newDigits}BrowserCreator.cs");
        var destination = context.Relative(destinationPath).Replace('\\', '/');
        if (File.Exists(destinationPath)) return;

        var cloned = CSharpTextEditor.CloneAndRename(
            File.ReadAllText(template.Path),
            ($"{prefix}{oldDigits}BrowserCreator", $"{prefix}{newDigits}BrowserCreator"));
        changes.Write(destination, cloned);
    }


    private static double DesktopVersionRank(string prefix, string digits)
    {
        if (prefix == "Windows" && digits == "81") return 8.1;
        return double.TryParse(digits, out var value) ? value : 0;
    }

    private static string AddDesktopSample(string source, string constant, string browser)
    {
        if (source.Contains(constant, StringComparison.Ordinal)) return source;
        var item = $"new DesktopPlatform(SauceryConstants.{constant}, {browser}, SauceryConstants.BROWSER_VERSION_LATEST)";
        return CSharpTextEditor.AddArrayItemBeforeClosing(source, "DesktopPlatforms", item, constant);
    }
}

internal sealed class ObsoletePlatformStrategy : IRemediationStrategy
{
    public string Name => "obsolete-platform";

    public bool CanHandle(CoverageDiscrepancy discrepancy) =>
        discrepancy.Type == DiscrepancyType.ObsoleteCandidate &&
        discrepancy.Family is "Android" or "iOS" or "macOS" or "Windows" or "Linux" or "ChromiumOS";

    public RemediationItem Plan(RepositoryContext context, CoverageDiscrepancy d, Options options)
    {
        if (d.Family == "Windows" &&
            (d.DisplayName.Equals("Windows 8", StringComparison.OrdinalIgnoreCase) ||
             d.DisplayName.Equals("Windows 8.1", StringComparison.OrdinalIgnoreCase)))
        {
            return new RemediationItem(
                d,
                RemediationDisposition.ManualReview,
                Name,
                $"'{d.DisplayName}' is a known case where the API-derived reporter can disagree with the Sauce Platform Configurator. Automatic removal is blocked even when --allow-obsolete-removal is supplied.",
                [],
                ["Confirm support against the current Platform Configurator and update the reporter's authoritative-source model instead of deleting Saucery support."]);
        }

        if (!options.AllowObsoleteRemoval)
        {
            return new RemediationItem(
                d,
                RemediationDisposition.ManualReview,
                Name,
                $"Removal is intentionally blocked because '{d.DisplayName}' is only an obsolete candidate. Re-run with --allow-obsolete-removal after independently confirming the Platform Configurator no longer supports it.",
                [],
                ["Sauce's API-derived catalogue is not sufficient by itself to prove a platform is obsolete (Windows 8/8.1 demonstrated this)."]);
        }

        return new RemediationItem(
            d,
            RemediationDisposition.AutoFix,
            Name,
            $"Remove explicit {d.DisplayName} mappings and representative test data. Version-specific classes are retained unless they become provably unreferenced; dead-code deletion can be reviewed in the resulting diff.",
            PlannedRemovalChanges(d),
            ["Destructive remediation was explicitly enabled with --allow-obsolete-removal."]);
    }

    public Task ApplyAsync(
        RepositoryContext context,
        CoverageDiscrepancy d,
        Options options,
        RepositoryTransaction transaction,
        ChangeCollector changes)
    {
        if (!options.AllowObsoleteRemoval) return Task.CompletedTask;

        if (d.Family == "iOS") RemoveIos(d, changes);
        else if (d.Family == "Android") RemoveAndroid(d, changes);
        else if (d.Family is "macOS" or "Windows" or "Linux" or "ChromiumOS") RemoveDesktop(d, changes);

        return Task.CompletedTask;
    }

    private static IReadOnlyList<FileChange> PlannedRemovalChanges(CoverageDiscrepancy d)
    {
        var changes = new List<FileChange>
        {
            new("Saucery.Core/Dojo/DojoExtensions.cs", "Modify", "Remove obsolete lookup mapping.")
        };

        if (d.Family == "iOS")
            changes.Add(new("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs", "Modify", "Remove obsolete factory mapping."));
        else if (d.Family == "Android")
            changes.Add(new("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs", "Modify", "Remove obsolete factory mapping."));
        else
        {
            changes.Add(new("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", "Modify", "Remove obsolete desktop platform mapping."));
            changes.Add(new("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", "Modify", "Remove obsolete desktop browser mapping."));
        }

        changes.Add(new("Saucery.Core.Tests", "Modify", "Remove representative test data for the obsolete target."));
        return changes;
    }

    private static void RemoveIos(CoverageDiscrepancy d, ChangeCollector changes)
    {
        if (d.Kind == PlatformRequirementKind.MobileExactVersion)
        {
            var version = PlatformNaming.ExactVersion(d.Version);
            changes.Modify("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs", source =>
                CSharpTextEditor.RemoveLinesContaining(source, $"\"{version}\" =>"));
            changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
                CSharpTextEditor.RemoveLinesContaining(source, $"\"iOS {version}\" =>"));
            changes.Modify("Saucery.Core.Tests/IOSFactoryVersionTests.cs", source =>
                RemoveQuotedArrayValue(source, "IOSPlatformVersions", version));
        }
        else
        {
            var major = PlatformNaming.WildcardMajor(d.Version);
            changes.Modify("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs", source =>
                RemoveLineInMethod(source, "CreateRealPlatform", $"\"{major}\" =>"));
            changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
                RemoveLineInMethod(source, "FindIOSPlatform", $"\"iOS {major}\" =>"));
            changes.Modify("Saucery.Core.Tests/RealIOSFactoryVersionTests.cs", source =>
                RemoveQuotedValueFromInlineNewArray(source, "SupportedTestCases", major));
            changes.Modify("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", source =>
                CSharpTextEditor.RemoveArrayLineContaining(source, "RealIOSDevices", $"\"{major}\"") );
        }
    }

    private static void RemoveAndroid(CoverageDiscrepancy d, ChangeCollector changes)
    {
        if (d.Kind == PlatformRequirementKind.MobileExactVersion)
        {
            var version = PlatformNaming.ExactVersion(d.Version);
            changes.Modify("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs", source =>
                CSharpTextEditor.RemoveLinesContaining(source, $"\"{version}\" =>"));
            changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
                CSharpTextEditor.RemoveLinesContaining(source, $"\"Linux {version}\" =>"));
            changes.Modify("Saucery.Core.Tests/AndroidFactoryVersionTests.cs", source =>
                CSharpTextEditor.RemoveArrayLineContaining(source, "SupportedTestCases", $"\"{version}\""));
        }
        else
        {
            var major = PlatformNaming.WildcardMajor(d.Version);
            changes.Modify("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs", source =>
                RemoveLineInMethod(source, "CreateRealPlatform", $"\"{major}\" =>"));
            changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
                RemoveLineInMethod(source, "FindAndroidPlatform", $"\"Linux {major}\" =>"));
            changes.Modify("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", source =>
                CSharpTextEditor.RemoveArrayLineContaining(source, "RealAndroidDevices", $"\"{major}\""));
        }
    }

    private static void RemoveDesktop(CoverageDiscrepancy d, ChangeCollector changes)
    {
        string? constant = d.Family switch
        {
            "ChromiumOS" => "PLATFORM_CHROMIUM_OS",
            "Linux" => "PLATFORM_LINUX",
            "macOS" when !string.IsNullOrWhiteSpace(d.Version) => $"PLATFORM_MAC_{PlatformNaming.Major(d.Version)}",
            "Windows" when !string.IsNullOrWhiteSpace(d.Version) => $"PLATFORM_WINDOWS_{d.Version.Replace('.', '_')}",
            _ => null
        };

        if (constant is null) return;

        changes.Modify("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", source =>
            CSharpTextEditor.RemoveLinesContaining(source, constant, "=>"));
        changes.Modify("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", source =>
            CSharpTextEditor.RemoveLinesContaining(source, constant, "=>"));
        changes.Modify("Saucery.Core/Dojo/DojoExtensions.cs", source =>
            CSharpTextEditor.RemoveLinesContaining(source, constant, "=>"));
        changes.Modify("Saucery.Core.Tests/DataProviders/PlatformDataClass.cs", source =>
            CSharpTextEditor.RemoveArrayLineContaining(source, "DesktopPlatforms", constant));

        // Constant removal is safe only for a uniquely named constant line.
        changes.Modify("Saucery.Core/Util/SauceryConstants.cs", source =>
            CSharpTextEditor.RemoveLinesContaining(source, $"const string {constant} "));
    }

    private static string RemoveLineInMethod(string source, string methodMarker, string needle)
    {
        var method = source.IndexOf(methodMarker, StringComparison.Ordinal);
        if (method < 0) return source;
        var nextMethod = source.IndexOf("public static", method + methodMarker.Length, StringComparison.Ordinal);
        if (nextMethod < 0) nextMethod = source.Length;

        var prefix = source[..method];
        var body = source[method..nextMethod];
        var suffix = source[nextMethod..];
        return prefix + CSharpTextEditor.RemoveLinesContaining(body, needle) + suffix;
    }

    private static string RemoveQuotedArrayValue(string source, string marker, string value)
    {
        var markerIndex = source.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0) return source;
        var open = source.IndexOf('[', markerIndex);
        var close = source.IndexOf(']', open);
        if (open < 0 || close < 0) return source;
        var body = source[(open + 1)..close];
        var pattern = $"\\s*,?\\s*\"{Regex.Escape(value)}\"\\s*,?";
        var regex = new Regex(pattern, RegexOptions.CultureInvariant);
        var updatedBody = regex.Replace(body, match => match.Value.Contains(',') ? "," : string.Empty, 1);
        updatedBody = Regex.Replace(updatedBody, @",\s*,", ",");
        return source[..(open + 1)] + updatedBody + source[close..];
    }

    private static string RemoveQuotedValueFromInlineNewArray(string source, string marker, string value)
    {
        var markerIndex = source.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0) return source;
        var arrayIndex = source.IndexOf("new[]", markerIndex, StringComparison.Ordinal);
        if (arrayIndex < 0) return source;
        var open = source.IndexOf('{', arrayIndex);
        var close = source.IndexOf('}', open);
        if (open < 0 || close < 0) return source;
        var body = source[(open + 1)..close];
        var values = Regex.Matches(body, "\"(?<v>[^\"]+)\"")
            .Select(x => x.Groups["v"].Value)
            .Where(x => !x.Equals(value, StringComparison.Ordinal))
            .ToArray();
        var replacement = " " + string.Join(", ", values.Select(x => $"\"{x}\"")) + " ";
        return source[..(open + 1)] + replacement + source[close..];
    }
}

internal sealed class ManualReviewStrategy : IRemediationStrategy
{
    public string Name => "manual-review";
    public bool CanHandle(CoverageDiscrepancy discrepancy) => true;

    public RemediationItem Plan(RepositoryContext context, CoverageDiscrepancy d, Options options) =>
        new(
            d,
            RemediationDisposition.ManualReview,
            Name,
            $"No deterministic remediation strategy is registered for {d.Type} {d.Family} {d.Kind} '{d.DisplayName}'. The tool refuses to invent architecture for an unknown platform shape.",
            [],
            ["Implement a dedicated IRemediationStrategy after establishing the required Saucery.Core semantics."]);

    public Task ApplyAsync(
        RepositoryContext context,
        CoverageDiscrepancy discrepancy,
        Options options,
        RepositoryTransaction transaction,
        ChangeCollector changes) => Task.CompletedTask;
}
