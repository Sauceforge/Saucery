# Saucery.NuGet

> A .NET global tool that bumps every PackageReference in opted-in `.csproj` files to the next available version on NuGet.org.

## Installation

```bash
dotnet tool install --global Saucery.NuGet
```

Or pin it to a repository using a tool manifest (recommended for CI):

```bash
dotnet new tool-manifest # once per repo, creates .config/dotnet-tools.json
dotnet tool install Saucery.NuGet
```

## Features

- Scans a solution for projects that opt-in to updates
- For each opted-in project, inspects PackageReference entries and determines the next available NuGet version
- Optionally includes prerelease versions when resolving the next version
- Optionally increments the project's own <PackageVersion> when updates are applied
- Supports dry-run mode to preview changes without modifying files

## Requirements

- .NET 10 SDK
- Network access to the NuGet.org v3 API (https://api.nuget.org/v3/index.json)

## Opting a project in

Only projects that explicitly opt in will be scanned and updated. To opt a project in, add a PackageReference with the opt-in package id to the project file (.csproj). For example:

```xml
<ItemGroup>
  <PackageReference Include="Saucery.NuGet" Version="1.0.0" />
  <PackageReference Include="Newtonsoft.Json" Version="12.0.0" />
  <PackageReference Include="Serilog" Version="2.10.0" />
</ItemGroup>
```

The tool scans the solution, finds every opted-in project, and bumps all **other** `PackageReference` entries. The `Saucery.NuGet` marker itself is never modified.

## Usage

### Prevew changes without writing (dry run)
```bash
saucery-nuget --solution MySolution.sln --dry-run
```

### Apply the next-version bumps
```bash
saucery-nuget --solution MySolution.sln
```

### Include prerelease versions
```bash
saucery-nuget --solution MySolution.sln --include-prerelease
```

### Also bump the projects own `<PackageVersion>`

When any depencency is updated, automatically increment the project's own `<PackageVersion>` (patch by default):
```bash
saucery-nuget --solution MySolution.sln --bump-own-version
```

Choose which semver segment to increment (patch, minor, or major):
```bash
saucery-nuget --solution MySolution.sln --bump-own-version --version-segment minor
saucery-nuget --solution MySolution.sln --bump-own-version --version-segment major
```

If no dependencies changed for the project, its `<PackageVersion>` is left untouched.

### All options

| Option | Alias | Description |
|---|---|---|
| `--solution <path>` | -s | Path to the solution (.sln) file to process. Required. |
| `--dry-run` |  | Print proposed changes without writing files. |
| `--include-prerelease` |  | Consider prerelease versions as candidates. |
| `--bump-own-version` |  | Increment `<PackageVersion>` when dependencies change. |
| `--version-segment <seg>` |  | Segment to bump: `patch` (default), `minor`, or `major` |

---

## What "next version" means

Given a current pin of `1.2.0` and available versions `1.2.0`, `1.3.0`, `1.4.0`, `2.0.0`, the tools selects **`1.3.0`** - the smallest version strictly greater than the current pin. Running it again would select **`1.4.0`**, and so on. This gives you controlled, incremental upgrades rather than a single jump to latest.

Prerelease versions (e.g. `1.3.0-beta.1`) are excluded by defauly and only considered when `--include-prerelease` is passed.

## CI / Pipeline integration

A ready-to-use GitHub Actions workflow is available in `.github/workflows/nuget-next-pipeline.yml`. It:

1. **Builds** the solution and uploads compiled artefacts
2. **Tests** by running `Saucery.NuGet.Tests` against the artefacts
3. **Bumps** PackageReferences in all opted-in projects via `saucery-nuget`
4. **Commits* the updated `.csproj` files back to the branch
5. **Cleans up** the build artefact from the run

Trigger it manually from the GitHub Actions UI. Available inputs:

| Input | Default | Description |
|---|---|---|
| `include-prerelease` | `false` | Include prerelease versions |
| `bump-own-version` | `false` | Bump each project's `<PackageVersion>` when it's dependencies change |
| `version-segment` | `patch` | Semver segment to increment (`patch`, `minor`, `major`) |
| `dry-run` | `false` | Print changes without commiting |

### Using a tool manifest in CI

```yaml
 - name: Restore .NET tools
  run: dotnet tool restore

 - name: Bump PackageReferences
  run: dotnet tool run saucery-nuget --solution MySolution.sln
```

## How it works

1. Parses the `.sln` file to discover all referenced csproj files.
2. Filters to projects containing a `PackageReference` to the opt-in package (e.g. `Saucery.NuGet`).
3. For each opted-in project, parses the `.csproj` XML to find all `PackageReference` entries (except the opt-in marker).
4. Resolves the next version using `NuGet.Versioning`
5. Rewrites the `.csproj` in-place, preserving the original file encoding and BOM.
6. if `--bump-own-version` is enabled, and at least one dependency changed, increments the project's `<PackageVersion>` by the requested segment (`patch` / `minor` / `major`), resetting lower order segments to zero.

## Contributing

- Fork the project, create a branch, implement your change, and submit a PR.
- Follow the repository's coding and testing patterns. Unit tests live in the corresponding *.Tests projects.

## License

Copyright (c) 2024 Andrew Gray. All rights reserved.
