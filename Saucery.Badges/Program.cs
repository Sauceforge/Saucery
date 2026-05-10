using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Saucery.Badges;

public static class Program {
    public static async Task Main() {
        var packages = new[]
        {
            "Saucery2",
            "Saucery3",
            "Saucery.Core",
            "Saucery",
            "Saucery.XUnit",
            "Saucery.TUnit",
            "Saucery.XUnit.v3",
            "Saucery.NuGet"
        };

        using var http = new HttpClient();

        http.DefaultRequestHeaders.UserAgent.ParseAdd("nuget-downloads-badge-bot/1.0");

        var index = await http.GetFromJsonAsync<JsonElement>(
            "https://api.nuget.org/v3/index.json");

        var resources = index.GetProperty("resources").EnumerateArray();

        string? searchBaseUrl = null;

        foreach(var r in resources) {
            var type = r.GetProperty("@type").GetString() ?? "";

            if(type.StartsWith("SearchQueryService", StringComparison.OrdinalIgnoreCase)) {
                searchBaseUrl = r.GetProperty("@id").GetString();
                break;
            }
        }

        if(string.IsNullOrWhiteSpace(searchBaseUrl)) {
            throw new Exception("Could not find SearchQueryService in NuGet service index.");
        }

        long total = 0;

        Console.WriteLine("NuGet package download totals:");
        Console.WriteLine();

        foreach(var p in packages) {
            var packageTotal = await GetTotalDownloadsAsync(http, searchBaseUrl, p);
            total += packageTotal;

            Console.WriteLine(
                $"{p}: {packageTotal.ToString("N0", CultureInfo.InvariantCulture)} " +
                $"({BadgeDownloadFormatter.FormatDownloadTotal(packageTotal)})");
        }

        Directory.CreateDirectory("badges");

        var formattedTotal = BadgeDownloadFormatter.FormatDownloadTotal(total);

        var badgeJson = new {
            schemaVersion = 1,
            label = "downloads",
            message = formattedTotal,
            color = "brightgreen",
        };

        await File.WriteAllTextAsync(
            "badges/nuget-total-downloads.json",
            JsonSerializer.Serialize(badgeJson)
        );

        var packageCountBadgeJson = new {
            schemaVersion = 1,
            label = "Saucery packages",
            message = packages.Length.ToString(CultureInfo.InvariantCulture),
            color = "blue",
        };

        await File.WriteAllTextAsync(
            "badges/nuget-package-count.json",
            JsonSerializer.Serialize(packageCountBadgeJson)
        );

        Console.WriteLine();
        Console.WriteLine(
            $"Total: {total.ToString("N0", CultureInfo.InvariantCulture)} ({formattedTotal})");

        Console.WriteLine(
            $"Wrote badges/nuget-total-downloads.json (total={total.ToString("N0", CultureInfo.InvariantCulture)}, badge={formattedTotal})");

        Console.WriteLine(
            $"Wrote badges/nuget-package-count.json (count={packages.Length})");
    }

    private static async Task<long> GetTotalDownloadsAsync(
        HttpClient http,
        string searchBaseUrl,
        string packageId) {
        var url =
            $"{searchBaseUrl}?q=packageid:{Uri.EscapeDataString(packageId)}&take=20&prerelease=true&semVerLevel=2.0.0";

        var json = await http.GetFromJsonAsync<JsonElement>(url);

        foreach(var item in json.GetProperty("data").EnumerateArray()) {
            var id = item.GetProperty("id").GetString();

            if(string.Equals(id, packageId, StringComparison.OrdinalIgnoreCase)) {
                if(item.TryGetProperty("totalDownloads", out var td)
                    && td.ValueKind == JsonValueKind.Number) {
                    return td.GetInt64();
                }

                long sum = 0;

                if(item.TryGetProperty("versions", out var versions)
                    && versions.ValueKind == JsonValueKind.Array) {
                    foreach(var v in versions.EnumerateArray()) {
                        if(v.TryGetProperty("downloads", out var d)
                            && d.ValueKind == JsonValueKind.Number) {
                            sum += d.GetInt64();
                        }
                    }
                }

                return sum;
            }
        }

        return 0;
    }
}