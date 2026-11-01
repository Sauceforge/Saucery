# Saucery.CoverageGapRemediator

`Saucery.CoverageGapRemediator` consumes discrepancies emitted by `Saucery.CoverageGapReporter` and synthesizes repository changes from Saucery's existing platform implementation patterns.

## Design

There are no `ChromiumOsMissingStrategy`, `IosExactMissingStrategy`, `Android18Strategy`, or other platform/version-specific remediation classes.

The remediator:

1. scans `Saucery.Core` and `Saucery.Core.Tests`;
2. discovers platform products, creators, browser creators, factory/lookup registrations, helper methods and representative test registrations;
3. chooses the closest structural analogue for each discrepancy;
4. clones and transforms the analogue's implementation slice;
5. syntax-checks every generated C# file with Roslyn;
6. builds `Saucery.Core`;
7. runs `Saucery.Core.Tests`;
8. reruns `Saucery.CoverageGapReporter`;
9. repeats synthesis for any targeted discrepancy that remains, up to `--max-repair-passes`;
10. rolls the entire transaction back if build/test validation fails or the discrepancy cannot be eliminated.

This means future Android/iOS/macOS/Windows versions and new platform families are not selected through a predefined strategy table. The engine attempts to infer them from the repository it is actually modifying.

## Central Package Management

The project uses the repository's Central Package Management. `Microsoft.CodeAnalysis.CSharp.Workspaces` has no version in this `.csproj`; its version belongs in the repository root `Directory.Packages.props`.

```xml
<PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="5.9.0" />
```

## Plan only

```powershell
dotnet run -c Release --project Saucery.CoverageGapRemediator -- `
  --repo-root . `
  --report saucery-functional-platform-coverage-gaps.txt
```

## Apply

```powershell
dotnet run -c Release --project Saucery.CoverageGapRemediator -- `
  --repo-root . `
  --report saucery-functional-platform-coverage-gaps.txt `
  --apply
```

Obsolete-candidate deletion remains opt-in because the reporter deliberately calls these *candidates* and Sauce API/catalogue disagreement has occurred before:

```powershell
--allow-obsolete-removal
```


Validation builds both Core projects and verifies the discrepancy with CoverageGapReporter. Runtime test failures caused by the live Sauce Labs acquisition hook are reported as inconclusive rather than as remediation failures.


## Multiple test suites

The remediator discovers every top-level `Saucery.Core.Tests*` project and treats it as a first-class test surface. This includes both:

- `Saucery.Core.Tests` (TUnit)
- `Saucery.Core.Tests,XUnit3` (xUnit.v3)

When a platform remediation is inferred from an existing analogue, equivalent test registrations/data are transformed in every discovered test suite. Validation builds and runs every discovered test project.


## Obsolete removal safety

Obsolete remediation removes matching registrations first and never line-edits owned platform/creator class declarations. Product/creator files are deleted only when no surviving source or test project references their declared symbols. Shared platform slices, such as an exact simulator version whose major real-device family is still supported, remain intact.


## Obsolete-removal safety

Production-code removal is scoped to registration files structurally associated with the obsolete platform. The remediator does not scan or line-edit unrelated platform product/creator classes.

Only `PLATFORM_*` identity constants found in structural registrations are considered platform-owned. Shared browser, screen-resolution, timeout, and capability constants are never inferred as owned by a platform merely because its product class references them.


## Multi-pass obsolete removal

Each reporter rerun is authoritative. If an obsolete platform has multiple surviving declarations, the next synthesis pass consumes the **fresh evidence file and source line from the newly generated report**, rather than retrying the stale declaration from the previous pass.

If the original product/creator prototype has already been deleted, the remediator can still remove the next exact reporter-evidenced registration after syntax-validating the resulting file.
