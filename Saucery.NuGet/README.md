# Saucery.NuGet

> A .NET global tool that bumps every `PackageReference` in opted-in `.csproj` files — and every `PackageVersion` entry in `Directory.Packages.props` files — to the next available version on NuGet.org.

Saucery.NuGet supports both traditional project-level NuGet package management and [.NET Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management).

## Installation

Install globally:

```bash
dotnet tool install --global Saucery.NuGet
```

Or pin it to a repository using a tool manifest, which is recommended for CI:

```bash
dotnet new tool-manifest
dotnet tool install Saucery.NuGet
```

---

## Features

- Scans a solution for projects that explicitly opt in to updates
- Inspects `PackageReference` entries in opted-in `.csproj` files
- Automatically discovers and processes `Directory.Packages.props` files under the solution root
- Updates `PackageVersion` entries used by .NET Central Package Management
- Selects the next available NuGet version rather than automatically jumping to the latest version
- Optionally includes prerelease versions
- Optionally increments a project's own `PackageVersion` when dependency updates are applied
- Supports synchronising a project's `PackageVersion` with a dependency
- Supports dry-run mode to preview changes without modifying files
- Optionally scans `.csproj` files on disk that are not registered in the solution
- Optionally excludes specific projects from processing
- Supports package exclusions through:
  - the command line
  - a solution-level configuration file
  - per-project declarations
  - per-`Directory.Packages.props` declarations

---

## Requirements

- .NET 10 SDK
- Network access to the NuGet.org v3 API at `https://api.nuget.org/v3/index.json`

---

# Project Package Management

## Opting a project in

Only projects that explicitly opt in are scanned and updated.

Add the following property to the `.csproj` file:

```xml
<PropertyGroup>
  <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
</PropertyGroup>
```

### Important

- Opt-in is property-based, not package-based.
- You do not add a `PackageReference` to `Saucery.NuGet`.
- `Saucery.NuGet` is a .NET tool, not a library dependency.

For example:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Shouldly" Version="4.2.1" />
  </ItemGroup>

</Project>
```

Saucery.NuGet will inspect the `PackageReference`, determine the next available version on NuGet.org and update the project if a newer version is available.

---

# Central Package Management

Saucery.NuGet supports NuGet's Central Package Management model using `Directory.Packages.props`.

If your repository contains one or more `Directory.Packages.props` files beneath the solution root, Saucery.NuGet automatically discovers and processes them.

For example:

```xml
<Project>

  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
    <PackageVersion Include="Shouldly" Version="4.2.1" />
    <PackageVersion Include="TUnit" Version="1.0.0" />
  </ItemGroup>

</Project>
```

Each `PackageVersion` entry is independently checked against NuGet.org.

Given:

```text
Current:   1.2.0
Available: 1.2.0, 1.3.0, 1.4.0, 2.0.0
```

Saucery.NuGet selects:

```text
1.3.0
```

It does not jump directly to `2.0.0`.

---

## Automatic discovery

`Directory.Packages.props` files do not require `SauceryNuGetOptIn`.

Their presence beneath the solution root is the opt-in mechanism.

Given a repository such as:

```text
MyRepository/
├── MySolution.sln
├── Directory.Packages.props
├── src/
│   ├── Application/
│   │   └── Application.csproj
│   └── Infrastructure/
│       ├── Directory.Packages.props
│       └── Infrastructure.csproj
└── tests/
    └── Application.Tests/
        └── Application.Tests.csproj
```

Running:

```bash
saucery-nuget --solution MySolution.sln
```

will discover and process both:

```text
MyRepository/Directory.Packages.props
```

and:

```text
MyRepository/src/Infrastructure/Directory.Packages.props
```

`bin` and `obj` directories are ignored during discovery.

---

## How Central Package Management differs from `.csproj` processing

| Behaviour | `.csproj` | `Directory.Packages.props` |
|---|---|---|
| Opt-in required | Yes — `SauceryNuGetOptIn` | No — file presence is automatic opt-in |
| Package element updated | `PackageReference` | `PackageVersion` |
| `--exclude-packages` | Supported | Supported |
| Per-file exclusions | Supported | Supported |
| `--bump-own-version` | Supported | Supported when central package updates affect opted-in projects |
| `--sync-with` | Resolves `PackageReference` or `ProjectReference` | Can resolve centrally managed external dependency versions |

---

## Central Package Management exclusions

Packages in a `Directory.Packages.props` file can be excluded using `SauceryNuGetExcludePackage`.

For example:

```xml
<Project>

  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Selenium.WebDriver" Version="4.30.0" />
    <PackageVersion Include="Selenium.Support" Version="4.30.0" />
    <PackageVersion Include="Shouldly" Version="4.2.1" />
  </ItemGroup>

  <ItemGroup>
    <SauceryNuGetExcludePackage Include="Selenium.WebDriver" />
    <SauceryNuGetExcludePackage Include="Selenium.Support" />
  </ItemGroup>

</Project>
```

In this example, Saucery.NuGet will ignore:

```text
Selenium.WebDriver
Selenium.Support
```

while continuing to process:

```text
Shouldly
```

The exclusion declarations can appear anywhere in the file.

---

# Usage

## Preview changes without writing

Always start with a dry run when introducing Saucery.NuGet to an existing repository:

```bash
saucery-nuget --solution MySolution.sln --dry-run
```

The proposed changes are printed without modifying any files.

---

## Apply dependency updates

```bash
saucery-nuget --solution MySolution.sln
```

This processes:

- opted-in `.csproj` files; and
- automatically discovered `Directory.Packages.props` files.

---

## Include prerelease versions

```bash
saucery-nuget \
  --solution MySolution.sln \
  --include-prerelease
```

Without `--include-prerelease`, prerelease packages are not considered when determining the next available version.

---

# Bump the project's own `PackageVersion`

When a dependency changes, Saucery.NuGet can automatically increment the project's own `PackageVersion`.

```bash
saucery-nuget \
  --solution MySolution.sln \
  --bump-own-version
```

The default increment is:

```text
patch
```

For example:

```text
1.4.2 → 1.4.3
```

To increment the minor version:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --bump-own-version \
  --version-segment minor
```

For example:

```text
1.4.2 → 1.5.0
```

To increment the major version:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --bump-own-version \
  --version-segment major
```

For example:

```text
1.4.2 → 2.0.0
```

If no relevant dependencies change, the project's own `PackageVersion` is left unchanged.

---

# Process specific opted-in projects

When multiple projects are opted in, processing can be limited using `--project`, or its alias `-p`.

You can specify:

- the project name without an extension;
- the `.csproj` filename; or
- the absolute project path.

Examples:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --project Saucery.Core
```

```bash
saucery-nuget \
  --solution MySolution.sln \
  --project Saucery.Core.csproj
```

```bash
saucery-nuget \
  --solution MySolution.sln \
  --project C:\repos\Saucery\Saucery.Core\Saucery.Core.csproj
```

If no opted-in project matches the supplied value, Saucery.NuGet exits with a non-zero exit code.

---

# Synchronise `PackageVersion` with a dependency

An opted-in project's `PackageVersion` can be synchronised with a dependency using `--sync-with`, or its alias `-w`.

```bash
saucery-nuget \
  --solution MySolution.sln \
  --project Saucery.TUnit \
  --sync-with TUnit
```

This is useful for integration packages whose own version should track the framework or dependency they integrate with.

For example:

```text
TUnit             1.8.0
Saucery.TUnit     1.8.0
```

`--sync-with` supports:

- a `PackageReference` in the selected project;
- a centrally managed `PackageVersion`;
- a `ProjectReference` to another project.

## Behaviour

Saucery.NuGet:

1. Uses the updated dependency version if the dependency changed during the current run.
2. Falls back to the dependency's existing version when no update occurred.
3. Performs no synchronisation if the requested dependency cannot be resolved.
4. Gives `--sync-with` precedence over `--bump-own-version`.
5. Reports the proposed synchronisation without writing files when `--dry-run` is used.

`--sync-with` must be used with `--project`.

### Examples

Synchronise with a NuGet dependency:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --project Saucery.TUnit \
  --sync-with TUnit
```

Synchronise with another project:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --project Saucery \
  --sync-with Saucery.Core
```

Preview the change:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --project Saucery.TUnit \
  --sync-with TUnit \
  --dry-run
```

---

# Scan unregistered projects

By default, project discovery is based on projects registered in the solution.

To include `.csproj` files that exist beneath the solution root but are not registered in the solution:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --scan-unregistered
```

Opt-in rules still apply.

An unregistered project is processed only when it contains:

```xml
<SauceryNuGetOptIn>true</SauceryNuGetOptIn>
```

This option affects `.csproj` discovery.

`Directory.Packages.props` files are already discovered recursively beneath the solution root.

---

# Exclude projects from processing

One or more projects can be skipped entirely using `--exclude-projects`.

```bash
saucery-nuget \
  --solution MySolution.sln \
  --exclude-projects Saucery.Core
```

Multiple projects can be specified:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --exclude-projects Saucery.Core Saucery.XUnit
```

You can specify:

- project name without an extension;
- `.csproj` filename; or
- absolute project path.

Excluded projects are removed from the opted-in project set before processing begins.

---

# Exclude packages from updates

There are three ways to exclude packages.

The effective exclusion list is the union of all applicable exclusion sources.

Matching is case-insensitive and duplicate values are ignored.

---

## 1. Command-line exclusions

For one-off exclusions:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --exclude-packages Selenium.WebDriver Selenium.Support
```

Command-line exclusions also apply when processing `Directory.Packages.props`.

---

## 2. Solution-level configuration

Create:

```text
saucery-nuget.json
```

in the same directory as the solution file.

For example:

```json
{
  "excludePackages": [
    "Selenium.WebDriver",
    "Selenium.Support"
  ]
}
```

The file is loaded automatically.

No command-line argument is required.

If the file does not exist, processing continues normally.

If the file contains invalid JSON, Saucery.NuGet prints a warning and ignores the global configuration file.

These exclusions apply to both:

- `.csproj` processing;
- `Directory.Packages.props` processing.

---

## 3. Per-project or per-file exclusions

### In a `.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Selenium.WebDriver" Version="4.30.0" />
    <PackageReference Include="Selenium.Support" Version="4.30.0" />
    <PackageReference Include="Shouldly" Version="4.2.1" />
  </ItemGroup>

  <ItemGroup>
    <SauceryNuGetExcludePackage Include="Selenium.WebDriver" />
    <SauceryNuGetExcludePackage Include="Selenium.Support" />
  </ItemGroup>

</Project>
```

These exclusions affect only that project.

### In `Directory.Packages.props`

```xml
<Project>

  <ItemGroup>
    <PackageVersion Include="Selenium.WebDriver" Version="4.30.0" />
    <PackageVersion Include="Selenium.Support" Version="4.30.0" />
    <PackageVersion Include="Shouldly" Version="4.2.1" />
  </ItemGroup>

  <ItemGroup>
    <SauceryNuGetExcludePackage Include="Selenium.WebDriver" />
    <SauceryNuGetExcludePackage Include="Selenium.Support" />
  </ItemGroup>

</Project>
```

These exclusions affect only that `Directory.Packages.props` file.

---

# All options

| Option | Alias | Description |
|---|---|---|
| `--solution <path>` | `-s` | Path to the solution file to process. Required. |
| `--dry-run` | | Print proposed changes without writing files. |
| `--include-prerelease` | | Consider prerelease versions as update candidates. |
| `--bump-own-version` | | Increment the project's own `PackageVersion` when dependencies change. |
| `--version-segment <segment>` | | `patch` by default, or `minor` or `major`. |
| `--project <project>` | `-p` | Limit project processing to a specific opted-in project. |
| `--sync-with <dependency>` | `-w` | Synchronise the selected project's `PackageVersion` with a dependency. |
| `--scan-unregistered` | | Include `.csproj` files not registered in the solution. |
| `--exclude-packages <packages>` | | Exclude one or more packages from updates. |
| `--exclude-projects <projects>` | | Exclude one or more projects from project processing. |

---

# What "next version" means

Saucery.NuGet deliberately updates dependencies incrementally.

Given:

```text
Current:   1.2.0

Available:
1.2.0
1.3.0
1.4.0
2.0.0
```

Saucery.NuGet selects:

```text
1.3.0
```

It always selects the smallest available version that is strictly greater than the current version.

The next run may then select:

```text
1.4.0
```

and a later run:

```text
2.0.0
```

This allows each dependency upgrade to be independently validated by your build and test pipeline.

---

# Example repository using Central Package Management

Given:

```text
MyRepository/
├── MySolution.sln
├── Directory.Packages.props
├── saucery-nuget.json
├── src/
│   ├── MyLibrary/
│   │   └── MyLibrary.csproj
│   └── MyApplication/
│       └── MyApplication.csproj
└── tests/
    └── MyLibrary.Tests/
        └── MyLibrary.Tests.csproj
```

`Directory.Packages.props`:

```xml
<Project>

  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
    <PackageVersion Include="Shouldly" Version="4.2.1" />
    <PackageVersion Include="TUnit" Version="1.0.0" />
  </ItemGroup>

</Project>
```

`MyLibrary.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PackageVersion>1.0.0</PackageVersion>
    <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" />
  </ItemGroup>

</Project>
```

Run:

```bash
saucery-nuget \
  --solution MySolution.sln \
  --dry-run
```

Saucery.NuGet will:

1. Discover opted-in projects.
2. Discover `Directory.Packages.props`.
3. Check each centrally managed `PackageVersion`.
4. Apply configured package exclusions.
5. Determine the next available version for each dependency.
6. Report the proposed changes.

Remove `--dry-run` to write the updates.

---

# CI / Pipeline integration

Typical GitHub Actions usage:

```yaml
- name: Install Saucery.NuGet
  run: dotnet tool install --global Saucery.NuGet

- name: Add .NET tools to PATH
  run: echo "$HOME/.dotnet/tools" >> $GITHUB_PATH

- name: Run Saucery.NuGet
  run: saucery-nuget --solution MySolution.sln
```

When Central Package Management is used, no additional command-line option is required.

`Directory.Packages.props` files beneath the solution root are discovered automatically.

For a real-world example, see the Saucery dogfooding pipeline:

https://github.com/Sauceforge/Saucery/blob/master/.github/workflows/saucery-nuget-sync-and-commit.yml

---

# How it works

## `.csproj` pipeline

1. Parses the solution to discover `.csproj` files.
2. Optionally discovers unregistered `.csproj` files when `--scan-unregistered` is used.
3. Filters projects to those containing:

   ```xml
   <SauceryNuGetOptIn>true</SauceryNuGetOptIn>
   ```

4. Removes projects listed in `--exclude-projects`.
5. Loads `saucery-nuget.json` from the solution directory, when present.
6. Merges solution-level exclusions with `--exclude-packages`.
7. Reads project-specific `SauceryNuGetExcludePackage` entries.
8. Finds `PackageReference` entries.
9. Skips packages in the effective exclusion list.
10. Resolves the next available version using `NuGet.Versioning`.
11. Updates the `.csproj` while preserving its encoding and BOM.
12. Optionally updates the project's own `PackageVersion`.

---

## `Directory.Packages.props` pipeline

1. Recursively discovers every `Directory.Packages.props` file beneath the solution root.
2. Skips `bin` and `obj` directories.
3. Loads global exclusions from:
   - `--exclude-packages`;
   - `saucery-nuget.json`.
4. Reads file-specific `SauceryNuGetExcludePackage` entries.
5. Merges the exclusion sources into the effective exclusion list for the file.
6. Finds every `PackageVersion` entry.
7. Skips packages in the effective exclusion list.
8. Resolves the next available version using `NuGet.Versioning`.
9. Updates the `Directory.Packages.props` file while preserving its encoding and BOM.

No explicit opt-in property is required for `Directory.Packages.props`.

---

# Contributing

Contributions are welcome.

- Fork the repository.
- Create a branch.
- Follow the existing coding and testing patterns.
- Submit a pull request.

Source:

https://github.com/Sauceforge/Saucery

---

# License

Copyright © 2024 Andrew Gray.

All rights reserved.