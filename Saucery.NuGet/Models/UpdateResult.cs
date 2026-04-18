namespace Saucery.NuGet.Models;

public sealed record UpdateResult(
    string ProjectPath,
    IReadOnlyList<PackageUpdate> Updates,
    string? Error = null,
    string? NewPackageVersion = null) {
    public bool Success => Error is null;
}
