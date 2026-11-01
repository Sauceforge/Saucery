# Saucery.CoverageGapRemediator

`Saucery.CoverageGapRemediator` is a .NET 10 console application that consumes the output of `Saucery.CoverageGapReporter` and produces a deterministic remediation plan for `Saucery.Core` and `Saucery.Core.Tests`.

The safe default is **plan only**. Source code is not changed unless `--apply` is supplied.

## Supported input

The remediator reads either:

- the current human-readable `saucery-functional-platform-coverage-gaps.txt` report, or
- a future structured JSON report with `missing` and `obsoleteCandidates` arrays.

The current text parser consumes the reporter's `CORE FUNCTIONAL PLATFORM GAPS` and `SAUCERY COVERAGE NO LONGER REQUIRED BY CURRENT SAUCE CATALOGUE` sections.

## Remediation model

The tool deliberately does not make arbitrary AI-style edits. Every automatic change must be handled by an explicit strategy.

Current automatic strategies cover:

- missing ChromiumOS desktop support;
- missing Android exact versions;
- missing Android major/wildcard real-device support;
- missing iOS exact versions;
- missing iOS major/wildcard real-device support;
- missing versioned macOS support;
- missing versioned Windows support;
- removal of confirmed obsolete Android/iOS/mobile/desktop mappings when destructive removal is explicitly enabled.

Unknown platform shapes are emitted as `ManualReview`; they are never guessed.

### Important obsolete-platform safeguard

An `OBSOLETE?` finding is not treated as proof that Sauce Labs has dropped a platform. By default every obsolete candidate remains plan-only/manual review.

Automatic removal requires:

```text
--allow-obsolete-removal
```

Windows 8 and Windows 8.1 are additionally hard-blocked from automatic removal because the Sauce API-derived catalogue has already been observed to disagree with the Platform Configurator for those platforms.

## Usage

From the repository root:

```powershell
dotnet run -c Release --project Saucery.CoverageGapRemediator -- `
  --repo-root . `
  --report saucery-functional-platform-coverage-gaps.txt
```

This creates:

```text
saucery-platform-remediation-plan.json
saucery-platform-remediation-plan.md
```

and changes no Saucery source files.

To apply supported non-destructive fixes:

```powershell
dotnet run -c Release --project Saucery.CoverageGapRemediator -- `
  --repo-root . `
  --report saucery-functional-platform-coverage-gaps.txt `
  --apply
```

To allow removal of independently confirmed obsolete candidates:

```powershell
dotnet run -c Release --project Saucery.CoverageGapRemediator -- `
  --repo-root . `
  --report saucery-functional-platform-coverage-gaps.txt `
  --apply `
  --allow-obsolete-removal
```

## Transaction and validation

During `--apply`, every touched file is captured before modification. The transaction is retained only if validation succeeds.

Validation performs:

1. Roslyn syntax validation on every generated or modified C# file before it is written.
2. `dotnet build Saucery.Core/Saucery.Core.csproj -c Release`.
3. `dotnet test Saucery.Core.Tests/Saucery.Core.Tests.csproj -c Release`.
4. If `Saucery.CoverageGapReporter` exists, re-runs it and verifies that every automatically targeted discrepancy disappeared.

If validation fails, all source changes are rolled back unless `--keep-on-validation-failure` was explicitly supplied.

Use `--skip-validation` only for debugging the remediator itself.

## Exit codes

| Code | Meaning |
| ---: | --- |
| 0 | Plan/apply completed and no manual-review items remain. |
| 1 | Remediator error. |
| 2 | Plan/apply completed, but at least one discrepancy requires manual review. |
| 3 | Validation failed after applying changes. Changes are rolled back by default. |

## Why Roslyn is included

The project references `Microsoft.CodeAnalysis.CSharp.Workspaces` and uses Roslyn to reject malformed generated C# before a transaction can continue. The transformation strategies themselves are intentionally narrow and guarded against known Saucery source constructs rather than general-purpose text replacement.

## Recommended GitHub workflow

For source changes, do not push directly to `master`. Run the remediator on a branch, validate it, and open a pull request containing the exact diff.

An example workflow is included under `examples/remediate-coverage-gaps.yml`.
