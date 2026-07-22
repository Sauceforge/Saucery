Skill: detect-available-platforms
Path: .copilot/skills/detect-available-platforms/SKILL.md

Purpose
-------
A Copilot skill to detect available Android platform implementations in the Saucery.Core codebase and apply the minimal source changes required to register new platforms across the codebase (factory, Dojo extensions, and test platform lists). This skill automates the series of edits that were applied in commit 478bbe4 (adding Android 17 support).

Behavior
--------
- Scan the repository for ConcreteProducts/Google/Android{N}Platform.cs files and identify the set of available Android major versions.
- For any Android major version found that is not already registered in the platform registration points, add the necessary switch arms and typeof(...) entries in:
  - Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs
  - Saucery.Core/Dojo/DojoExtensions.cs
  - Saucery.Core.Tests/REST/Data/PlatformTypes.cs
  - Saucery.Core.Tests.XUnitv3/REST/PlatformTypes.cs
- Optionally create minimal placeholder creator/product files if an implementation file is present in only one of the two expected locations.

Usage
-----
Run the included PowerShell tool: tools\\detect-platforms\\detect-platforms.ps1 from the repository root (pwsh.exe). The script is idempotent and will only apply edits when required. It creates simple backups of files it edits (appending .bak) so changes can be reviewed or reverted.

Notes
-----
- This skill performs conservative text edits using simple heuristics; it is intended to accelerate repetitive changes and should be reviewed before committing.
- Complex formatting and semantic correctness (for example, ensuring using/import statements and namespaces compile) should be verified by running the solution build after applying changes.
