using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Saucery.Badges;

public static class Program {
    private const string ProfileName = "fullcircle";

    public static async Task Main() {
        using var http = new HttpClient {
            Timeout = TimeSpan.FromSeconds(30)
        };

        http.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("Saucery-NuGet-Downloads-Badge/1.0");

        http.DefaultRequestHeaders.CacheControl =
            new System.Net.Http.Headers.CacheControlHeaderValue {
                NoCache = true,
                NoStore = true
            };

        http.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");

        var packageDownloads =
            await GetProfilePackageDownloadsAsync(
                http,
                ProfileName);

        long total = 0;

        Console.WriteLine(
            $"NuGet profile: https://www.nuget.org/profiles/{ProfileName}");

        Console.WriteLine();
        Console.WriteLine("NuGet package download totals:");
        Console.WriteLine();

        foreach(var package in Packages.Values) {
            if(!packageDownloads.TryGetValue(
                   package,
                   out var downloads)) {
                throw new Exception(
                    $"Package '{package}' was not found on NuGet profile '{ProfileName}'.");
            }

            total += downloads;

            Console.WriteLine(
                $"{package}: " +
                $"{downloads.ToString("N0", CultureInfo.InvariantCulture)} " +
                $"({BadgeDownloadFormatter.FormatDownloadTotal(downloads)})");
        }

        Directory.CreateDirectory("badges");

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

        Console.WriteLine();

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

    private static async Task<Dictionary<string, long>>
        GetProfilePackageDownloadsAsync(
            HttpClient http,
            string profileName) {
        var url =
            $"https://www.nuget.org/profiles/" +
            $"{Uri.EscapeDataString(profileName)}" +
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

        var results =
            new Dictionary<string, long>(
                StringComparer.OrdinalIgnoreCase);

        foreach(var package in Packages.Values) {
            var downloads =
                ParsePackageDownloads(
                    html,
                    package);

            results[package] = downloads;
        }

        return results;
    }

    private static long ParsePackageDownloads(
        string html,
        string packageId) {
        /*
         * Each package on a NuGet profile page contains a link to:
         *
         *     /packages/{packageId}
         *
         * followed within the same package card by text such as:
         *
         *     297,526 total downloads
         *
         * We deliberately anchor the match to the exact package URL,
         * rather than simply searching for download numbers.
         */
        var encodedPackageId =
            Regex.Escape(packageId);

        var pattern =
            $"""
            href\s*=\s*
            ["']
            /packages/
            {encodedPackageId}
            /?
            ["']
            (?:
                (?!href\s*=\s*["']/packages/). 
            )*?
            (?<downloads>\d[\d,]*)
            \s+
            total\s+downloads
            """;

        var match =
            Regex.Match(
                html,
                pattern,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline |
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.CultureInvariant);

        if(!match.Success) {
            throw new Exception(
                $"Could not find the exact download total for package " +
                $"'{packageId}' on the NuGet profile page.");
        }

        var raw =
            WebUtility.HtmlDecode(
                match.Groups["downloads"].Value);

        raw = raw.Replace(
            ",",
            "",
            StringComparison.Ordinal);

        if(!long.TryParse(
               raw,
               NumberStyles.None,
               CultureInfo.InvariantCulture,
               out var downloads)) {
            throw new Exception(
                $"NuGet returned an invalid download count " +
                $"'{match.Groups["downloads"].Value}' " +
                $"for package '{packageId}'.");
        }

        return downloads;
    }
}