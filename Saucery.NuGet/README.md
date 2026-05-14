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

---

## Features

- Scans a solution for projects that opt-in to updates
- For each opted-in project, inspects PackageReference entries and determines the next available NuGet version
- Optionally includes prerelease versions when resolving the next version
- Optionally increments the project's own `<PackageVersion>` when updates are applied
- Supports dry-run mode to preview changes without modifying files
- Optionally scans `csproj` files on disk that are not registered in the solution
- Optionally excludes specific projects from being updated
- Optionally excludes specific packages from processing via CLI flag, a solution-level config file, or a per-project declaration

---

## Requirements

- .NET 10 SDK
- Network access to the NuGet.org v3 API (https://api.nuget.org/v3/index.json)

---

## Opting a project in

Only projects that explicitly opt in will be scanned and updated.

To opt a project in, add the following property to your `.csproj` file:

```xml
<PropertyGroup>
  <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
</PropertyGroup>
```

### Important

- Opt-in is **property-based**, not package-based
- You do **not** add a `PackageReference` to `Saucery.NuGet`
- `Saucery.NuGet` is a **tool**, not a library dependency

---

## Usage

### Preview changes without writing (dry run)

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

---

## Bump the project’s own `<PackageVersion>`

When any dependency is updated, automatically increment the project's own `<PackageVersion>` (patch by default):

```bash
saucery-nuget --solution MySolution.sln --bump-own-version
```

Choose which semver segment to increment:

```bash
saucery-nuget --solution MySolution.sln --bump-own-version --version-segment minor
saucery-nuget --solution MySolution.sln --bump-own-version --version-segment major
```

If no dependencies changed, `<PackageVersion>` is left unchanged.

---

## Process specific opted-in projects

When multiple projects are opted in, limit processing with `--project` (alias `-p`).

You can pass:

- project name (no extension)
- project filename (`.csproj`)
- absolute path

Examples:

```bash
saucery-nuget --solution MySolution.sln --project Saucery.Core
saucery-nuget --solution MySolution.sln --project Saucery.Core.csproj
saucery-nuget --solution MySolution.sln --project C:\repos\Saucery\Saucery.Core\Saucery.Core.csproj
```

If no opted-in project matches, the tool exits with a non-zero code.

---

## Sync PackageVersion with a dependency

Keep an opted-in project's `<PackageVersion>` in sync with a dependency using `--sync-with` (alias `-w`).

Supports:

- `PackageReference` in the same project
- `ProjectReference` to another project

### Behavior

- Uses updated dependency version if changed in the same run
- Falls back to existing dependency version if unchanged
- If dependency not found → no sync occurs
- Takes precedence over `--bump-own-version`
- In `--dry-run`, reports changes but does not write files

### Requirement

`--sync-with` must be used with `--project`.

### Examples

```bash
saucery-nuget --solution MySolution.sln --project Saucery.TUnit --sync-with TUnit
saucery-nuget --solution MySolution.sln --project Saucery --sync-with Saucery.Core
saucery-nuget --solution MySolution.sln --project Saucery.TUnit --sync-with TUnit --dry-run
```

---

## Scan unregistered projects

Include `.csproj` files found on disk that are not listed as `Project(...)` entries in the solution file:

```bash
saucery-nuget --solution MySolution.sln --scan-unregistered
```

Useful when projects exist in the repository but have not been added to the solution.

---

## Exclude projects from processing

Skip one or more projects entirely using `--exclude-projects`:

```bash
saucery-nuget --solution MySolution.sln --exclude-projects Saucery.Core
saucery-nuget --solution MySolution.sln --exclude-projects Saucery.Core Saucery.XUnit
```

You can pass:

- project name (no extension)
- project filename (`.csproj`)
- absolute path

The listed projects will be removed from the opted-in set before any processing begins.

---

## Exclude packages from updates

There are three ways to exclude specific packages, and they all work together. The effective exclusion list for each project is the union of all three sources (case-insensitve, deduplicated).

### 1 - CLI flag (runtime, one-off)

```bash
saucery-nuget --solution MySolution.sln --exclude-packages Selenium.WebDriver Selenium.Support
```

### 2 - Global config file (persistent, applies to every project)

Create a `saucery-nuget.json` file in the same directory as your `.sln`:

```json
{
  "excludePackages": [
    "Selenium.WebDriver",
    "Selenium.Support"
  ]
}
```

- The file is read automatically on every run - no CLI flag needed.
- If the file does not exist, the tool continues normally.
- If the file contains invalid JSON, a warning is printed and global exclusions are skipped.

### 3 - Per-project declaration (persistent, single project only)

Add one `<SauceryNuGetExclude>` element per package to be exclude to the `.csproj`:

```xml
<PropertyGroup>
  <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
  <SauceryNuGetExclude>Selenium.WebDriver</SauceryNuGetExclude>
  <SauceryNuGetExclude>Selenium.Support</SauceryNuGetExclude>
</PropertyGroup>
```

- Only effect the project that declares the element.
- Case-insensitive, multiple elements are supported.
- Can also be placed in an `<ItemGroup>` - any location in the `.csproj` is fine.

---

## All options

| Option | Alias | Description |
|---|---|---|
| `--solution <path>` | `-s` | Path to the solution (.sln) file to process. Required. |
| `--dry-run` |  | Print proposed changes without writing files. |
| `--include-prerelease` |  | Consider prerelease versions as candidates. |
| `--bump-own-version` |  | Increment `<PackageVersion>` when dependencies change. |
| `--version-segment <seg>` |  | `patch` (default), `minor`, or `major` |
| `--project <name\|path>` | `-p` | Limit processing to opted-in projects. |
| `--sync-with <packageId>` | `-w` | Sync `<PackageVersion>` with dependency. |
| `--scan-unregistered` |  | Include `.csproj` files not registered in the solution. |
| `--exclude-packages <ids>` |  | Exclude one or more packages from updates (CLI; merges with config file and per-project declarations). |
| `--exclude-projects <names>` |  | Exclude one or more projects from processing. |

---

## What "next version" means

Given:

```
Current:   1.2.0
Available: 1.2.0, 1.3.0, 1.4.0, 2.0.0
```

The tool selects:

```
1.3.0
```

It always picks the **smallest version strictly greater than current**.

---

## CI / Pipeline integration

Typical usage:

```yaml
- name: Install Saucery.NuGet tool
  run: dotnet tool install --global Saucery.NuGet --version 1.0.1

- name: Add .NET tools to PATH
  run: echo "$HOME/.dotnet/tools" >> $GITHUB_PATH

- name: Run Saucery.NuGet
  run: saucery-nuget --solution MySolution.sln
```

See our [dogfooding pipeline](https://github.com/Sauceforge/Saucery/blob/master/.github/workflows/saucery-nuget-sync-and-commit.yml) for a real-world example.

---

## How it works

1. Parses the `.sln` file to discover all `.csproj` files
2. Optionally adds `.csproj` files found on disk that are not registered in the solution (`--scan-unregistered`)
3. Filters to projects with `<SauceryNuGetOptIn>true</SauceryNuGetOptIn>`
4. Removes any projects listed in `--exclude-projects`
5. Reads `saucery.nuget.json` from the solution directory and merges its `excludePackages` with any `--exclude-packages` CLI values
6. For each project, reads any `<SauceryNuGetExclude>` elements from the `.csproj` and merges them with the global exclusion list
7. Finds all `PackageReference` entries, skipping any in the effective exclusion list
8. Resolves the next version using `NuGet.Versioning`
9. Updates the `.csproj` file (preserving encoding and BOM)
10. Optionally updates `<PackageVersion>`

---

## Contributing

- Fork the project, create a branch, and submit a PR
- Follow existing coding and testing patterns

---

## License

Copyright (c) 2024 Andrew Gray. All rights reserved.