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
2. Filters to projects with `<SauceryNuGetOptIn>true</SauceryNuGetOptIn>`
3. Finds all `PackageReference` entries
4. Resolves the next version using `NuGet.Versioning`
5. Updates the `.csproj` file (preserving encoding and BOM)
6. Optionally updates `<PackageVersion>`

---

## Contributing

- Fork the project, create a branch, and submit a PR
- Follow existing coding and testing patterns

---

## License

Copyright (c) 2024 Andrew Gray. All rights reserved.