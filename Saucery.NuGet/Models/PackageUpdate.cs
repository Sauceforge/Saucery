namespace Saucery.NuGet.Models;

public sealed record PackageUpdate(
    string ProjectPath,
    string PackageId,
    string FromVersion,
    string ToVersion
);
