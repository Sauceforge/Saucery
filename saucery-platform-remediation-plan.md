# Saucery Platform Coverage Remediation Plan

This plan is inferred from the current repository structure. It does not select a hard-coded per-platform remediation strategy.

| Discrepancy | Disposition | Inference |
| --- | --- | --- |
| ChromiumOS (Missing) | AutoFix | Infer remediation from repository analogue LinuxPlatform (Linux ). No platform-specific remediation class is used. |
| iOS 13.x (ObsoleteCandidate) | ManualReview | The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed. |
| iOS 26.0 (ObsoleteCandidate) | ManualReview | The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed. |
| Windows 8 (ObsoleteCandidate) | ManualReview | The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed. |
| Windows 8.1 (ObsoleteCandidate) | ManualReview | The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed. |

## Missing: ChromiumOS

Infer remediation from repository analogue LinuxPlatform (Linux ). No platform-specific remediation class is used.

- **Clone/transform** `Saucery.Core/Dojo/Platforms/ConcreteProducts/Linux/LinuxPlatform.cs` — Use LinuxPlatform as the structural product analogue.
- **Clone/transform** `Saucery.Core/Dojo/Platforms/ConcreteCreators/Linux/LinuxPlatformCreator.cs` — Use LinuxPlatformCreator as the creator analogue.
- **Clone/transform** `Saucery.Core/Dojo/Browsers/ConcreteCreators/Linux/LinuxBrowserCreator.cs` — Use LinuxBrowserCreator as the browser-creator analogue.
- **Infer registration** `Saucery.Core/Dojo/PlatformConfigurator.cs` — Transform an analogous registration/test line when semantically applicable.
- **Infer registration** `Saucery.Core/Dojo/DojoExtensions.cs` — Transform an analogous registration/test line when semantically applicable.
- **Infer registration** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Transform an analogous registration/test line when semantically applicable.
- **Infer registration** `Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs` — Transform an analogous registration/test line when semantically applicable.
- **Infer registration** `Saucery.Core.Tests/DesktopFactoryVersionTests.cs` — Transform an analogous registration/test line when semantically applicable.
- **Infer registration** `Saucery.Core.Tests/DataProviders/PlatformDataClass.cs` — Transform an analogous registration/test line when semantically applicable.
- **Infer registration** `Saucery.Core.Tests/REST/Data/PlatformTypes.cs` — Transform an analogous registration/test line when semantically applicable.
- **Infer helper** `Saucery.Core/Dojo/PlatformConfigurator.cs` — Clone a family-specific helper when a new family requires one.
- ⚠ All generated C# is syntax-checked; source changes are retained only after build, tests, and reporter verification pass.

## ObsoleteCandidate: iOS 13.x

The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed.

- ⚠ Obsolete-candidate removal is opt-in because a Sauce catalogue can be incomplete.

## ObsoleteCandidate: iOS 26.0

The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed.

- ⚠ Obsolete-candidate removal is opt-in because a Sauce catalogue can be incomplete.

## ObsoleteCandidate: Windows 8

The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed.

- ⚠ Obsolete-candidate removal is opt-in because a Sauce catalogue can be incomplete.

## ObsoleteCandidate: Windows 8.1

The repository graph can remove this candidate, but destructive remediation is disabled. Re-run with --allow-obsolete-removal after lifecycle evidence has been reviewed.

- ⚠ Obsolete-candidate removal is opt-in because a Sauce catalogue can be incomplete.
