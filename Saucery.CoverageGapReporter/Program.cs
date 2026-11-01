using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Saucery.CoverageGapReporter;

internal static class Program
{
    internal const string ReporterBuild = "lifecycle-readme-v5";
    private const string CoreProjectDirectory = "Saucery.Core";
    private const string TestsProjectDirectory = "Saucery.Core.Tests";

    public static async Task<int> Main(string[] args)
    {
        try
        {
            var options = Options.Parse(args);
            var repoRoot = ResolveRepositoryRoot(options.RepositoryRoot);
            var coreRoot = Path.Combine(repoRoot, CoreProjectDirectory);
            var testsRoot = Path.Combine(repoRoot, TestsProjectDirectory);

            Console.WriteLine($"Reporter build: {ReporterBuild}");
            Console.WriteLine($"Repository: {repoRoot}");
            Console.WriteLine("Indexing Saucery.Core...");
            var coreCorpus = SourceCorpus.Load(coreRoot);

            Console.WriteLine("Indexing Saucery.Core.Tests...");
            var testCorpus = SourceCorpus.Load(testsRoot);

            var dependencyAudit = DependencyAudit.Run(repoRoot);
            var credentials = SauceCredentials.From(options);

            using var http = CreateHttpClient(credentials);

            Console.WriteLine("Reading Sauce Labs virtual platform catalogue...");
            var virtualRows = await SauceLabsClient.GetVirtualPlatformsAsync(http, options.Region);

            var realDevices = new List<SauceRealDevice>();
            string? realDeviceWarning = null;

            if (options.IncludeRealDevices)
            {
                try
                {
                    Console.WriteLine("Reading Sauce Labs real-device catalogue...");
                    realDevices = await SauceLabsClient.GetRealDevicesAsync(http, options.Region);
                }
                catch (Exception ex)
                {
                    realDeviceWarning = $"Real-device inventory could not be read: {ex.Message}";
                }
            }

            Console.WriteLine("Collapsing Sauce rows into functional platform requirements...");
            var requirements = FunctionalPlatformBuilder.Build(virtualRows, realDevices);

            Console.WriteLine("Extracting explicitly declared Saucery.Core platforms...");
            var declaredPlatforms = SauceryPlatformExtractor.Extract(coreRoot);

            Console.WriteLine($"Comparing {requirements.Count:N0} functional platform requirements...");
            var findings = requirements
                .Select(r => CoverageAnalyzer.Analyze(r, coreCorpus, testCorpus))
                .OrderBy(x => x.Requirement.Family)
                .ThenBy(x => x.Requirement.SortKey, NaturalStringComparer.Instance)
                .ToList();

            var lifecycle = PlatformLifecycleAnalyzer.Analyze(requirements, declaredPlatforms);

            var report = ReportWriter.Write(
                repoRoot,
                options,
                dependencyAudit,
                virtualRows.Count,
                realDevices.Count,
                findings,
                lifecycle,
                realDeviceWarning);

            var outputPath = Path.GetFullPath(
                Path.IsPathRooted(options.OutputPath)
                    ? options.OutputPath
                    : Path.Combine(repoRoot, options.OutputPath));

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await File.WriteAllTextAsync(outputPath, report, new UTF8Encoding(false));

            var readmePath = Path.Combine(repoRoot, "README.md");
            var readmeChanged = ReadmeCoverageUpdater.Update(readmePath, findings, lifecycle);

            var coreGaps = findings.Count(x => x.CoreStatus == CoverageStatus.Gap);
            var coreReview = findings.Count(x => x.CoreStatus == CoverageStatus.Review);
            var missingFamilySamples = findings
                .GroupBy(x => x.Requirement.FamilyKey)
                .Count(g => g.All(x => x.TestSampleStatus == TestSampleStatus.None));

            Console.WriteLine();
            Console.WriteLine($"Report written to: {outputPath}");
            Console.WriteLine($"README coverage table: {(readmeChanged ? "updated" : "unchanged")} ({readmePath})");
            Console.WriteLine($"Sauce virtual rows read: {virtualRows.Count:N0}");
            Console.WriteLine($"Sauce real devices read: {realDevices.Count:N0}");
            Console.WriteLine($"Functional platform requirements: {requirements.Count:N0}");
            Console.WriteLine($"Core functional gaps: {coreGaps:N0}");
            Console.WriteLine($"Core review items: {coreReview:N0}");
            Console.WriteLine($"Saucery-only obsolete candidates: {lifecycle.ObsoleteCandidates.Count:N0}");
            Console.WriteLine($"Platform families with no representative test sample: {missingFamilySamples:N0}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static string ResolveRepositoryRoot(string? configuredRoot)
    {
        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            var explicitRoot = Path.GetFullPath(configuredRoot);
            if (ContainsRequiredProjects(explicitRoot))
            {
                return explicitRoot;
            }

            throw new DirectoryNotFoundException(
                $"The supplied repository root does not contain both '{CoreProjectDirectory}' " +
                $"and '{TestsProjectDirectory}': {explicitRoot}");
        }

        var starts = new[]
        {
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory
        }
        .Select(Path.GetFullPath)
        .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var start in starts)
        {
            var current = new DirectoryInfo(start);
            while (current is not null)
            {
                if (ContainsRequiredProjects(current.FullName))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not locate the Saucery repository root from '{Directory.GetCurrentDirectory()}' " +
            $"or '{AppContext.BaseDirectory}'. Use --repo-root <path>.");
    }

    private static bool ContainsRequiredProjects(string directory) =>
        Directory.Exists(Path.Combine(directory, CoreProjectDirectory)) &&
        Directory.Exists(Path.Combine(directory, TestsProjectDirectory));

    private static HttpClient CreateHttpClient(SauceCredentials credentials)
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Saucery-CoverageGapReporter/2.0 (+https://github.com/Sauceforge/Saucery)");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (credentials.IsConfigured)
        {
            var bytes = Encoding.UTF8.GetBytes($"{credentials.Username}:{credentials.AccessKey}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
        }

        return client;
    }
}

internal sealed record Options(
    string? RepositoryRoot,
    string OutputPath,
    string Region,
    bool IncludeRealDevices,
    string? Username,
    string? AccessKey)
{
    public static Options Parse(string[] args)
    {
        string? repoRoot = null;
        var output = "saucery-functional-platform-coverage-gaps.txt";
        var region = "us-west-1";
        var includeReal = true;
        string? username = null;
        string? accessKey = null;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--repo-root":
                    repoRoot = RequireValue(args, ref i, "--repo-root");
                    break;
                case "--output":
                    output = RequireValue(args, ref i, "--output");
                    break;
                case "--region":
                    region = RequireValue(args, ref i, "--region");
                    break;
                case "--include-real":
                    includeReal = bool.Parse(RequireValue(args, ref i, "--include-real"));
                    break;
                case "--username":
                    username = RequireValue(args, ref i, "--username");
                    break;
                case "--access-key":
                    accessKey = RequireValue(args, ref i, "--access-key");
                    break;
                case "--help":
                case "-h":
                    PrintHelp();
                    Environment.Exit(0);
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[i]}");
            }
        }

        return new Options(repoRoot, output, region, includeReal, username, accessKey);
    }

    private static string RequireValue(string[] args, ref int i, string name)
    {
        if (i + 1 >= args.Length)
        {
            throw new ArgumentException($"{name} requires a value.");
        }

        return args[++i];
    }

    private static void PrintHelp()
    {
        Console.WriteLine(
            """
            Saucery.CoverageGapReporter

            Reports FUNCTIONAL PLATFORM coverage, not device inventory coverage.

            Examples of functional requirements:
              ChromiumOS
              Mac Golden Gate (27)
              iOS 27.1
              iOS 27.x
              Android 17.0
              Android 17.x

            Individual phones/tablets are only representative examples and never create
            independent coverage requirements.

            Usage:
              dotnet run -c Release -- [options]

            Options:
              --repo-root <path>       Repository root. Default: auto-detect by walking upward.
              --output <path>          Report path. Default: saucery-functional-platform-coverage-gaps.txt
              --region <region>        Sauce region. Default: us-west-1
              --include-real <bool>    Include real-device OS versions. Default: true
              --username <value>       Sauce username. Prefer environment variables.
              --access-key <value>     Sauce access key. Prefer environment variables.
              --help                   Show this help.
            """);
    }
}

internal sealed record SauceCredentials(string? Username, string? AccessKey)
{
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(AccessKey);

    public static SauceCredentials From(Options options)
    {
        var username = options.Username
            ?? Environment.GetEnvironmentVariable("SAUCE_USERNAME")
            ?? Environment.GetEnvironmentVariable("SAUCE_USER_NAME");

        var accessKey = options.AccessKey
            ?? Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY")
            ?? Environment.GetEnvironmentVariable("SAUCE_API_KEY");

        if (string.IsNullOrWhiteSpace(username) != string.IsNullOrWhiteSpace(accessKey))
        {
            throw new InvalidOperationException(
                "Sauce credentials are incomplete. Supply both username and access key.");
        }

        return new SauceCredentials(username, accessKey);
    }
}

internal static class SauceLabsClient
{
    public static async Task<List<SauceVirtualRow>> GetVirtualPlatformsAsync(
        HttpClient http,
        string region)
    {
        var url = $"https://api.{region}.saucelabs.com/rest/v1/info/platforms/all";
        using var document = await GetJsonAsync(http, url);

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"Unexpected Sauce platform response from {url}.");
        }

        return [.. document.RootElement
            .EnumerateArray()
            .Select(item => new SauceVirtualRow(
                AutomationBackend: JsonValue(item, "automation_backend"),
                ApiName: JsonValue(item, "api_name"),
                LongName: JsonValue(item, "long_name"),
                Device: JsonValue(item, "device"),
                Os: JsonValue(item, "os"),
                ShortVersion: JsonValue(item, "short_version"),
                LongVersion: JsonValue(item, "long_version"),
                RecommendedBackendVersion: JsonValue(item, "recommended_backend_version")))];
    }

    public static async Task<List<SauceRealDevice>> GetRealDevicesAsync(
        HttpClient http,
        string region)
    {
        var url = $"https://api.{region}.saucelabs.com/v1/rdc/devices";
        using var document = await GetJsonAsync(http, url);

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"Unexpected Sauce real-device response from {url}.");
        }

        var result = new List<SauceRealDevice>();

        foreach (var item in document.RootElement.EnumerateArray())
        {
            var id = JsonValue(item, "id");
            var name = JsonValue(item, "name");
            var os = JsonValue(item, "os");
            var osVersion = JsonValue(item, "osVersion");
            var manufacturer = JsonArrayOrScalar(item, "manufacturer");

            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            result.Add(new SauceRealDevice(id, name, os, osVersion, manufacturer));
        }

        return result;
    }

    private static async Task<JsonDocument> GetJsonAsync(HttpClient http, string url)
    {
        using var response = await http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            var preview = body.Length > 500 ? body[..500] : body;
            throw new HttpRequestException(
                $"GET {url} returned {(int)response.StatusCode} {response.ReasonPhrase}. {preview}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }

    private static string JsonValue(JsonElement item, string property)
    {
        if (!item.TryGetProperty(property, out var value))
        {
            return string.Empty;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => string.Empty
        };
    }

    private static string JsonArrayOrScalar(JsonElement item, string property)
    {
        if (!item.TryGetProperty(property, out var value))
        {
            return string.Empty;
        }

        if (value.ValueKind == JsonValueKind.Array)
        {
            return string.Join(", ", value.EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString())
                .Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        return JsonValue(item, property);
    }
}

internal sealed record SauceVirtualRow(
    string AutomationBackend,
    string ApiName,
    string LongName,
    string Device,
    string Os,
    string ShortVersion,
    string LongVersion,
    string RecommendedBackendVersion);

internal sealed record SauceRealDevice(
    string Id,
    string Name,
    string Os,
    string OsVersion,
    string Manufacturer);

internal enum PlatformRequirementKind
{
    DesktopPlatform,
    MobileExactVersion,
    MobileMajorWildcard
}

internal sealed record FunctionalPlatformRequirement(
    PlatformRequirementKind Kind,
    string Family,
    string FamilyKey,
    string DisplayName,
    string Version,
    string SortKey,
    int SauceConfigurationCount,
    IReadOnlyList<string> RepresentativeDevices,
    IReadOnlyList<string> SourceDescriptions);

internal static class FunctionalPlatformBuilder
{
    private static readonly Dictionary<int, string> MacNames =
        new() {
            [11] = "Big Sur",
            [12] = "Monterey",
            [13] = "Ventura",
            [14] = "Sonoma",
            [15] = "Sequoia",
            [26] = "Tahoe",
            [27] = "Golden Gate"
        };

    public static List<FunctionalPlatformRequirement> Build(
        IReadOnlyList<SauceVirtualRow> virtualRows,
        IReadOnlyList<SauceRealDevice> realDevices)
    {
        var accumulators = new Dictionary<string, RequirementAccumulator>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in virtualRows)
        {
            if (TryGetMobileFamily(row, out var mobileFamily))
            {
                var version = NormalizeMobileVersion(FirstNonEmpty(row.ShortVersion, row.LongVersion));
                if (!string.IsNullOrWhiteSpace(version))
                {
                    AddMobileExact(accumulators, mobileFamily, version, DescribeVirtual(row), RepresentativeDevice(row));
                    AddMobileWildcard(accumulators, mobileFamily, version, DescribeVirtual(row), RepresentativeDevice(row));
                }

                continue;
            }

            // Browser name/version permutations do not define platform coverage.
            // A Selenium row contributes only its OS platform.
            var desktop = NormalizeDesktopPlatform(row.Os);
            if (desktop is null)
            {
                continue;
            }

            var key = $"desktop|{desktop.Value.FamilyKey}|{desktop.Value.Canonical}";
            GetOrCreate(
                accumulators,
                key,
                PlatformRequirementKind.DesktopPlatform,
                desktop.Value.Family,
                desktop.Value.FamilyKey,
                desktop.Value.DisplayName,
                desktop.Value.Version,
                desktop.Value.SortKey)
                .Add(DescribeVirtual(row), RepresentativeDevice(row));
        }

        foreach (var device in realDevices)
        {
            var family = NormalizeRealMobileFamily(device.Os);
            var version = NormalizeMobileVersion(device.OsVersion);

            if (family is null || string.IsNullOrWhiteSpace(version))
            {
                continue;
            }

            var source = $"Real device: {device.Os} {device.OsVersion}";
            var sample = FirstNonEmpty(device.Name, device.Id);
            AddMobileExact(accumulators, family, version, source, sample);
            AddMobileWildcard(accumulators, family, version, source, sample);
        }

        return [.. accumulators.Values
            .Select(x => x.Build())
            .OrderBy(x => x.Family)
            .ThenBy(x => x.SortKey, NaturalStringComparer.Instance)];
    }

    private static void AddMobileExact(
        IDictionary<string, RequirementAccumulator> accumulators,
        string family,
        string version,
        string source,
        string sample)
    {
        var key = $"mobile-exact|{family}|{version}";
        GetOrCreate(
            accumulators,
            key,
            PlatformRequirementKind.MobileExactVersion,
            family,
            $"mobile:{family}",
            $"{family} {version}",
            version,
            $"{family}|0|{version}")
            .Add(source, sample);
    }

    private static void AddMobileWildcard(
        IDictionary<string, RequirementAccumulator> accumulators,
        string family,
        string exactVersion,
        string source,
        string sample)
    {
        var major = ParseMajor(exactVersion);
        if (major is null)
        {
            return;
        }

        var wildcard = $"{major.Value}.x";
        var key = $"mobile-wildcard|{family}|{wildcard}";
        GetOrCreate(
            accumulators,
            key,
            PlatformRequirementKind.MobileMajorWildcard,
            family,
            $"mobile:{family}",
            $"{family} {wildcard}",
            wildcard,
            $"{family}|1|{major.Value:D3}")
            .Add(source, sample);
    }

    private static RequirementAccumulator GetOrCreate(
        IDictionary<string, RequirementAccumulator> accumulators,
        string key,
        PlatformRequirementKind kind,
        string family,
        string familyKey,
        string displayName,
        string version,
        string sortKey)
    {
        if (!accumulators.TryGetValue(key, out var accumulator))
        {
            accumulator = new RequirementAccumulator(
                kind,
                family,
                familyKey,
                displayName,
                version,
                sortKey);
            accumulators[key] = accumulator;
        }

        return accumulator;
    }

    private static bool TryGetMobileFamily(SauceVirtualRow row, out string family)
    {
        var api = row.ApiName.Trim().ToLowerInvariant();
        var device = $"{row.Device} {row.LongName}";
        var os = row.Os.Trim();

        if (api is "iphone" or "ipad" or "ios" ||
            device.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
            device.Contains("iPad", StringComparison.OrdinalIgnoreCase) ||
            os.Equals("iOS", StringComparison.OrdinalIgnoreCase) ||
            os.Equals("IOS", StringComparison.OrdinalIgnoreCase))
        {
            family = "iOS";
            return true;
        }

        if (api == "android" ||
            device.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
            device.Contains("Emulator", StringComparison.OrdinalIgnoreCase) ||
            os.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            family = "Android";
            return true;
        }

        family = string.Empty;
        return false;
    }

    private static string? NormalizeRealMobileFamily(string os) 
        => os.Equals("Android", StringComparison.OrdinalIgnoreCase)
            ? "Android"
            : os.Equals("iOS", StringComparison.OrdinalIgnoreCase) || 
              os.Equals("IOS", StringComparison.OrdinalIgnoreCase)
                ? "iOS"
                : null;

    private static (string Family, string FamilyKey, string Canonical, string DisplayName, string Version, string SortKey)?
        NormalizeDesktopPlatform(string os)
    {
        if (string.IsNullOrWhiteSpace(os))
        {
            return null;
        }

        os = Regex.Replace(os.Trim(), @"\s+", " ");

        if (os.Equals("ChromiumOS", StringComparison.OrdinalIgnoreCase) ||
            os.Equals("ChromeOS", StringComparison.OrdinalIgnoreCase))
        {
            return ("ChromiumOS", "desktop:ChromiumOS", "ChromiumOS", "ChromiumOS", string.Empty, "ChromiumOS");
        }

        if (os.StartsWith("Windows", StringComparison.OrdinalIgnoreCase))
        {
            return ("Windows", "desktop:Windows", os, os, ExtractTrailingVersion(os), os);
        }

        if (os.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
        {
            return ("Linux", "desktop:Linux", os, os, ExtractTrailingVersion(os), os);
        }

        var macMatch = Regex.Match(os, @"^(?:Mac|macOS|OS X)\s+(?<version>\d+(?:\.\d+)*)$", RegexOptions.IgnoreCase);
        if (macMatch.Success)
        {
            var version = macMatch.Groups["version"].Value;
            var major = ParseMajor(version);
            var marketingName = major is not null && MacNames.TryGetValue(major.Value, out var name)
                ? name
                : string.Empty;

            var display = string.IsNullOrWhiteSpace(marketingName)
                ? $"Mac {version}"
                : $"Mac {marketingName} ({version})";

            return ("macOS", "desktop:macOS", $"Mac {version}", display, version, $"Mac|{version}");
        }

        // Keep other genuine OS platform names, but never browser/device names.
        return (os, $"desktop:{os}", os, os, ExtractTrailingVersion(os), os);
    }

    private static string NormalizeMobileVersion(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        var match = Regex.Match(raw.Trim(), @"(?<major>\d+)(?:\.(?<minor>\d+))?");
        if (!match.Success)
        {
            return string.Empty;
        }

        var major = match.Groups["major"].Value;
        var minor = match.Groups["minor"].Success ? match.Groups["minor"].Value : "0";
        return $"{major}.{minor}";
    }

    private static int? ParseMajor(string version)
    {
        var match = Regex.Match(version, @"^\s*(\d+)");
        return match.Success && int.TryParse(match.Groups[1].Value, out var major)
            ? major
            : null;
    }

    private static string ExtractTrailingVersion(string value)
    {
        var match = Regex.Match(value, @"(\d+(?:\.\d+)*)\s*$");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static string DescribeVirtual(SauceVirtualRow row) =>
        $"Virtual: api={row.ApiName}; os={row.Os}; version={FirstNonEmpty(row.ShortVersion, row.LongVersion)}; device={row.Device}";

    private static string RepresentativeDevice(SauceVirtualRow row) =>
        FirstNonEmpty(row.Device, row.LongName);

    private static string FirstNonEmpty(params string[] values) 
        => values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;

    private sealed class RequirementAccumulator(
        PlatformRequirementKind kind,
        string family,
        string familyKey,
        string displayName,
        string version,
        string sortKey) {
        private readonly HashSet<string> _sources = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _devices = new(StringComparer.OrdinalIgnoreCase);
        private int _count;

        public PlatformRequirementKind Kind { get; } = kind;
        public string Family { get; } = family;
        public string FamilyKey { get; } = familyKey;
        public string DisplayName { get; } = displayName;
        public string Version { get; } = version;
        public string SortKey { get; } = sortKey;

        public void Add(string source, string device)
        {
            _count++;
            if (!string.IsNullOrWhiteSpace(source) && _sources.Count < 8)
            {
                _sources.Add(source);
            }

            if (!string.IsNullOrWhiteSpace(device) && _devices.Count < 5)
            {
                _devices.Add(device);
            }
        }

        public FunctionalPlatformRequirement Build() =>
            new(Kind,
                Family,
                FamilyKey,
                DisplayName,
                Version,
                SortKey,
                _count,
                [.. _devices.OrderBy(x => x)],
                [.. _sources.OrderBy(x => x)]);
    }
}

internal sealed class SourceCorpus
{
    private readonly IReadOnlyList<SourceFile> _files;

    private SourceCorpus(IReadOnlyList<SourceFile> files)
    {
        _files = files;
    }

    public static SourceCorpus Load(string root)
    {
        var files = Directory
            .EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(path => !ContainsBuildDirectory(path))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(path => new SourceFile(
                Path.GetRelativePath(root, path),
                File.ReadAllText(path)))
            .ToArray();

        return new SourceCorpus(files);
    }

    public Evidence Find(FunctionalPlatformRequirement requirement)
    {
        var familyAliases = FamilyAliases(requirement);
        var directAliases = DirectAliases(requirement);

        foreach (var file in _files)
        {
            if (directAliases.Any(file.Contains))
            {
                return new Evidence(true, EvidenceStrength.Direct, file.RelativePath,
                    directAliases.First(file.Contains));
            }
        }

        if (requirement.Kind == PlatformRequirementKind.MobileExactVersion)
        {
            var versionAliases = ExactVersionAliases(requirement.Version);
            foreach (var file in _files)
            {
                if (HasFamilyContext(file, familyAliases) && versionAliases.Any(file.Contains))
                {
                    return new Evidence(true, EvidenceStrength.Contextual, file.RelativePath,
                        versionAliases.First(file.Contains));
                }
            }
        }

        if (requirement.Kind == PlatformRequirementKind.MobileMajorWildcard)
        {
            var wildcardAliases = WildcardAliases(requirement.Version);
            foreach (var file in _files)
            {
                if (HasFamilyContext(file, familyAliases) && wildcardAliases.Any(file.Contains))
                {
                    return new Evidence(true, EvidenceStrength.Contextual, file.RelativePath,
                        wildcardAliases.First(file.Contains));
                }
            }
        }

        if (requirement.Kind == PlatformRequirementKind.DesktopPlatform)
        {
            foreach (var file in _files)
            {
                if (!HasFamilyContext(file, familyAliases))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(requirement.Version))
                {
                    return new Evidence(true, EvidenceStrength.Contextual, file.RelativePath,
                        familyAliases.First(file.Contains));
                }

                // A bare desktop version number such as "27" or "11" is not evidence by itself.
                // Versioned desktop coverage must have been found through a platform-specific
                // direct alias such as "Mac 27", "Golden Gate", or "Windows 11" above.
            }
        }

        var familyOnly = _files.FirstOrDefault(file => HasFamilyContext(file, familyAliases));
        if (familyOnly is not null)
        {
            return new Evidence(false, EvidenceStrength.FamilyOnly, familyOnly.RelativePath,
                familyAliases.First(familyOnly.Contains));
        }

        return Evidence.None;
    }

    public Evidence FindFamilySample(string familyKey, string family)
    {
        var aliases = FamilyAliases(familyKey, family);
        foreach (var file in _files)
        {
            if (HasFamilyContext(file, aliases))
            {
                return new Evidence(true, EvidenceStrength.FamilyOnly, file.RelativePath,
                    aliases.First(file.Contains));
            }
        }

        return Evidence.None;
    }

    private static bool HasFamilyContext(SourceFile file, IReadOnlyList<string> aliases) =>
        aliases.Any(file.Contains) || aliases.Any(file.PathContains);

    private static IReadOnlyList<string> FamilyAliases(FunctionalPlatformRequirement requirement) =>
        FamilyAliases(requirement.FamilyKey, requirement.Family);

    private static IReadOnlyList<string> FamilyAliases(string familyKey, string family)
    {
        if (familyKey.Equals("mobile:iOS", StringComparison.OrdinalIgnoreCase))
        {
            return ["ios", "iphone", "ipad"];
        }

        if (familyKey.Equals("mobile:Android", StringComparison.OrdinalIgnoreCase))
        {
            return ["android"];
        }

        if (familyKey.Equals("desktop:macOS", StringComparison.OrdinalIgnoreCase))
        {
            return ["macos", "mac os", "mac", "os x"];
        }

        if (familyKey.Equals("desktop:Windows", StringComparison.OrdinalIgnoreCase))
        {
            return ["windows"];
        }

        if (familyKey.Equals("desktop:ChromiumOS", StringComparison.OrdinalIgnoreCase))
        {
            return ["chromiumos", "chromium os", "chromeos", "chrome os"];
        }

        if (familyKey.Equals("desktop:Linux", StringComparison.OrdinalIgnoreCase))
        {
            return ["linux"];
        }

        return [Normalize(family)];
    }

    private static IReadOnlyList<string> DirectAliases(FunctionalPlatformRequirement requirement)
    {
        var values = new List<string>();

        if (requirement.FamilyKey == "desktop:ChromiumOS")
        {
            values.AddRange(["chromiumos", "chromium os", "chromeos", "chrome os"]);
        }

        if (requirement.FamilyKey == "desktop:macOS" && !string.IsNullOrWhiteSpace(requirement.Version))
        {
            values.Add($"mac {requirement.Version}");
            values.Add($"macos {requirement.Version}");

            var nameMatch = Regex.Match(requirement.DisplayName, @"Mac\s+(.+)\s+\(");
            if (nameMatch.Success)
            {
                values.Add(nameMatch.Groups[1].Value);
            }
        }

        if (requirement.FamilyKey == "desktop:Windows" && !string.IsNullOrWhiteSpace(requirement.Version))
        {
            values.Add($"windows {requirement.Version}");
        }

        if (requirement.FamilyKey == "mobile:iOS")
        {
            values.Add($"ios {requirement.Version}");
            values.Add($"iphone {requirement.Version}");
            values.Add($"ipad {requirement.Version}");
        }

        if (requirement.FamilyKey == "mobile:Android")
        {
            values.Add($"android {requirement.Version}");
        }

        return [.. values.Select(Normalize).Distinct()];
    }

    private static IReadOnlyList<string> ExactVersionAliases(string version)
    {
        var normalized = Normalize(version);
        var compact = version.Replace(".", string.Empty, StringComparison.Ordinal);
        var underscore = version.Replace('.', '_');
        return [
            normalized,
            Normalize($"v{version}"),
            Normalize($"version {version}"),
            Normalize($"v{underscore}"),
            Normalize($"version{underscore}"),
            Normalize(compact)
        ];
    }

    private static IReadOnlyList<string> WildcardAliases(string version)
    {
        var major = Regex.Match(version, @"^(\d+)").Groups[1].Value;
        return [
            Normalize(version),
            Normalize($"{major}x"),
            Normalize($"v{major}x"),
            Normalize($"version {major} x")
        ];
    }

    private static string Normalize(string value)
    {
        var builder = new StringBuilder(value.Length);
        var previousSpace = false;

        foreach (var ch in value)
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(char.ToLowerInvariant(ch));
                previousSpace = false;
            }
            else if (!previousSpace)
            {
                builder.Append(' ');
                previousSpace = true;
            }
        }

        return builder.ToString().Trim();
    }

    private static bool ContainsBuildDirectory(string path)
    {
        var parts = path.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);

        return parts.Any(x =>
            x.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
            x.Equals("obj", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class SourceFile(string relativePath, string text) {
        private readonly string _normalizedText = Normalize(text);
        private readonly string _normalizedPath = Normalize(relativePath);

        public string RelativePath { get; } = relativePath;

        public bool Contains(string value)
        {
            var normalized = Normalize(value);
            return normalized.Length > 0 &&
                   _normalizedText.Contains(normalized, StringComparison.OrdinalIgnoreCase);
        }

        public bool PathContains(string value)
        {
            var normalized = Normalize(value);
            return normalized.Length > 0 &&
                   _normalizedPath.Contains(normalized, StringComparison.OrdinalIgnoreCase);
        }
    }
}

internal enum EvidenceStrength
{
    None,
    FamilyOnly,
    Contextual,
    Direct
}

internal sealed record Evidence(
    bool Found,
    EvidenceStrength Strength,
    string File,
    string Match)
{
    public static Evidence None { get; } = new(false, EvidenceStrength.None, string.Empty, string.Empty);
}

internal enum CoverageStatus
{
    Covered,
    Review,
    Gap
}

internal enum TestSampleStatus
{
    None,
    FamilySample,
    DirectPlatformSample
}

internal sealed record CoverageFinding(
    FunctionalPlatformRequirement Requirement,
    CoverageStatus CoreStatus,
    Evidence CoreEvidence,
    TestSampleStatus TestSampleStatus,
    Evidence TestEvidence);

internal static class CoverageAnalyzer
{
    public static CoverageFinding Analyze(
        FunctionalPlatformRequirement requirement,
        SourceCorpus core,
        SourceCorpus tests)
    {
        var coreEvidence = core.Find(requirement);
        var coreStatus = coreEvidence switch
        {
            { Found: true, Strength: EvidenceStrength.Direct or EvidenceStrength.Contextual } => CoverageStatus.Covered,
            { Strength: EvidenceStrength.FamilyOnly } => CoverageStatus.Review,
            _ => CoverageStatus.Gap
        };

        var directTest = tests.Find(requirement);
        TestSampleStatus sampleStatus;
        Evidence sampleEvidence;

        if (directTest.Found)
        {
            sampleStatus = TestSampleStatus.DirectPlatformSample;
            sampleEvidence = directTest;
        }
        else
        {
            var familySample = tests.FindFamilySample(requirement.FamilyKey, requirement.Family);
            sampleStatus = familySample.Found ? TestSampleStatus.FamilySample : TestSampleStatus.None;
            sampleEvidence = familySample;
        }

        return new CoverageFinding(
            requirement,
            coreStatus,
            coreEvidence,
            sampleStatus,
            sampleEvidence);
    }
}


internal sealed record DeclaredPlatform(
    PlatformRequirementKind Kind,
    string Family,
    string FamilyKey,
    string DisplayName,
    string Version,
    string CanonicalKey,
    string File,
    string Match);

internal sealed record PlatformLifecycleResult(
    IReadOnlyList<DeclaredPlatform> DeclaredPlatforms,
    IReadOnlyList<DeclaredPlatform> ObsoleteCandidates);

internal static class PlatformCanonicalizer
{
    public static string RequirementKey(FunctionalPlatformRequirement requirement) =>
        Key(requirement.Kind, requirement.FamilyKey, requirement.Version, requirement.DisplayName);

    public static string Key(
        PlatformRequirementKind kind,
        string familyKey,
        string version,
        string displayName)
    {
        return kind switch
        {
            PlatformRequirementKind.MobileExactVersion =>
                $"mobile-exact|{familyKey}|{NormalizeMobileExactVersion(version)}",
            PlatformRequirementKind.MobileMajorWildcard =>
                $"mobile-wildcard|{familyKey}|{NormalizeWildcard(version)}",
            PlatformRequirementKind.DesktopPlatform =>
                $"desktop|{familyKey}|{NormalizeDesktopVersion(version, displayName)}",
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    private static string NormalizeMobileExactVersion(string value)
    {
        var match = Regex.Match(
            value,
            @"^\s*(?<major>\d+)(?:\.(?<minor>\d+))?\s*$",
            RegexOptions.CultureInvariant);

        if (!match.Success)
        {
            return value.Trim().ToLowerInvariant();
        }

        var major = int.Parse(match.Groups["major"].Value);
        var minor = match.Groups["minor"].Success
            ? int.Parse(match.Groups["minor"].Value)
            : 0;

        return $"{major}.{minor}";
    }

    private static string NormalizeWildcard(string value)
    {
        var match = Regex.Match(value, @"(?<major>\d+)", RegexOptions.CultureInvariant);
        return match.Success
            ? $"{int.Parse(match.Groups["major"].Value)}.x"
            : value.Trim().ToLowerInvariant();
    }

    private static string NormalizeDesktopVersion(string version, string displayName)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return Regex.Replace(displayName.Trim().ToLowerInvariant(), @"\s+", " ");
        }

        var match = Regex.Match(
            version,
            @"^\s*(?<major>\d+)(?:\.(?<minor>\d+))?(?:\.(?<patch>\d+))?\s*$",
            RegexOptions.CultureInvariant);

        if (!match.Success)
        {
            return version.Trim().ToLowerInvariant();
        }

        var parts = new List<int>
        {
            int.Parse(match.Groups["major"].Value)
        };

        if (match.Groups["minor"].Success)
        {
            parts.Add(int.Parse(match.Groups["minor"].Value));
        }

        if (match.Groups["patch"].Success)
        {
            parts.Add(int.Parse(match.Groups["patch"].Value));
        }

        while (parts.Count > 1 && parts[^1] == 0)
        {
            parts.RemoveAt(parts.Count - 1);
        }

        return string.Join('.', parts);
    }
}

internal static class SauceryPlatformExtractor
{
    private static readonly Dictionary<int, string> MacMarketingNames = new()
    {
        [11] = "Big Sur",
        [12] = "Monterey",
        [13] = "Ventura",
        [14] = "Sonoma",
        [15] = "Sequoia",
        [26] = "Tahoe",
        [27] = "Golden Gate"
    };

    private static readonly Regex MobileMappingRegex = new(
        "\\\"(?<label>[^\\\"]+)\\\"\\s*=>.*?GetPlatform<(?<type>Android\\d+Platform|IOS\\d+Platform)>",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex DesktopPlatformTypeRegex = new(
        @"GetPlatform<(?<type>LinuxPlatform|Windows\d+Platform|Mac\d+Platform)>",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex DesktopCreatorTypeRegex = new(
        @"new\s+(?<type>LinuxBrowserCreator|Windows\d+BrowserCreator|Mac\d+BrowserCreator)\b",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static List<DeclaredPlatform> Extract(string coreRoot)
    {
        var found = new Dictionary<string, DeclaredPlatform>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in Directory
                     .EnumerateFiles(coreRoot, "*.cs", SearchOption.AllDirectories)
                     .Where(path => !ContainsBuildDirectory(path))
                     .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var relativePath = Path.GetRelativePath(coreRoot, path);
            var lines = File.ReadAllLines(path);

            for (var i = 0; i < lines.Length; i++)
            {
                var sourceText = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(sourceText) ||
                    sourceText is "{" or "}" or "," or ";")
                {
                    continue;
                }

                ExtractMobileMapping(found, relativePath, i + 1, sourceText);
                ExtractDesktopMapping(found, relativePath, i + 1, sourceText);
                ExtractChromiumOs(found, relativePath, i + 1, sourceText);
            }
        }

        return [.. found.Values
            .OrderBy(x => x.Family)
            .ThenBy(x => x.Version, NaturalStringComparer.Instance)
            .ThenBy(x => x.DisplayName, StringComparer.OrdinalIgnoreCase)];
    }

    private static void ExtractMobileMapping(
        IDictionary<string, DeclaredPlatform> found,
        string file,
        int lineNumber,
        string sourceText)
    {
        var match = MobileMappingRegex.Match(sourceText);
        if (!match.Success)
        {
            return;
        }

        var label = match.Groups["label"].Value.Trim();
        var typeName = match.Groups["type"].Value;

        var family = typeName.StartsWith("Android", StringComparison.Ordinal)
            ? "Android"
            : "iOS";
        var familyKey = family == "Android" ? "mobile:Android" : "mobile:iOS";

        if (family == "Android")
        {
            if (!label.StartsWith("Linux ", StringComparison.OrdinalIgnoreCase) &&
                !label.StartsWith("Android ", StringComparison.OrdinalIgnoreCase))
            {
                throw ContradictoryDeclaration(file, lineNumber, sourceText,
                    $"Android platform type '{typeName}' has unexpected label '{label}'.");
            }
        }
        else if (!label.StartsWith("iOS ", StringComparison.OrdinalIgnoreCase))
        {
            throw ContradictoryDeclaration(file, lineNumber, sourceText,
                $"iOS platform type '{typeName}' has unexpected label '{label}'.");
        }

        var versionMatch = Regex.Match(
            label,
            @"(?<major>\d{1,3})(?:\.(?<minor>\d{1,3}))?\s*$",
            RegexOptions.CultureInvariant);

        if (!versionMatch.Success)
        {
            throw ContradictoryDeclaration(file, lineNumber, sourceText,
                $"Could not parse a platform version from '{label}'.");
        }

        var major = int.Parse(versionMatch.Groups["major"].Value);
        var hasMinor = versionMatch.Groups["minor"].Success;

        if (hasMinor)
        {
            var minor = int.Parse(versionMatch.Groups["minor"].Value);
            var version = $"{major}.{minor}";
            Add(
                found,
                PlatformRequirementKind.MobileExactVersion,
                family,
                familyKey,
                $"{family} {version}",
                version,
                file,
                lineNumber,
                sourceText);
        }
        else
        {
            var version = $"{major}.x";
            Add(
                found,
                PlatformRequirementKind.MobileMajorWildcard,
                family,
                familyKey,
                $"{family} {version}",
                version,
                file,
                lineNumber,
                sourceText);
        }
    }

    private static void ExtractDesktopMapping(
        IDictionary<string, DeclaredPlatform> found,
        string file,
        int lineNumber,
        string sourceText)
    {
        var platformTypeMatch = DesktopPlatformTypeRegex.Match(sourceText);
        var creatorTypeMatch = DesktopCreatorTypeRegex.Match(sourceText);

        if (!platformTypeMatch.Success && !creatorTypeMatch.Success)
        {
            return;
        }

        var typeName = platformTypeMatch.Success
            ? platformTypeMatch.Groups["type"].Value
            : creatorTypeMatch.Groups["type"].Value;

        if (typeName.StartsWith("Linux", StringComparison.Ordinal))
        {
            Add(
                found,
                PlatformRequirementKind.DesktopPlatform,
                "Linux",
                "desktop:Linux",
                "Linux",
                string.Empty,
                file,
                lineNumber,
                sourceText);
            return;
        }

        if (typeName.StartsWith("Windows", StringComparison.Ordinal))
        {
            var digits = Regex.Match(typeName, @"Windows(?<digits>\d+)").Groups["digits"].Value;
            var version = ParseWindowsVersion(digits, file, lineNumber, sourceText);

            Add(
                found,
                PlatformRequirementKind.DesktopPlatform,
                "Windows",
                "desktop:Windows",
                $"Windows {version}",
                version,
                file,
                lineNumber,
                sourceText);
            return;
        }

        if (typeName.StartsWith("Mac", StringComparison.Ordinal))
        {
            var digits = Regex.Match(typeName, @"Mac(?<digits>\d+)").Groups["digits"].Value;
            if (!int.TryParse(digits, out var major))
            {
                throw ContradictoryDeclaration(file, lineNumber, sourceText,
                    $"Could not parse macOS version from type '{typeName}'.");
            }

            var version = major.ToString();
            var displayName = MacMarketingNames.TryGetValue(major, out var marketingName)
                ? $"Mac {marketingName} ({version})"
                : $"Mac {version}";

            Add(
                found,
                PlatformRequirementKind.DesktopPlatform,
                "macOS",
                "desktop:macOS",
                displayName,
                version,
                file,
                lineNumber,
                sourceText);
        }
    }

    private static void ExtractChromiumOs(
        IDictionary<string, DeclaredPlatform> found,
        string file,
        int lineNumber,
        string sourceText)
    {
        var hasExplicitDeclaration =
            sourceText.Contains("ChromiumOS", StringComparison.OrdinalIgnoreCase) ||
            sourceText.Contains("Chromium OS", StringComparison.OrdinalIgnoreCase) ||
            sourceText.Contains("ChromeOS", StringComparison.OrdinalIgnoreCase) ||
            sourceText.Contains("Chrome OS", StringComparison.OrdinalIgnoreCase);

        if (!hasExplicitDeclaration)
        {
            return;
        }

        if (!sourceText.Contains("PLATFORM_", StringComparison.OrdinalIgnoreCase) &&
            !sourceText.Contains("Platform", StringComparison.OrdinalIgnoreCase) &&
            !sourceText.Contains("BrowserCreator", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Add(
            found,
            PlatformRequirementKind.DesktopPlatform,
            "ChromiumOS",
            "desktop:ChromiumOS",
            "ChromiumOS",
            string.Empty,
            file,
            lineNumber,
            sourceText);
    }

    private static string ParseWindowsVersion(
        string digits,
        string file,
        int lineNumber,
        string sourceText)
    {
        if (digits == "81")
        {
            return "8.1";
        }

        if (int.TryParse(digits, out var major))
        {
            return major.ToString();
        }

        throw ContradictoryDeclaration(file, lineNumber, sourceText,
            $"Could not parse Windows version from '{digits}'.");
    }

    private static void Add(
        IDictionary<string, DeclaredPlatform> found,
        PlatformRequirementKind kind,
        string family,
        string familyKey,
        string displayName,
        string version,
        string file,
        int lineNumber,
        string sourceText)
    {
        ValidateDeclaration(kind, family, version, file, lineNumber, sourceText);

        var key = PlatformCanonicalizer.Key(kind, familyKey, version, displayName);
        if (found.ContainsKey(key))
        {
            return;
        }

        found[key] = new DeclaredPlatform(
            kind,
            family,
            familyKey,
            displayName,
            version,
            key,
            file,
            $"line {lineNumber}: {sourceText}");
    }

    private static void ValidateDeclaration(
        PlatformRequirementKind kind,
        string family,
        string version,
        string file,
        int lineNumber,
        string sourceText)
    {
        if (string.IsNullOrWhiteSpace(sourceText) ||
            sourceText.Trim() is "{" or "}" or "," or ";")
        {
            throw ContradictoryDeclaration(file, lineNumber, sourceText,
                "A platform declaration cannot be represented by punctuation or an empty source line.");
        }

        if (family == "Android" &&
            !sourceText.Contains("Android", StringComparison.Ordinal))
        {
            throw ContradictoryDeclaration(file, lineNumber, sourceText,
                "Android declaration does not contain an Android platform type.");
        }

        if (family == "iOS" &&
            !sourceText.Contains("IOS", StringComparison.Ordinal) &&
            !sourceText.Contains("iOS", StringComparison.OrdinalIgnoreCase))
        {
            throw ContradictoryDeclaration(file, lineNumber, sourceText,
                "iOS declaration does not contain an iOS platform type or label.");
        }

        if (family == "Linux" &&
            sourceText.Contains("Android", StringComparison.Ordinal))
        {
            throw ContradictoryDeclaration(file, lineNumber, sourceText,
                "Linux declaration contains an Android platform type.");
        }

        if (kind != PlatformRequirementKind.DesktopPlatform && string.IsNullOrWhiteSpace(version))
        {
            throw ContradictoryDeclaration(file, lineNumber, sourceText,
                "Mobile platform declaration has no version.");
        }
    }

    private static InvalidDataException ContradictoryDeclaration(
        string file,
        int lineNumber,
        string sourceText,
        string detail) =>
        new($"Invalid Saucery platform declaration extraction at {file}:{lineNumber}. {detail} Source: {sourceText}");

    private static bool ContainsBuildDirectory(string path)
    {
        var parts = path.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);

        return parts.Any(x =>
            x.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
            x.Equals("obj", StringComparison.OrdinalIgnoreCase));
    }
}

internal static class PlatformLifecycleAnalyzer
{
    public static PlatformLifecycleResult Analyze(
        IReadOnlyList<FunctionalPlatformRequirement> sauceRequirements,
        IReadOnlyList<DeclaredPlatform> declaredPlatforms)
    {
        var sauceKeys = sauceRequirements
            .Select(PlatformCanonicalizer.RequirementKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var sauceFamilies = sauceRequirements
            .Select(x => x.FamilyKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var obsolete = declaredPlatforms
            .Where(x => !sauceKeys.Contains(x.CanonicalKey))
            .Where(x => ShouldReportAsObsoleteCandidate(x, sauceFamilies))
            .OrderBy(x => x.Family)
            .ThenBy(x => x.Version, NaturalStringComparer.Instance)
            .ThenBy(x => x.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new PlatformLifecycleResult(declaredPlatforms, obsolete);
    }

    private static bool ShouldReportAsObsoleteCandidate(
        DeclaredPlatform platform,
        HashSet<string> sauceFamilies)
    {
        // An explicit exact version or wildcard that is absent from Sauce is a lifecycle candidate.
        if (platform.Kind is PlatformRequirementKind.MobileExactVersion or
            PlatformRequirementKind.MobileMajorWildcard)
        {
            return true;
        }

        // For desktop platforms, report explicit versioned declarations. Also report a whole
        // platform family when Sauce no longer advertises that family at all.
        return !string.IsNullOrWhiteSpace(platform.Version) ||
               !sauceFamilies.Contains(platform.FamilyKey);
    }
}

internal sealed record PackageVersionFinding(
    string Package,
    string? Version,
    string Status,
    string Detail);

internal sealed record DependencyAuditResult(IReadOnlyList<PackageVersionFinding> Findings);

internal static class DependencyAudit
{
    public static DependencyAuditResult Run(string repoRoot)
    {
        var packageFiles = Directory
            .EnumerateFiles(repoRoot, "*.csproj", SearchOption.AllDirectories)
            .Concat(Directory.EnumerateFiles(repoRoot, "Directory.Packages.props", SearchOption.AllDirectories))
            .Where(path => !path.Contains(
                $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
                StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var versions = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in packageFiles)
        {
            try
            {
                var document = XDocument.Load(file);
                foreach (var element in document.Descendants())
                {
                    if (!element.Name.LocalName.Equals("PackageReference", StringComparison.OrdinalIgnoreCase) &&
                        !element.Name.LocalName.Equals("PackageVersion", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var include = element.Attribute("Include")?.Value ?? element.Attribute("Update")?.Value;
                    if (string.IsNullOrWhiteSpace(include))
                    {
                        continue;
                    }

                    var version = element.Attribute("Version")?.Value
                        ?? element.Elements()
                            .FirstOrDefault(x => x.Name.LocalName.Equals("Version", StringComparison.OrdinalIgnoreCase))
                            ?.Value;

                    if (!versions.ContainsKey(include) || !string.IsNullOrWhiteSpace(version))
                    {
                        versions[include] = version;
                    }
                }
            }
            catch
            {
                // Supplementary only; do not fail the platform report on unrelated XML.
            }
        }

        return new DependencyAuditResult([
            CheckMinimumMajor(versions, "Selenium.WebDriver", 4,
                "Selenium 4+ is required by the stated Saucery.Core goal."),
            CheckPresence(versions, "Appium.WebDriver",
                "Appium client support is required by the stated Saucery.Core goal.")
        ]);
    }

    private static PackageVersionFinding CheckPresence(
        Dictionary<string, string?> versions,
        string package,
        string detail)
    {
        if (!versions.TryGetValue(package, out var version))
        {
            return new PackageVersionFinding(package, null, "REVIEW",
                $"{detail} Package reference was not found.");
        }

        return new PackageVersionFinding(package, version, "OK", detail);
    }

    private static PackageVersionFinding CheckMinimumMajor(
        Dictionary<string, string?> versions,
        string package,
        int minimumMajor,
        string detail)
    {
        if (!versions.TryGetValue(package, out var version))
        {
            return new PackageVersionFinding(package, null, "REVIEW",
                $"{detail} Package reference was not found.");
        }

        if (string.IsNullOrWhiteSpace(version))
        {
            return new PackageVersionFinding(package, null, "REVIEW",
                $"{detail} Version could not be resolved statically.");
        }

        var numeric = Regex.Match(version, @"\d+").Value;
        if (int.TryParse(numeric, out var major))
        {
            return new PackageVersionFinding(
                package,
                version,
                major >= minimumMajor ? "OK" : "GAP",
                detail);
        }

        return new PackageVersionFinding(package, version, "REVIEW",
            $"{detail} Version '{version}' could not be interpreted.");
    }
}

internal static class ReportWriter
{
    public static string Write(
        string repoRoot,
        Options options,
        DependencyAuditResult dependencyAudit,
        int virtualRowCount,
        int realDeviceCount,
        IReadOnlyList<CoverageFinding> findings,
        PlatformLifecycleResult lifecycle,
        string? realDeviceWarning)
    {
        var sb = new StringBuilder(128 * 1024);
        var coreGaps = findings.Where(x => x.CoreStatus == CoverageStatus.Gap).ToList();
        var reviews = findings.Where(x => x.CoreStatus == CoverageStatus.Review).ToList();
        var covered = findings.Where(x => x.CoreStatus == CoverageStatus.Covered).ToList();

        var familyGroups = findings
            .GroupBy(x => x.Requirement.FamilyKey)
            .OrderBy(x => x.First().Requirement.Family)
            .ToList();

        var familiesWithoutTests = familyGroups
            .Where(g => g.All(x => x.TestSampleStatus == TestSampleStatus.None))
            .ToList();

        sb.AppendLine("SAUCERY FUNCTIONAL PLATFORM COVERAGE GAP REPORT");
        sb.AppendLine(new string('=', 92));
        sb.AppendLine($"Generated UTC : {DateTimeOffset.UtcNow:O}");
        sb.AppendLine($"Reporter build : {Program.ReporterBuild}");
        sb.AppendLine($"Repository    : {repoRoot}");
        sb.AppendLine($"Region        : {options.Region}");
        sb.AppendLine();

        sb.AppendLine("DEFINITION OF COVERAGE");
        sb.AppendLine(new string('-', 92));
        sb.AppendLine("The unit of coverage is a FUNCTIONAL PLATFORM, not an individual Sauce device.");
        sb.AppendLine("Examples: ChromiumOS, Mac Golden Gate (27), iOS 27.1, iOS 27.x, Android 17.0, Android 17.x.");
        sb.AppendLine("Browser versions, Electron versions, iPhone/iPad models, Pixel models, Galaxy models, etc. do not");
        sb.AppendLine("create independent platform requirements. Device names are retained only as representative evidence.");
        sb.AppendLine("Tests are intentionally sparse: a platform family needs representative test evidence, not one test per device.");
        sb.AppendLine();

        sb.AppendLine("NORMALIZATION RULES");
        sb.AppendLine(new string('-', 92));
        sb.AppendLine("1. Mobile simulator/emulator rows are classified as iOS or Android before host OS is considered.");
        sb.AppendLine("2. Mobile patch versions are collapsed to major.minor functional versions (for example 17.0).");
        sb.AppendLine("3. Every observed mobile major version also creates a major wildcard requirement (for example 17.x).");
        sb.AppendLine("4. Selenium rows collapse to their OS platform; browser/version permutations do not multiply requirements.");
        sb.AppendLine("5. Tests are reported as direct-platform samples or family-level representative samples; missing per-device tests are ignored.");
        sb.AppendLine();

        sb.AppendLine("SAUCE INPUT");
        sb.AppendLine(new string('-', 92));
        sb.AppendLine($"Virtual catalogue rows read : {virtualRowCount,8:N0}");
        sb.AppendLine($"Real devices read           : {realDeviceCount,8:N0}");
        sb.AppendLine($"Functional requirements     : {findings.Count,8:N0}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(realDeviceWarning))
        {
            sb.AppendLine("WARNING");
            sb.AppendLine(new string('-', 92));
            sb.AppendLine(realDeviceWarning);
            sb.AppendLine();
        }

        sb.AppendLine("DEPENDENCY CHECK");
        sb.AppendLine(new string('-', 92));
        foreach (var finding in dependencyAudit.Findings)
        {
            sb.AppendLine($"{finding.Status,-7} {finding.Package,-24} {finding.Version ?? "(unresolved)",-16} {finding.Detail}");
        }
        sb.AppendLine();

        sb.AppendLine("SUMMARY");
        sb.AppendLine(new string('-', 92));
        sb.AppendLine($"Functional platform requirements : {findings.Count,6:N0}");
        sb.AppendLine($"Core covered                     : {covered.Count,6:N0}");
        sb.AppendLine($"Core review                      : {reviews.Count,6:N0}");
        sb.AppendLine($"Core functional gaps             : {coreGaps.Count,6:N0}");
        sb.AppendLine($"Platform families                : {familyGroups.Count,6:N0}");
        sb.AppendLine($"Families with no test sample     : {familiesWithoutTests.Count,6:N0}");
        sb.AppendLine($"Explicit Core platform entries   : {lifecycle.DeclaredPlatforms.Count,6:N0}");
        sb.AppendLine($"Obsolete Core candidates         : {lifecycle.ObsoleteCandidates.Count,6:N0}");
        sb.AppendLine();

        sb.AppendLine("SUMMARY BY PLATFORM FAMILY");
        sb.AppendLine(new string('-', 92));
        foreach (var group in familyGroups)
        {
            var first = group.First();
            var sampleStatus = group.Any(x => x.TestSampleStatus != TestSampleStatus.None) ? "YES" : "NO";
            sb.AppendLine(
                $"{first.Requirement.Family,-18} requirements={group.Count(),4:N0} | " +
                $"covered={group.Count(x => x.CoreStatus == CoverageStatus.Covered),4:N0} | " +
                $"review={group.Count(x => x.CoreStatus == CoverageStatus.Review),4:N0} | " +
                $"gaps={group.Count(x => x.CoreStatus == CoverageStatus.Gap),4:N0} | " +
                $"representative tests={sampleStatus}");
        }
        sb.AppendLine();

        WriteCoreGapSection(sb, "CORE FUNCTIONAL PLATFORM GAPS", coreGaps);
        WriteCoreGapSection(sb, "CORE REVIEW ITEMS", reviews);

        WriteObsoleteSection(sb, lifecycle.ObsoleteCandidates);

        sb.AppendLine("REPRESENTATIVE TEST COVERAGE");
        sb.AppendLine(new string('=', 92));
        foreach (var group in familyGroups)
        {
            var first = group.First();
            var evidence = group
                .Where(x => x.TestSampleStatus != TestSampleStatus.None)
                .OrderByDescending(x => x.TestSampleStatus)
                .Select(x => x.TestEvidence)
                .FirstOrDefault();

            if (evidence is null || !evidence.Found)
            {
                sb.AppendLine($"GAP     {first.Requirement.Family}: no representative test evidence found.");
            }
            else
            {
                sb.AppendLine($"SAMPLED {first.Requirement.Family}: {evidence.File} (matched '{evidence.Match}')");
            }
        }
        sb.AppendLine();

        sb.AppendLine("ALL FUNCTIONAL PLATFORM REQUIREMENTS");
        sb.AppendLine(new string('=', 92));
        foreach (var finding in findings)
        {
            WriteFinding(sb, finding);
        }

        return sb.ToString();
    }

    private static void WriteCoreGapSection(
        StringBuilder sb,
        string title,
        List<CoverageFinding> findings)
    {
        sb.AppendLine(title);
        sb.AppendLine(new string('=', 92));

        if (findings.Count == 0)
        {
            sb.AppendLine("None.");
            sb.AppendLine();
            return;
        }

        foreach (var finding in findings)
        {
            sb.AppendLine($"{finding.CoreStatus,-7} {finding.Requirement.DisplayName}");
            sb.AppendLine($"        Sauce rows collapsed into this requirement: {finding.Requirement.SauceConfigurationCount:N0}");
            sb.AppendLine($"        Core evidence: {FormatEvidence(finding.CoreEvidence)}");
            if (finding.Requirement.RepresentativeDevices.Count > 0)
            {
                sb.AppendLine($"        Example devices: {string.Join("; ", finding.Requirement.RepresentativeDevices.Take(3))}");
            }
        }

        sb.AppendLine();
    }

    private static void WriteObsoleteSection(
        StringBuilder sb,
        IReadOnlyList<DeclaredPlatform> obsolete)
    {
        sb.AppendLine("SAUCERY COVERAGE NO LONGER REQUIRED BY CURRENT SAUCE CATALOGUE");
        sb.AppendLine(new string('=', 92));
        sb.AppendLine("These are explicit Saucery.Core platform/version declarations that could not be matched to");
        sb.AppendLine("the current Sauce functional platform set. Treat them as removal candidates, not automatic");
        sb.AppendLine("deletions: a temporarily absent/private Sauce target or an extraction ambiguity should be reviewed.");
        sb.AppendLine();

        if (obsolete.Count == 0)
        {
            sb.AppendLine("None detected.");
            sb.AppendLine();
            return;
        }

        foreach (var platform in obsolete)
        {
            sb.AppendLine($"OBSOLETE? {platform.DisplayName}");
            sb.AppendLine($"          Family          : {platform.Family}");
            sb.AppendLine($"          Core declaration: {platform.File}");
            sb.AppendLine($"          Matched source  : {platform.Match}");
            sb.AppendLine("          Sauce status    : no matching current functional platform requirement");
            sb.AppendLine();
        }
    }

    private static void WriteFinding(StringBuilder sb, CoverageFinding finding)
    {
        sb.AppendLine($"[{finding.Requirement.Kind}] {finding.Requirement.DisplayName}");
        sb.AppendLine($"  Family                 : {finding.Requirement.Family}");
        sb.AppendLine($"  Sauce configurations   : {finding.Requirement.SauceConfigurationCount:N0}");
        sb.AppendLine($"  Core status            : {finding.CoreStatus}");
        sb.AppendLine($"  Core evidence          : {FormatEvidence(finding.CoreEvidence)}");
        sb.AppendLine($"  Representative testing : {finding.TestSampleStatus}");
        sb.AppendLine($"  Test evidence          : {FormatEvidence(finding.TestEvidence)}");

        if (finding.Requirement.RepresentativeDevices.Count > 0)
        {
            sb.AppendLine($"  Example Sauce devices  : {string.Join("; ", finding.Requirement.RepresentativeDevices.Take(3))}");
        }

        sb.AppendLine();
    }

    private static string FormatEvidence(Evidence evidence)
    {
        if (evidence.Strength == EvidenceStrength.None)
        {
            return "none";
        }

        return $"{evidence.Strength}; {evidence.File}; matched '{evidence.Match}'";
    }
}


internal static class ReadmeCoverageUpdater
{
    private const string BeginMarker = "<!-- BEGIN SAUCERY PLATFORM COVERAGE -->";
    private const string EndMarker = "<!-- END SAUCERY PLATFORM COVERAGE -->";
    private const string PreferredInsertionHeading = "### Packages";

    public static bool Update(
        string readmePath,
        IReadOnlyList<CoverageFinding> findings,
        PlatformLifecycleResult lifecycle)
    {
        if (!File.Exists(readmePath))
        {
            throw new FileNotFoundException("The repository root README.md could not be found.", readmePath);
        }

        var original = File.ReadAllText(readmePath);
        var newline = original.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var section = BuildSection(findings, lifecycle, newline);
        var updated = ReplaceOrInsertSection(original, section, newline);

        if (string.Equals(original, updated, StringComparison.Ordinal))
        {
            return false;
        }

        File.WriteAllText(readmePath, updated, new UTF8Encoding(false));
        return true;
    }

    private static string BuildSection(
        IReadOnlyList<CoverageFinding> findings,
        PlatformLifecycleResult lifecycle,
        string newline)
    {
        var missing = findings
            .Where(x => x.CoreStatus == CoverageStatus.Gap)
            .OrderBy(x => x.Requirement.Family)
            .ThenBy(x => x.Requirement.SortKey, NaturalStringComparer.Instance)
            .ToList();

        var obsolete = lifecycle.ObsoleteCandidates
            .OrderBy(x => x.Family)
            .ThenBy(x => x.Version, NaturalStringComparer.Instance)
            .ThenBy(x => x.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var lines = new List<string>
        {
            BeginMarker,
            "### Platform Coverage Audit",
            string.Empty,
            "> Automatically maintained by `Saucery.CoverageGapReporter`. Missing means Sauce currently advertises the functional platform but Saucery.Core coverage was not detected. Obsolete candidate means an explicit Saucery.Core declaration was not found in the current Sauce API-derived functional platform set and should be reviewed before removal.",
            string.Empty,
            "| Status | Platform | Evidence |",
            "| --- | --- | --- |"
        };

        foreach (var finding in missing)
        {
            var requirement = finding.Requirement;
            var evidence = requirement.SauceConfigurationCount == 1
                ? "1 Sauce catalogue configuration; no Saucery.Core coverage detected."
                : $"{requirement.SauceConfigurationCount:N0} Sauce catalogue configurations; no Saucery.Core coverage detected.";

            lines.Add($"| Missing | {EscapeCell(requirement.DisplayName)} | {EscapeCell(evidence)} |");
        }

        foreach (var platform in obsolete)
        {
            var evidence = $"Declared in `{platform.File}`; no matching current Sauce API-derived functional platform requirement.";
            lines.Add($"| Obsolete candidate | {EscapeCell(platform.DisplayName)} | {EscapeCell(evidence)} |");
        }

        if (missing.Count == 0 && obsolete.Count == 0)
        {
            lines.Add("| None | — | No missing or obsolete coverage detected. |");
        }

        lines.Add(EndMarker);
        return string.Join(newline, lines);
    }

    private static string ReplaceOrInsertSection(string original, string section, string newline)
    {
        var beginIndex = original.IndexOf(BeginMarker, StringComparison.Ordinal);
        var endIndex = original.IndexOf(EndMarker, StringComparison.Ordinal);

        if (beginIndex >= 0 || endIndex >= 0)
        {
            if (beginIndex < 0 || endIndex < 0 || endIndex < beginIndex)
            {
                throw new InvalidDataException(
                    $"README.md contains an incomplete or invalid generated coverage section. Expected both '{BeginMarker}' and '{EndMarker}'.");
            }

            var replaceEnd = endIndex + EndMarker.Length;
            return original[..beginIndex] + section + original[replaceEnd..];
        }

        var headingIndex = FindHeadingStart(original, PreferredInsertionHeading);
        if (headingIndex >= 0)
        {
            var prefix = original[..headingIndex].TrimEnd('\r', '\n');
            var suffix = original[headingIndex..].TrimStart('\r', '\n');
            return prefix + newline + newline + section + newline + newline + suffix;
        }

        return original.TrimEnd('\r', '\n') + newline + newline + section + newline;
    }

    private static int FindHeadingStart(string text, string heading)
    {
        var index = text.IndexOf(heading, StringComparison.Ordinal);
        while (index >= 0)
        {
            if (index == 0 || text[index - 1] == '\n')
            {
                return index;
            }

            index = text.IndexOf(heading, index + heading.Length, StringComparison.Ordinal);
        }

        return -1;
    }

    private static string EscapeCell(string value) =>
        value.Replace("|", "\\|", StringComparison.Ordinal)
             .Replace("\r", " ", StringComparison.Ordinal)
             .Replace("\n", " ", StringComparison.Ordinal);
}

internal sealed class NaturalStringComparer : IComparer<string>
{
    public static NaturalStringComparer Instance { get; } = new();

    public int Compare(string? x, string? y)
    {
        x ??= string.Empty;
        y ??= string.Empty;

        var xParts = Regex.Split(x, @"(\d+)");
        var yParts = Regex.Split(y, @"(\d+)");

        for (var i = 0; i < Math.Min(xParts.Length, yParts.Length); i++)
        {
            if (int.TryParse(xParts[i], out var xi) && int.TryParse(yParts[i], out var yi))
            {
                var numeric = xi.CompareTo(yi);
                if (numeric != 0)
                {
                    return numeric;
                }
            }
            else
            {
                var text = string.Compare(xParts[i], yParts[i], StringComparison.OrdinalIgnoreCase);
                if (text != 0)
                {
                    return text;
                }
            }
        }

        return xParts.Length.CompareTo(yParts.Length);
    }
}
