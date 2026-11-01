# Saucery.CoverageGapReporter

This version reports **functional platform coverage**, not coverage of every Sauce Labs device/configuration row.

## Coverage units

Examples:

- ChromiumOS
- Mac Golden Gate (27)
- iOS 27.1
- iOS 27.x
- Android 17.0
- Android 17.x

Individual iPhones, iPads, Pixels, Galaxies, browser versions, and Electron versions do **not** create separate platform requirements.

## Test philosophy

`Saucery.Core.Tests` only needs representative evidence for a platform family/path. It does not need one test for every physical or simulated device.

The report therefore separates:

- Core functional platform coverage
- Core review items
- Core functional gaps
- Representative test sampling by platform family

## Run

From `C:\gitrepos\Saucery\Saucery.CoverageGapReporter`:

```powershell
dotnet run -c Release
```

The repository root is auto-detected by walking upward until both `Saucery.Core` and `Saucery.Core.Tests` are found.

Default output:

```text
C:\gitrepos\Saucery\saucery-functional-platform-coverage-gaps.txt
```

## Sauce credentials

Use either:

```powershell
$env:SAUCE_USERNAME = "..."
$env:SAUCE_ACCESS_KEY = "..."
```

or:

```powershell
$env:SAUCE_USER_NAME = "..."
$env:SAUCE_API_KEY = "..."
```
