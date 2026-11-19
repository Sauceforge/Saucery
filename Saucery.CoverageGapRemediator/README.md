# Saucery.CoverageGapRemediator

`Saucery.CoverageGapRemediator` consumes discrepancies emitted by `Saucery.CoverageGapReporter` and synthesizes repository changes from Saucery's existing platform implementation patterns.

## Design

The reporter and remediator together keep Saucery.Core in step with the Sauce Labs platform catalogue in both directions: a platform Sauce starts offering is reported as a `Gap` and added; a platform Sauce stops offering is reported as an obsolete candidate and removed. Browser-version choices such as "latest", "prev. major" and "Any version" are not platforms and never produce discrepancies.

Saucery.Core declares three kinds of platform, and each is matched against its own Sauce inventory:

| Kind | Sauce source | Saucery.Core declaration |
|---|---|---|
| Simulator/emulator exact version (`iOS 27.1`, `Android 5.0`) | virtual catalogue | `"iOS 27.1"` / `"Linux 5.0"` in `FindIOSBrowser`/`FindAndroidBrowser`, `"27.1"` in `CreatePlatform` |
| Real-device major (`iOS 27.x`, `Android 17.x`) | real-device catalogue | `"iOS 27"` / `"Linux 17"` in `FindIOSPlatform`/`FindAndroidPlatform`, `"27"` in `CreateRealPlatform` |
| Desktop (`Windows 8.1`, `Mac Golden Gate (27)`) | virtual catalogue | `SauceryConstants.PLATFORM_*` in `DesktopPlatformFactory`, `FindDesktopBrowser`, `BrowserFactory` |

A simulator and a real device of the same major share one product/creator pair (`IOS27Platform`), so additions reuse it and removals keep it while either kind still uses it.

### Additions

Each `Gap` is added by cloning the nearest existing platform of the same kind and family (the *sibling*) at every site Saucery.Core registers one. Type names (`IOS27Platform` → `IOS271Platform`), `SauceryConstants` names and quoted version labels are rewritten; a registration that already exists is left alone. For desktop platforms the report's `Sauce browsers:` line limits the cloned browser lists to what Sauce offers on that OS, and a sibling that already supports all of those browsers is preferred.

Every addition also adds tests, mirroring the sibling's coverage in both test projects:

- simulator/emulator: `typeof` in `SupportedPlatformTypes`, plus the sibling's version list (`IOSPlatformVersions`) or factory-version row, when the sibling has one;
- real-device major: `typeof` in `SupportedRealDeviceTypes`, the `Real*FactoryVersionTests` version list/rows, and a `PlatformDataClass` real-device row using a device Sauce reports for that version;
- desktop: `typeof` in `SupportedPlatformTypes` and `PlatformsWithBrowsersTypes`, and a `DesktopFactoryVersionTests` supported row for a browser Sauce offers.

Shapes with no sibling pattern (for example a new unversioned desktop family) are planned as `ManualReview` rather than guessed.

### Validation

After synthesis the remediator syntax-checks every generated C# file with Roslyn, builds `Saucery.Core` and every `Saucery.Core.Tests*` project, runs the xUnit.v3 tests, and re-runs `Saucery.CoverageGapReporter` to confirm every targeted `Gap` disappeared. If validation fails, the whole transaction is rolled back.

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

Additions and removals update test data in every discovered test suite. Validation builds every discovered test project.

## Real-device obsolete removal

An obsolete real-device major (for example `iOS 14.x`) removes only its real-device declarations: the bare-major lines in `FindIOSPlatform`/`FindAndroidPlatform` and `CreateRealPlatform`, and that family's real-device test data (`IOSRealDevice`/`AndroidRealDevice` rows, `typeof` entries in `SupportedRealDeviceTypes`, and the version in `RealIOS*`/`RealAndroid*` version lists). The product/creator pair and the simulator declarations are kept while simulators still use them; only when nothing else references them does the whole-platform removal below apply.


## Obsolete removal safety

Obsolete remediation removes matching registrations first and never line-edits owned platform/creator class declarations. Product/creator files are deleted only when no surviving source or test project references their declared symbols. Shared platform slices, such as an exact simulator version whose major real-device family is still supported, remain intact.


## Obsolete platform remediation

Obsolete removal is deterministic and does not use nearest-version inference.

Examples:

- `iOS 13.x` -> `IOS13Platform` and `IOS13PlatformCreator`
- `Windows 8` -> `Windows8Platform` and `Windows8PlatformCreator`
- `Windows 8.1` -> `Windows81Platform` and `Windows81PlatformCreator`

The remediator deletes the exact `ConcreteProduct` and `PlatformCreator`, removes only exact C# identifier references to those deleted types that are required for the code to remain valid, and then validates by compiling Core and all discovered Core test projects.

Reporter disappearance is **not** a success criterion for obsolete candidates. The coverage reporter may continue to see legacy constants or metadata after the implementation classes have been removed. Obsolete candidates are complete once the exact implementation pair is removed and the solution compiles/tests successfully.

Reporter disappearance remains the success criterion for `Missing` discrepancies.


## Unit-test remediation for obsolete platforms

Deleting an obsolete platform implementation also updates both discovered Core test suites.

The remediator removes:
- direct references to the deleted `ConcreteProduct`/`PlatformCreator` types;
- exact obsolete platform data rows such as `SauceryConstants.PLATFORM_WINDOWS_8` and `SauceryConstants.PLATFORM_WINDOWS_81`;
- matching strongly typed iOS/Android data rows for the obsolete major version.

When rows are removed from a `DataProviders` source, the matching platform-count test (`DesktopPlatformsTest`, `IOSPlatformsTest`, or `AndroidPlatformsTest`) is decremented by exactly the number of removed rows in that test project.

External Sauce Labs fixture-construction failures are treated as inconclusive only when the run contains no ordinary assertion failure. A real assertion failure such as a stale platform count still fails remediation and must be repaired.


## Validation test runner

The remediation validator builds every discovered `Saucery.Core.Tests*` project so compile-time breakage is caught in both test suites.

For actual unit-test execution, the validator runs **only**:

`Saucery.Core.Tests.XUnitv3/Saucery.Core.Tests.XUnitv3.csproj`

The TUnit project is compiled but is not executed by the remediation pipeline. If the required xUnit.v3 project cannot be found, validation fails explicitly.


## Unit-test remediation

Obsolete platform remediation repairs both `Saucery.Core.Tests` and `Saucery.Core.Tests.XUnitv3`.

- Supported-platform inventory rows for the obsolete platform are removed.
- Expected inventory counts are reduced by the exact number of removed rows.
- `PlatformExpansionTests` rows are not deleted merely because they use an obsolete platform as a generic fixture; those rows are retargeted to `PLATFORM_WINDOWS_10` so expansion cardinality remains unchanged.
- Exact deleted-type references are removed from REST/platform-type test data.
- The xUnit.v3 suite is the executed validation suite.

Any genuine xUnit.v3 assertion failure is a hard failure. It takes precedence over recognized Sauce Labs infrastructure failures, causing transaction rollback and preventing branch/commit/PR creation.


## ChangeCollector consistency

`ChangeCollector.Write` persists remediated content immediately through `RepositoryTransaction`. Subsequent remediation steps read the current transactional file content with `ChangeCollector.Read`; there is no separate pending-write dictionary.


## Count assertion scope

Count assertions are adjusted only for the inventory that actually feeds them:

- desktop-family removals adjust `DesktopPlatformsTest` and `AllPlatformsTest`;
- iOS removals adjust `IOSPlatformsTest` only;
- Android removals adjust `AndroidPlatformsTest` only.

This prevents an iOS/Android remediation from incorrectly decrementing the desktop-backed `AllPlatformsTest` count.


## Test result authority

The xUnit.v3 test output is authoritative over the outer process exit code.

Validation fails immediately if output contains any of:

- `Shouldly.ShouldAssertException`
- xUnit assertion failure markers
- `Test run summary: Failed!`
- a non-zero `failed: N` Microsoft.Testing.Platform summary
- an individual `failed Saucery.Core.Tests.XUnitv3...` result

Only after confirming that the output contains no failed tests may an exit code of zero be treated as PASS. This prevents a successful `dotnet run` process code from masking failed xUnit.v3 tests.


## Internally consistent obsolete remediation

Obsolete remediation now updates the complete platform declaration surface:

- deletes the exact obsolete `ConcreteProduct` when the whole platform family is obsolete;
- deletes the matching `PlatformCreator`;
- removes exact obsolete declarations from `Saucery.Core/Dojo/Browsers/BrowserFactory.cs`;
- removes exact obsolete declarations from `Saucery.Core/Dojo/DojoExtensions.cs`;
- repairs matching data/usages in both Core test suites;
- preserves generic `PlatformExpansionTests` cardinality by retargeting generic fixtures instead of deleting them;
- validates the resulting solution and xUnit.v3 suite.

Exact mobile versions that share a major-family implementation are handled specially. For example, an obsolete `iOS 26.0` declaration does not delete a shared `IOS26Platform` used for `iOS 26.x`; only the exact `26.0` declaration/test data is removed.


## Complete obsolete implementation slice

For a whole obsolete platform family, remediation deletes the complete dedicated implementation slice when those classes exist:

- `*Platform`
- `*PlatformCreator`
- `*BrowserCreator`

It then removes the exact corresponding declarations from `BrowserFactory.cs` and `DojoExtensions.cs`, repairs matching test data/usages, builds the solution, and runs the xUnit.v3 validation suite.

Exact-version declarations that share a major-family implementation (for example `iOS 26.0` sharing `IOS26Platform`) do not delete the shared family classes.


## Repository-graph obsolete discovery

Obsolete whole-platform removal no longer guesses implementation class names from version strings.

The resolver follows the repository's actual wiring:

1. Find the exact obsolete registration in `BrowserFactory.cs`.
2. Read the concrete `*BrowserCreator` type referenced by that registration.
3. Find the exact obsolete mapping in `DojoExtensions.cs` and read the concrete `*Platform` type.
4. Resolve the corresponding `*PlatformCreator` through the platform factory wiring.
5. Delete the discovered `*Platform`, `*PlatformCreator`, and `*BrowserCreator` class files.
6. Remove the exact BrowserFactory and DojoExtensions registrations.
7. Repair matching test data and validate the final solution/xUnit.v3 suite.

Exact mobile versions that share a major-family implementation remain declaration-only removals.


## Shared obsolete platform identity

Exact platform identity resolution is shared by obsolete discovery and synthesis through `ObsoletePlatformIdentity.Exact`. This prevents resolver code from depending on a private synthesizer method.


## Owned obsolete slice discovery

Whole-platform obsolete removal now discovers implementation ownership from source content rather than guessed class names or BrowserFactory constructor syntax.

For each obsolete whole platform it requires exactly one dedicated:

- `*Platform` under `ConcreteProducts`
- `*PlatformCreator` under `ConcreteCreators` that constructs that exact platform type
- `*BrowserCreator` under `BrowserCreators` whose source owns the exact obsolete platform identity/version

If any required implementation cannot be uniquely resolved, remediation fails instead of creating a partial two-file PR.

After discovery, the remediator deletes all three implementation files, removes exact declarations from `BrowserFactory.cs` and `DojoExtensions.cs`, repairs test data, builds, and validates xUnit.v3.


## Nullable resolver fix

Required obsolete implementation slices now use explicit null checks and non-null tuple locals before constructing `ObsoleteTarget`. No nullable tuple `.Value` dereferences remain in the resolver.


## Deterministic obsolete slice resolution

Whole-platform obsolete remediation no longer uses source-content ownership heuristics.

It resolves the exact `*Platform` type deterministically from family/version, locates that exact class declaration under `ConcreteProducts`, derives the exact `*PlatformCreator` class name and locates its declaration regardless of filename, and reads the actual `*BrowserCreator` type directly from the matching `BrowserFactory.cs` registration.

For example, `Windows8PlatformCreator` is located by class declaration even when its file is named `Windows8BPlatformCreator.cs`.

Windows obsolete remediation requires a matching BrowserFactory `*BrowserCreator` registration. Shared-family exact mobile versions such as `iOS 26.0 -> IOS26Platform` remain declaration-only cleanup.


## Effective obsolete version identity

Obsolete remediation now normalizes the target version from `CoverageDiscrepancy.Version` when present, otherwise from `DisplayName` (`Windows 8`, `Windows 8.1`, `iOS 26.0`, etc.). The normalized version is reused for exact product type resolution, Windows platform constants, BrowserFactory registration matching, declaration removal, and obsolete test-row matching.

Shared-family exact mobile versions are detected from the actual class surface rather than depending on `PlatformRequirementKind`; for example, `iOS 26.0` is declaration-only when `IOS260Platform` is absent but `IOS26Platform` exists.


## Obsolete candidates: AutoFix or fail

`ObsoleteCandidate` entries can no longer be emitted as `ManualReview`.

The invariant is now:

- with `--allow-obsolete-removal`: every obsolete candidate must become `AutoFix`;
- if exact obsolete resolution fails: the remediator throws and the workflow fails immediately;
- without `--allow-obsolete-removal`: the remediator throws immediately;
- a plan-only PR caused by obsolete candidates silently falling back to `ManualReview` is no longer possible.

`ManualReview` remains available only for non-obsolete inference paths.


## Idempotent obsolete cleanup

Obsolete remediation now supports partially-remediated repositories. Exact `*Platform`, `*PlatformCreator`, and `*BrowserCreator` type identities are still resolved deterministically, but their class files are optional. If a prior run already deleted one of those files, the remediator continues by removing any remaining exact implementation files, stale `BrowserFactory.cs` / `DojoExtensions.cs` declarations, direct type references, and matching test data.

Already-absent implementation files never cause `ManualReview` or block cleanup. Obsolete candidates remain AutoFix-or-fail.
