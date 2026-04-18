using System;
using System.Collections.Generic;
using System.Text;

namespace Saucery.NuGet;

internal static class Constants {
    internal static class NuGetApi {
        internal const string ServiceIndexUrl = "https://api.nuget.org/v3/index.json";
        internal const string FlatContainerResourceType = "PackageBaseAddress/3.0.0";
        internal const string ResourcesProperty = "resources";
        internal const string TypeProperty = "@type";
        internal const string IdProperty = "@id";
        internal const string UserAgent = "saucery-nuget/1.0";
    }

    internal static class Xml {
        internal const string PackageReferenceElement = "PackageReference";
        internal const string IncludeAttribute = "Include";
        internal const string VersionAttribute = "Version";
    }

    internal static class Package {
        internal const string OptInPackageId = "Saucery.NuGet";
    }

    internal static class Cli {
        internal const string SolutionOption = "--solution";
        internal const string SolutionAlias = "-s";
        internal const string IncludePrereleaseOption = "--include-prerelease";
        internal const string DryRunOption = "--dry-run";
        internal const string BumpOwnVersionOption = "--bump-own-version";
        internal const string VersionSegmentOption = "--version-segment";
    }
}
