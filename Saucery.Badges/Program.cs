using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Saucery.Badges;

public static class Program {
    private const string StateFile = "badges/nuget-download-state.json";

    public static async Task Main() {
        using var http = new HttpClient {
            Timeout = TimeSpan.FromSeconds(30)
        };

        http.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("Saucery-NuGet-Downloads-Badge/1.0");

        var previousState = await LoadPreviousStateAsync();

        var searchBaseUrls = await GetSearchEndpointsAsync(http);

        Console.WriteLine("NuGet SearchQueryService endpoints:");
        foreach(var endpoint in searchBaseUrls) {
            Console.WriteLine($"  {endpoint}");
        }

        Console.WriteLine();

        var newState = new Dictionary<string, long>(
            StringComparer.OrdinalIgnoreCase);

        long total = 0;

        foreach(var package in Packages.Values) {
            Console.WriteLine($"Package: {package}");

            var candidates = new List<DownloadCandidate>();

            foreach(var endpoint in searchBaseUrls) {
                try {
                    var downloads = await GetSearchApiDownloadsAsync(
                        http,
                        endpoint,
                        package);

                    candidates.Add(new DownloadCandidate(
                        $"SearchQueryService: {endpoint}",
                        downloads));

                    Console.WriteLine(
                        $"  Search API: {downloads.ToString("N0", CultureInfo.InvariantCulture)}");
                } catch(Exception ex) {
                    Console.WriteLine(
                        $"  WARNING Search API failed: {endpoint}");
                    Console.WriteLine(
                        $"    {ex.Message}");
                }
            }

            try {
                var galleryDownloads =
                    await GetGalleryDownloadsAsync(http, package);

                candidates.Add(new DownloadCandidate(
                    "nuget.org package page",
                    galleryDownloads));

                Console.WriteLine(
                    $"  Gallery page: {galleryDownloads.ToString("N0", CultureInfo.InvariantCulture)}");
            } catch(Exception ex) {
                Console.WriteLine(
                    $"  WARNING Gallery page failed: {ex.Message}");
            }

            if(previousState.TryGetValue(
                   package,
                   out var previousDownloads)) {
                candidates.Add(new DownloadCandidate(
                    "previous committed state",
                    previousDownloads));

                Console.WriteLine(
                    $"  Previous state: {previousDownloads.ToString("N0", CultureInfo.InvariantCulture)}");
            }

            if(candidates.Count == 0) {
                throw new Exception(
                    $"Could not obtain any download count for package '{package}'.");
            }

            var selected = candidates
                .OrderByDescending(c => c.Downloads)
                .First();

            if(candidates
               .Select(c => c.Downloads)
               .Distinct()
               .Count() > 1) {
                Console.WriteLine("  WARNING: NuGet sources disagree:");

                foreach(var candidate in candidates
                            .OrderByDescending(c => c.Downloads)) {
                    Console.WriteLine(
                        $"    {candidate.Source}: " +
                        $"{candidate.Downloads.ToString("N0", CultureInfo.InvariantCulture)}");
                }
            }

            Console.WriteLine(
                $"  SELECTED: " +
                $"{selected.Downloads.ToString("N0", CultureInfo.InvariantCulture)} " +
                $"from {selected.Source}");

            Console.WriteLine();

            newState[package] = selected.Downloads;
            total += selected.Downloads;
        }

        Directory.CreateDirectory("badges");

        await SaveStateAsync(newState);

        var formattedTotal =
            BadgeDownloadFormatter.FormatDownloadTotal(total);

        var badgeJson = new {
            schemaVersion = 1,
            label = "downloads",
            message = formattedTotal,
            color = "brightgreen"
        };

        await File.WriteAllTextAsync(
            "badges/nuget-total-downloads.json",
            JsonSerializer.Serialize(badgeJson));

        var packageCountBadgeJson = new {
            schemaVersion = 1,
            label = "Saucery packages",
            message = Packages.Values.Length.ToString(
                CultureInfo.InvariantCulture),
            color = "blue"
        };

        await File.WriteAllTextAsync(
            "badges/nuget-package-count.json",
            JsonSerializer.Serialize(packageCountBadgeJson));

        Console.WriteLine(
            $"TOTAL: {total.ToString("N0", CultureInfo.InvariantCulture)}");

        Console.WriteLine(
            $"BADGE: {formattedTotal}");
    }

    private static async Task<string[]> GetSearchEndpointsAsync(
        HttpClient http) {
        var index =
            await http.GetFromJsonAsync<JsonElement>(
                "https://api.nuget.org/v3/index.json");

        return [.. index
            .GetProperty("resources")
            .EnumerateArray()
            .Where(r => {
                var type =
                    r.GetProperty("@type").GetString() ?? "";

                return type.StartsWith(
                    "SearchQueryService",
                    StringComparison.OrdinalIgnoreCase);
            })
            .Select(r =>
                r.GetProperty("@id").GetString())
            .Where(url =>
                !string.IsNullOrWhiteSpace(url))
            .Select(url => url!)
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    private static async Task<long> GetSearchApiDownloadsAsync(
        HttpClient http,
        string searchBaseUrl,
        string packageId) {
        var separator =
            searchBaseUrl.Contains('?')
                ? "&"
                : "?";

        var url =
            $"{searchBaseUrl}" +
            $"{separator}" +
            $"q=packageid:{Uri.EscapeDataString(packageId)}" +
            $"&take=20" +
            $"&prerelease=true" +
            $"&semVerLevel=2.0.0" +
            $"&_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.CacheControl =
            new System.Net.Http.Headers.CacheControlHeaderValue {
                NoCache = true,
                NoStore = true
            };

        using var response =
            await http.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content
                .ReadFromJsonAsync<JsonElement>();

        foreach(var item in json
                    .GetProperty("data")
                    .EnumerateArray()) {
            var id =
                item.GetProperty("id").GetString();

            if(!string.Equals(
                   id,
                   packageId,
                   StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            if(item.TryGetProperty(
                   "totalDownloads",
                   out var totalDownloads) &&
               totalDownloads.ValueKind ==
               JsonValueKind.Number) {
                return totalDownloads.GetInt64();
            }

            if(item.TryGetProperty(
                   "versions",
                   out var versions) &&
               versions.ValueKind ==
               JsonValueKind.Array) {
                long sum = 0;

                foreach(var version in versions
                            .EnumerateArray()) {
                    if(version.TryGetProperty(
                           "downloads",
                           out var downloads) &&
                       downloads.ValueKind ==
                       JsonValueKind.Number) {
                        sum += downloads.GetInt64();
                    }
                }

                return sum;
            }
        }

        throw new Exception(
            $"SearchQueryService returned no package '{packageId}'.");
    }

    private static async Task<long> GetGalleryDownloadsAsync(
        HttpClient http,
        string packageId) {
        var url =
            $"https://www.nuget.org/packages/" +
            $"{Uri.EscapeDataString(packageId)}" +
            $"?nocache={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        using var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                url);

        request.Headers.CacheControl =
            new System.Net.Http.Headers.CacheControlHeaderValue {
                NoCache = true,
                NoStore = true
            };

        request.Headers.Pragma.ParseAdd("no-cache");

        using var response =
            await http.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var html =
            await response.Content.ReadAsStringAsync();

        /*
         * NuGet's package page contains a Downloads section
         * with the total package download count.
         *
         * We deliberately support several representations so
         * this survives minor HTML formatting changes.
         */

        var patterns = new[] {
            @"(?is)Total\s*</[^>]+>\s*<[^>]+>\s*([\d,]+)",
            @"(?is)Total\s+([\d,]+)",
            @"(?is)([\d,]+)\s+total\s+downloads",
            @"""totalDownloads""\s*:\s*(\d+)"
        };

        foreach(var pattern in patterns) {
            var matches =
                Regex.Matches(
                    html,
                    pattern,
                    RegexOptions.IgnoreCase |
                    RegexOptions.CultureInvariant);

            foreach(Match match in matches) {
                var raw =
                    WebUtility.HtmlDecode(
                        match.Groups[1].Value);

                raw = raw.Replace(
                    ",",
                    "",
                    StringComparison.Ordinal);

                if(long.TryParse(
                       raw,
                       NumberStyles.Integer,
                       CultureInfo.InvariantCulture,
                       out var value) &&
                   value >= 0) {
                    return value;
                }
            }
        }

        throw new Exception(
            $"Could not locate total download count on NuGet gallery page for '{packageId}'.");
    }

    private static async Task<
        Dictionary<string, long>>
        LoadPreviousStateAsync() {
        if(!File.Exists(StateFile)) {
            return new Dictionary<string, long>(
                StringComparer.OrdinalIgnoreCase);
        }

        try {
            var json =
                await File.ReadAllTextAsync(
                    StateFile);

            var state =
                JsonSerializer.Deserialize<
                    Dictionary<string, long>>(json);

            return state ??
                   new Dictionary<string, long>(
                       StringComparer.OrdinalIgnoreCase);
        } catch(Exception ex) {
            Console.WriteLine(
                $"WARNING: Could not read previous state: {ex.Message}");

            return new Dictionary<string, long>(
                StringComparer.OrdinalIgnoreCase);
        }
    }

    private static async Task SaveStateAsync(
        Dictionary<string, long> state) {
        var ordered =
            state
                .OrderBy(
                    kvp => kvp.Key,
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value,
                    StringComparer.OrdinalIgnoreCase);

        JsonSerializerOptions options = new() { WriteIndented = true };
        var json = JsonSerializer.Serialize(ordered, options);

        await File.WriteAllTextAsync(
            StateFile,
            json);
    }

    private sealed record DownloadCandidate(
        string Source,
        long Downloads);
}