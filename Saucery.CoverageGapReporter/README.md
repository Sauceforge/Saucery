# Saucery.CoverageGapReporter

.NET 10 console application that performs a bidirectional functional-platform lifecycle audit between the current Sauce Labs catalogue and `Saucery.Core` / `Saucery.Core.Tests`.

## What it reports

The unit of coverage is a functional platform, not an individual device. Examples include:

- ChromiumOS
- Mac Golden Gate (27)
- iOS 27.1
- iOS 27.x
- Android 17.0
- Android 17.x

The report contains four important classes of result:

1. **Required and covered** — Sauce currently advertises the functional platform and Saucery has support evidence.
2. **Required but missing/review** — Sauce advertises the platform, but Saucery support is missing or cannot be proven safely.
3. **Obsolete Core candidates** — Saucery.Core contains an explicit platform/version declaration that has no matching current Sauce functional-platform requirement.
4. **Representative test coverage** — tests are sampled by platform family/path; there is no requirement to test every individual Sauce device model.

## Obsolete-platform detection

The reporter extracts explicit platform/version declarations from `Saucery.Core` and compares them back to the normalized Sauce platform set. An obsolete result is deliberately labelled a **candidate**, not an automatic deletion instruction, because private targets, temporary catalogue omissions, or unusual source declarations can require manual review.

## Run

From the reporter project directory:

```powershell
dotnet run -c Release
```

The repository root is discovered automatically by walking upward until both `Saucery.Core` and `Saucery.Core.Tests` are found.

Optional override:

```powershell
dotnet run -c Release -- --repo-root C:\gitrepos\Saucery
```

Default report:

```text
saucery-functional-platform-coverage-gaps.txt
```

## Sauce credentials

The reporter accepts either:

```text
SAUCE_USERNAME / SAUCE_ACCESS_KEY
```

or:

```text
SAUCE_USER_NAME / SAUCE_API_KEY
```

Credentials can also be supplied with `--username` and `--access-key`.

## Obsolete-platform extraction

Lifecycle extraction is structural rather than proximity-based. The reporter only creates Saucery declarations from explicit switch mappings whose target platform/creator type is on the same source statement. The target type determines the family (for example, `Android17Platform` is Android even when the Sauce-facing lookup label is `Linux 17.0`). Source evidence includes the exact line number and statement. Desktop numeric versions are canonicalized so equivalent spellings such as macOS `11.00` and Sauce `Mac 11` compare as the same platform.
