# Saucery Platform Coverage Remediation Plan

This plan is inferred from the current repository structure. It does not select a hard-coded per-platform remediation strategy.

| Discrepancy | Disposition | Inference |
| --- | --- | --- |
| iOS 26.0 (ObsoleteCandidate) | AutoFix | No implementation class files remain to delete; remove exact BrowserFactory/DojoExtensions declarations, repair tests, then validate. |
| Windows 8 (ObsoleteCandidate) | AutoFix | Delete remaining implementation classes: Windows8BrowserCreator; remove exact BrowserFactory/DojoExtensions declarations, repair tests, then validate. |
| Windows 8.1 (ObsoleteCandidate) | AutoFix | Delete remaining implementation classes: Windows81BrowserCreator; remove exact BrowserFactory/DojoExtensions declarations, repair tests, then validate. |

## ObsoleteCandidate: iOS 26.0

No implementation class files remain to delete; remove exact BrowserFactory/DojoExtensions declarations, repair tests, then validate.

- **Remove exact declaration** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Remove exact iOS 26.0 browser declaration when present.
- **Remove exact declaration** `Saucery.Core/Dojo/DojoExtensions.cs` — Remove exact iOS 26.0 Dojo declaration when present.
- ⚠ Obsolete cleanup is idempotent: already-removed implementation files do not block removal of remaining declarations or references.

## ObsoleteCandidate: Windows 8

Delete remaining implementation classes: Windows8BrowserCreator; remove exact BrowserFactory/DojoExtensions declarations, repair tests, then validate.

- **Remove exact declaration** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Remove exact Windows 8 browser declaration when present.
- **Remove exact declaration** `Saucery.Core/Dojo/DojoExtensions.cs` — Remove exact Windows 8 Dojo declaration when present.
- **Delete** `Saucery.Core/Dojo/Browsers/ConcreteCreators/PC/Windows8BrowserCreator.cs` — Delete exact obsolete BrowserCreator Windows8BrowserCreator.
- ⚠ Obsolete cleanup is idempotent: already-removed implementation files do not block removal of remaining declarations or references.

## ObsoleteCandidate: Windows 8.1

Delete remaining implementation classes: Windows81BrowserCreator; remove exact BrowserFactory/DojoExtensions declarations, repair tests, then validate.

- **Remove exact declaration** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Remove exact Windows 8.1 browser declaration when present.
- **Remove exact declaration** `Saucery.Core/Dojo/DojoExtensions.cs` — Remove exact Windows 8.1 Dojo declaration when present.
- **Delete** `Saucery.Core/Dojo/Browsers/ConcreteCreators/PC/Windows81BrowserCreator.cs` — Delete exact obsolete BrowserCreator Windows81BrowserCreator.
- ⚠ Obsolete cleanup is idempotent: already-removed implementation files do not block removal of remaining declarations or references.
