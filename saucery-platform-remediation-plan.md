# Saucery Platform Coverage Remediation Plan

This plan is inferred from the current repository structure. It does not select a hard-coded per-platform remediation strategy.

| Discrepancy | Disposition | Inference |
| --- | --- | --- |
| iOS 26.0 (ObsoleteCandidate) | AutoFix | Remove only the exact iOS 26.0 declarations from BrowserFactory/DojoExtensions and matching unit-test data. Shared major-family product/creator implementations are preserved. |

## ObsoleteCandidate: iOS 26.0

Remove only the exact iOS 26.0 declarations from BrowserFactory/DojoExtensions and matching unit-test data. Shared major-family product/creator implementations are preserved.

- **Remove exact declaration** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Remove exact iOS 26.0 browser declaration when present.
- **Remove exact declaration** `Saucery.Core/Dojo/DojoExtensions.cs` — Remove exact iOS 26.0 Dojo declaration when present.
- ⚠ Exact-version cleanup never deletes a shared major-family ConcreteProduct or PlatformCreator.
