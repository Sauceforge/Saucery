using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Saucery.NuGet.Core;

public sealed class NuGetApiClient : INuGetApiClient, IDisposable {
    private readonly HttpClient _http;
    private string? _flatContainerBase;

    public NuGetApiClient() {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.NuGetApi.UserAgent);
    }

    public Task<IReadOnlyList<string>> GetAvailableVersionsAsync(
        string packageId) {
        return GetAvailableVersionsAsync(
            packageId,
            CancellationToken.None);
    }

    public async Task<IReadOnlyList<string>> GetAvailableVersionsAsync(
        string packageId,
        CancellationToken ct) {
        var baseUrl = await ResolveFlatContainerBaseAsync(ct)
            .ConfigureAwait(false);
        var url = $"{baseUrl.TrimEnd('/')}/{packageId.ToLowerInvariant()}/index.json";

        try {
            var response = await _http
                .GetFromJsonAsync<VersionIndex>(url, ct)
                .ConfigureAwait(false);

            return response?.Versions ?? [];
        } catch(HttpRequestException ex)
              when(ex.StatusCode == HttpStatusCode.NotFound) {
            return [];
        }
    }

    public void Dispose() {
        _http.Dispose();
    }

    private async Task<string> ResolveFlatContainerBaseAsync(
        CancellationToken ct) {
        if(_flatContainerBase is not null) {
            return _flatContainerBase;
        }

        var index = await _http
            .GetFromJsonAsync<JsonElement>(
                Constants.NuGetApi.ServiceIndexUrl,
                ct)
            .ConfigureAwait(false);

        foreach(var resource in index
                     .GetProperty(Constants.NuGetApi.ResourcesProperty)
                     .EnumerateArray()) {
            var type = resource
                           .GetProperty(Constants.NuGetApi.TypeProperty)
                           .GetString()
                       ?? string.Empty;

            if(!type.Equals(
                    Constants.NuGetApi.FlatContainerResourceType,
                    StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            _flatContainerBase = resource
                                     .GetProperty(Constants.NuGetApi.IdProperty)
                                     .GetString()
                                 ?? throw new InvalidOperationException(
                                     "PackageBaseAddress resource has no @id.");

            return _flatContainerBase;
        }

        throw new InvalidOperationException(
            $"Could not find '{Constants.NuGetApi.FlatContainerResourceType}' " +
            "in NuGet service index.");
    }

    private sealed class VersionIndex {
        public string[] Versions { get; set; } = [];
    }
}