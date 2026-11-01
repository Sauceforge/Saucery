# Saucery Platform Coverage Remediation Plan

This plan is inferred from the current repository structure. It does not select a hard-coded per-platform remediation strategy.

| Discrepancy | Disposition | Inference |
| --- | --- | --- |
| iOS 13.x (ObsoleteCandidate) | AutoFix | Delete IOS13Platform and IOS13PlatformCreator, then remove only exact identifier references required for compilation. |
| iOS 26.0 (ObsoleteCandidate) | ManualReview | The exact obsolete ConcreteProduct/PlatformCreator pair could not be resolved safely. |
| Windows 8 (ObsoleteCandidate) | AutoFix | Delete Windows8Platform and Windows8PlatformCreator, then remove only exact identifier references required for compilation. |
| Windows 8.1 (ObsoleteCandidate) | AutoFix | Delete Windows81Platform and Windows81PlatformCreator, then remove only exact identifier references required for compilation. |

## ObsoleteCandidate: iOS 13.x

Delete IOS13Platform and IOS13PlatformCreator, then remove only exact identifier references required for compilation.

- **Delete** `Saucery.Core/Dojo/Platforms/ConcreteProducts/Apple/IOS13Platform.cs` — Delete exact obsolete ConcreteProduct IOS13Platform.
- **Delete** `Saucery.Core/Dojo/Platforms/ConcreteCreators/Apple/IOS13PlatformCreator.cs` — Delete exact obsolete PlatformCreator IOS13PlatformCreator.
- ⚠ Obsolete removal never uses nearest-version inference or fuzzy text matching.

## ObsoleteCandidate: iOS 26.0

The exact obsolete ConcreteProduct/PlatformCreator pair could not be resolved safely.

- ⚠ No exact class declaration was found for Windows260Platform.

## ObsoleteCandidate: Windows 8

Delete Windows8Platform and Windows8PlatformCreator, then remove only exact identifier references required for compilation.

- **Delete** `Saucery.Core/Dojo/Platforms/ConcreteProducts/PC/Windows8Platform.cs` — Delete exact obsolete ConcreteProduct Windows8Platform.
- **Delete** `Saucery.Core/Dojo/Platforms/ConcreteCreators/PC/Windows8BPlatformCreator.cs` — Delete exact obsolete PlatformCreator Windows8PlatformCreator.
- ⚠ Obsolete removal never uses nearest-version inference or fuzzy text matching.

## ObsoleteCandidate: Windows 8.1

Delete Windows81Platform and Windows81PlatformCreator, then remove only exact identifier references required for compilation.

- **Delete** `Saucery.Core/Dojo/Platforms/ConcreteProducts/PC/Windows81Platform.cs` — Delete exact obsolete ConcreteProduct Windows81Platform.
- **Delete** `Saucery.Core/Dojo/Platforms/ConcreteCreators/PC/Windows81PlatformCreator.cs` — Delete exact obsolete PlatformCreator Windows81PlatformCreator.
- ⚠ Obsolete removal never uses nearest-version inference or fuzzy text matching.
