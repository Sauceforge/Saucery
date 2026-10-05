using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Saucery.Badges;

public static class Program {
    public static async Task Main() {
        using var http = new HttpClient();

        http.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("nuget-downloads-badge-bot/1.0");

        var index = await http.GetFromJsonAsync<JsonElement>("https://api.nuget.org/v3/index.json");

        var searchBaseUrls = index
            .GetProperty("resources")
            .EnumerateArray()
            .Where(r => {
                var type = r.GetProperty("@type").GetString() ?? "";

                return type.Equals("SearchQueryService", StringComparison.OrdinalIgnoreCase);
            })
            .Select(r => r.GetProperty("@id").GetString())
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => url!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if(searchBaseUrls.Length == 0) {
            throw new Exception("Could not find any SearchQueryService endpoints in NuGet service index.");
        }

        Console.WriteLine("NuGet SearchQueryService endpoints:");

        foreach(var searchBaseUrl in searchBaseUrls) {
            Console.WriteLine($"  {searchBaseUrl}");
        }

        Console.WriteLine();

        long total = 0;

        Console.WriteLine("NuGet package download totals:");
        Console.WriteLine();

        foreach(var package in Packages.Values) {
            var results = new List<(string Endpoint, long Downloads)>();

            foreach(var searchBaseUrl in searchBaseUrls) {
                try {
                    var downloads = await GetTotalDownloadsAsync(
                        http,
                        searchBaseUrl,
                        package);

                    results.Add((searchBaseUrl, downloads));
                } catch(Exception ex) {
                    Console.WriteLine(
                        $"WARNING: Failed to query {searchBaseUrl} for {package}: {ex.Message}");
                }
            }

            if(results.Count == 0) {
                throw new Exception($"All NuGet SearchQueryService endpoints failed for package {package}.");
            }

            var (Endpoint, Downloads) = results.MaxBy(r => r.Downloads);

            total += Downloads;

            Console.WriteLine(
                $"{package}: {Downloads.ToString("N0", CultureInfo.InvariantCulture)} " +
                $"({BadgeDownloadFormatter.FormatDownloadTotal(Downloads)})");

            foreach(var result in results) {
                Console.WriteLine(
                    $"  {result.Endpoint}: " +
                    $"{result.Downloads.ToString("N0", CultureInfo.InvariantCulture)}");
            }

            Console.WriteLine($"  Selected: {Endpoint}");
            Console.WriteLine();
        }

        Directory.CreateDirectory("badges");

        var formattedTotal = BadgeDownloadFormatter.FormatDownloadTotal(total);

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
            message = Packages.Values.Length.ToString(CultureInfo.InvariantCulture),
            color = "blue"
        };

        await File.WriteAllTextAsync(
            "badges/nuget-package-count.json",
            JsonSerializer.Serialize(packageCountBadgeJson));

        Console.WriteLine(
            $"Total: {total.ToString("N0", CultureInfo.InvariantCulture)} " +
            $"({formattedTotal})");

        Console.WriteLine(
            $"Wrote badges/nuget-total-downloads.json " +
            $"(total={total.ToString("N0", CultureInfo.InvariantCulture)}, " +
            $"badge={formattedTotal})");

        Console.WriteLine(
            $"Wrote badges/nuget-package-count.json " +
            $"(count={Packages.Values.Length})");
    }

    private static async Task<long> GetTotalDownloadsAsync(
        HttpClient http,
        string searchBaseUrl,
        string packageId) {
        var url =
            $"{searchBaseUrl}" +
            $"?q=packageid:{Uri.EscapeDataString(packageId)}" +
            $"&take=20" +
            $"&prerelease=true" +
            $"&semVerLevel=2.0.0";

        var json = await http.GetFromJsonAsync<JsonElement>(url);

        foreach(var item in json.GetProperty("data").EnumerateArray()) {
            var id = item.GetProperty("id").GetString();

            if(!string.Equals(id, packageId, StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            if(item.TryGetProperty("totalDownloads", out var totalDownloads) &&
               totalDownloads.ValueKind == JsonValueKind.Number) {
                return totalDownloads.GetInt64();
            }

            long sum = 0;

            if(item.TryGetProperty("versions", out var versions) &&
               versions.ValueKind == JsonValueKind.Array) {
                foreach(var version in versions.EnumerateArray()) {
                    if(version.TryGetProperty("downloads", out var downloads) &&
                       downloads.ValueKind == JsonValueKind.Number) {
                        sum += downloads.GetInt64();
                    }
                }
            }

            return sum;
        }

        throw new Exception($"NuGet SearchQueryService returned no result for package '{packageId}'.");
    }
}