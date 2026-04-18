namespace Saucery.NuGet.Core; 

public interface INuGetApiClient {
    Task<IReadOnlyList<string>> GetAvailableVersionsAsync(string packageId, CancellationToken ct = default);
}
