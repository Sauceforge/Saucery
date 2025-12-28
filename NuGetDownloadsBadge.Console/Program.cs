using System.Net.Http.Json;
using System.Text.Json;

var packages = new[]
{
    "Saucery2",
    "Saucery3",
    "Saucery.Core",
    "Saucery",
    "Saucery.XUnit",
    "Saucery.TUnit",
    "Saucery.XUnit.v3",
};

using var http = new HttpClient();
http.DefaultRequestHeaders.UserAgent.ParseAdd("nuget-downloads-badge-bot/1.0");

// Discover endpoints from service index (don’t hardcode search URL)
var index = await http.GetFromJsonAsync<JsonElement>("https://api.nuget.org/v3/index.json");
var resources = index.GetProperty("resources").EnumerateArray();

string? searchBaseUrl = null;
foreach(var r in resources) {
    var type = r.GetProperty("@type").GetString() ?? "";
    if(type.StartsWith("SearchQueryService", StringComparison.OrdinalIgnoreCase)) {
        searchBaseUrl = r.GetProperty("@id").GetString();
        break;
    }
}

if(string.IsNullOrWhiteSpace(searchBaseUrl))
    throw new Exception("Could not find SearchQueryService in NuGet service index.");

static async Task<long> GetTotalDownloadsAsync(HttpClient http, string searchBaseUrl, string packageId) {
    // Use an exact-match style query; we also defensively pick the result whose id matches exactly.
    var url = $"{searchBaseUrl}?q=packageid:{Uri.EscapeDataString(packageId)}&take=20&prerelease=true&semVerLevel=2.0.0";
    var json = await http.GetFromJsonAsync<JsonElement>(url);

    foreach(var item in json.GetProperty("data").EnumerateArray()) {
        var id = item.GetProperty("id").GetString();
        if(string.Equals(id, packageId, StringComparison.OrdinalIgnoreCase)) {
            if(item.TryGetProperty("totalDownloads", out var td) && td.ValueKind == JsonValueKind.Number)
                return td.GetInt64();

            // Spec says this can be inferred by summing version downloads if needed. :contentReference[oaicite:2]{index=2}
            long sum = 0;
            if(item.TryGetProperty("versions", out var versions) && versions.ValueKind == JsonValueKind.Array) {
                foreach(var v in versions.EnumerateArray()) {
                    if(v.TryGetProperty("downloads", out var d) && d.ValueKind == JsonValueKind.Number)
                        sum += d.GetInt64();
                }
            }
            return sum;
        }
    }

    // Package might be unlisted (search may omit it). In that case you can fallback to scraping,
    // but for your listed packages this should work.
    return 0;
}

long total = 0;
foreach(var p in packages) {
    total += await GetTotalDownloadsAsync(http, searchBaseUrl, p);
}

Directory.CreateDirectory("badges");

var badgeJson = new {
    schemaVersion = 1,
    label = "Total All Time Saucery downloads",
    message = total.ToString("N0"),
    color = "brightgreen",
};

await File.WriteAllTextAsync(
    "badges/nuget-total-downloads.json",
    JsonSerializer.Serialize(badgeJson)
);

Console.WriteLine($"Wrote badges/nuget-total-downloads.json (total={total:N0})");
