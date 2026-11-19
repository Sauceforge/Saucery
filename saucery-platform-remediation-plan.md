# Saucery Platform Coverage Remediation Plan

This plan is inferred from the current repository structure. It does not select a hard-coded per-platform remediation strategy.

| Discrepancy | Disposition | Inference |
| --- | --- | --- |
| Android 5.0 (Missing) | AutoFix | Clone the nearest existing Android declaration of the same kind at every Saucery.Core registration site and add Android 5.0 to the tests that cover that sibling. |
| iOS 27.1 (Missing) | AutoFix | Clone the nearest existing iOS declaration of the same kind at every Saucery.Core registration site and add iOS 27.1 to the tests that cover that sibling. |
| Mac Golden Gate (27) (Missing) | AutoFix | Clone the nearest existing macOS declaration of the same kind at every Saucery.Core registration site and add Mac Golden Gate (27) to the tests that cover that sibling. |
| Windows 8 (Missing) | AutoFix | Clone the nearest existing Windows declaration of the same kind at every Saucery.Core registration site and add Windows 8 to the tests that cover that sibling. |
| Windows 8.1 (Missing) | AutoFix | Clone the nearest existing Windows declaration of the same kind at every Saucery.Core registration site and add Windows 8.1 to the tests that cover that sibling. |
| Android 9.x (ObsoleteCandidate) | AutoFix | Remove only the real-device Android 9.x declarations and real-device unit-test data. Android9Platform/Android9PlatformCreator are still used by simulator/emulator declarations and are preserved. |
| iOS 14.x (ObsoleteCandidate) | AutoFix | Remove only the real-device iOS 14.x declarations and real-device unit-test data. IOS14Platform/IOS14PlatformCreator are still used by simulator/emulator declarations and are preserved. |

## Missing: Android 5.0

Clone the nearest existing Android declaration of the same kind at every Saucery.Core registration site and add Android 5.0 to the tests that cover that sibling.

- **Clone** `Saucery.Core` — Model Android 5.0 on Android51Platform (Android 5.1).
- **Register** `Saucery.Core/Dojo/DojoExtensions.cs` — Clone Android51Platform registration for Android5Platform.
- **Register** `Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs` — Clone Android51Platform registration for Android5Platform.
- **Add tests** `Saucery.Core.Tests*` — Add Android 5.0 to the same test lists/rows that cover Android 5.1.
- ⚠ All generated C# is syntax-checked; source changes are retained only after build, tests, and reporter verification pass.

## Missing: iOS 27.1

Clone the nearest existing iOS declaration of the same kind at every Saucery.Core registration site and add iOS 27.1 to the tests that cover that sibling.

- **Clone** `Saucery.Core` — Model iOS 27.1 on IOS27Platform (iOS 27.0).
- **Register** `Saucery.Core/Dojo/DojoExtensions.cs` — Clone IOS27Platform registration for IOS271Platform.
- **Register** `Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs` — Clone IOS27Platform registration for IOS271Platform.
- **Add tests** `Saucery.Core.Tests*` — Add iOS 27.1 to the same test lists/rows that cover iOS 27.0.
- ⚠ All generated C# is syntax-checked; source changes are retained only after build, tests, and reporter verification pass.

## Missing: Mac Golden Gate (27)

Clone the nearest existing macOS declaration of the same kind at every Saucery.Core registration site and add Mac Golden Gate (27) to the tests that cover that sibling.

- **Clone** `Saucery.Core` — Model Mac Golden Gate (27) on Mac26Platform (macOS 26).
- **Register** `Saucery.Core/Util/SauceryConstants.cs` — Clone Mac26Platform registration for Mac27Platform.
- **Register** `Saucery.Core/Dojo/PlatformConfigurator.cs` — Clone Mac26Platform registration for Mac27Platform.
- **Register** `Saucery.Core/Dojo/DojoExtensions.cs` — Clone Mac26Platform registration for Mac27Platform.
- **Register** `Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs` — Clone Mac26Platform registration for Mac27Platform.
- **Register** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Clone Mac26Platform registration for Mac27Platform.
- **Add tests** `Saucery.Core.Tests*` — Add Mac Golden Gate (27) to the same test lists/rows that cover macOS 26.
- ⚠ All generated C# is syntax-checked; source changes are retained only after build, tests, and reporter verification pass.

## Missing: Windows 8

Clone the nearest existing Windows declaration of the same kind at every Saucery.Core registration site and add Windows 8 to the tests that cover that sibling.

- **Clone** `Saucery.Core` — Model Windows 8 on Windows10Platform (Windows 10).
- **Register** `Saucery.Core/Util/SauceryConstants.cs` — Clone Windows10Platform registration for Windows8Platform.
- **Register** `Saucery.Core/Dojo/DojoExtensions.cs` — Clone Windows10Platform registration for Windows8Platform.
- **Register** `Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs` — Clone Windows10Platform registration for Windows8Platform.
- **Register** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Clone Windows10Platform registration for Windows8Platform.
- **Add tests** `Saucery.Core.Tests*` — Add Windows 8 to the same test lists/rows that cover Windows 10.
- ⚠ All generated C# is syntax-checked; source changes are retained only after build, tests, and reporter verification pass.

## Missing: Windows 8.1

Clone the nearest existing Windows declaration of the same kind at every Saucery.Core registration site and add Windows 8.1 to the tests that cover that sibling.

- **Clone** `Saucery.Core` — Model Windows 8.1 on Windows10Platform (Windows 10).
- **Register** `Saucery.Core/Util/SauceryConstants.cs` — Clone Windows10Platform registration for Windows81Platform.
- **Register** `Saucery.Core/Dojo/DojoExtensions.cs` — Clone Windows10Platform registration for Windows81Platform.
- **Register** `Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs` — Clone Windows10Platform registration for Windows81Platform.
- **Register** `Saucery.Core/Dojo/Browsers/BrowserFactory.cs` — Clone Windows10Platform registration for Windows81Platform.
- **Add tests** `Saucery.Core.Tests*` — Add Windows 8.1 to the same test lists/rows that cover Windows 10.
- ⚠ All generated C# is syntax-checked; source changes are retained only after build, tests, and reporter verification pass.

## ObsoleteCandidate: Android 9.x

Remove only the real-device Android 9.x declarations and real-device unit-test data. Android9Platform/Android9PlatformCreator are still used by simulator/emulator declarations and are preserved.

- **Remove real-device declaration** `Saucery.Core/Dojo/DojoExtensions.cs` — Remove the major-version real-device mapping for Android 9.x.
- **Remove real-device declaration** `Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs` — Remove the major-version real-device mapping for Android 9.x.
- ⚠ Real-device cleanup never deletes a ConcreteProduct or PlatformCreator that simulator/emulator declarations still use.

## ObsoleteCandidate: iOS 14.x

Remove only the real-device iOS 14.x declarations and real-device unit-test data. IOS14Platform/IOS14PlatformCreator are still used by simulator/emulator declarations and are preserved.

- **Remove real-device declaration** `Saucery.Core/Dojo/DojoExtensions.cs` — Remove the major-version real-device mapping for iOS 14.x.
- **Remove real-device declaration** `Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs` — Remove the major-version real-device mapping for iOS 14.x.
- ⚠ Real-device cleanup never deletes a ConcreteProduct or PlatformCreator that simulator/emulator declarations still use.
