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

### Process specific opted-in projects

When multiple projects in a solution are opted in, you can limit processing to one or more specific opted-in projects using `--project` (alias `-p`). You can pass the project by its file name (with or without `.csproj`) or by an absolute path to the `.csproj` file. The option may be provided multiple times to process multiple projects.

Examples:

- By project name (no extension):

  ```bash
  saucery-nuget --solution MySolution.sln --project Saucery.Core
  ```

- By project filename:

  ```bash
  saucery-nuget --solution MySolution.sln --project Saucery.Core.csproj
  ```

- By absolute path:

  ```bash
  saucery-nuget --solution MySolution.sln --project C:\repos\Saucery\Saucery.Core\Saucery.Core.csproj
  ```

If no opted-in project matches the supplied filter(s), the tool exits with a non-zero code and prints an error detailing the filters that failed to match.

### Sync PackageVersion with a dependency

You can keep an opted-in project's `<PackageVersion>` in sync with the version of a specific dependency using `--sync-with` (alias `-w`). The sync target can be either:

- a PackageReference within the same project (e.g. `--sync-with TUnit` will sync to the `TUnit` PackageReference version), or
- a ProjectReference to another project in the solution (e.g. `--sync-with Saucery.Core` will sync to the `Saucery.Core.csproj` <PackageVersion>).

Behavior:

- The tool first looks for a PackageReference that matches the supplied id and, if present, uses its version (preferring any proposed update from the run).
- If no matching PackageReference exists, the tool will examine ProjectReference entries and resolve the referenced project file. If the referenced project contains a `<PackageVersion>` element, that value is used for the sync. If the referenced project was processed earlier in the same run and had its `<PackageVersion>` updated, the new value will be read from disk and used.
- If the dependency (package or project) is not present or has no `<PackageVersion>`, no sync occurs for that target project.
- Sync takes precedence over `--bump-own-version`. If `--sync-with` produces a new `<PackageVersion>`, `--bump-own-version` is ignored for that project.
- When `--dry-run` is used the tool reports the new package version but does not write changes to disk.

Note: `--sync-with` is only valid when used together with one or more `--project` filters. The tool requires you to explicitly target the project(s) whose `<PackageVersion>` you want to sync. If `--sync-with` is provided without any `--project` values, the tool will exit with an error.

Examples:

- Sync the PackageVersion with package `TUnit` for the specified project(s):

  ```bash
  saucery-nuget --solution MySolution.sln --project Saucery.TUnit --sync-with TUnit
  ```

- Sync Saucery's PackageVersion to match the PackageVersion defined in Saucery.Core (ProjectReference):

  ```bash
  saucery-nuget --solution MySolution.sln --project Saucery --sync-with Saucery.Core
  ```

- Sync only for Saucery.TUnit and perform a dry run:

  ```bash
  saucery-nuget --solution MySolution.sln --project Saucery.TUnit --sync-with TUnit --dry-run
  ```

### All options

| Option | Alias | Description |
|---|---|---|
| `--solution <path>` | -s | Path to the solution (.sln) file to process. Required. |
| `--dry-run` |  | Print proposed changes without writing files. |
| `--include-prerelease` |  | Consider prerelease versions as candidates. |
| `--bump-own-version` |  | Increment `<PackageVersion>` when dependencies change. |
| `--version-segment <seg>` |  | Segment to bump: `patch` (default), `minor`, or `major` |
| `--project <name|path>` | -p | Limit processing to one or more opted-in projects by project name, filename, or absolute path. May be provided multiple times. |
| `--sync-with <packageId>` | -w | Keep each processed project's `<PackageVersion>` equal to the specified dependency package id's version (if present). |

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
| `project` | (none) | Limit processing to one or more opted-in projects (name, filename, or absolute path). May be specified multiple times. |
| `sync-with` | (none) | Keep each processed project's `<PackageVersion>` equal to the specified dependency package id's version. Requires one or more `project` inputs. |

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
