<#
Simple platform detector and applicator for Saucery repo.

Run from repo root:
  pwsh.exe -File tools\detect-platforms\detect-platforms.ps1

Or run from this script folder:
  pwsh.exe -File .\detect-platforms.ps1

This script scans the Saucery repo for Android{N}Platform implementations
and ensures registration points in the codebase include corresponding entries.
It is conservative and makes backups (*.bak) before editing.
#>

param(
    [string]$RepoRoot
)

function Resolve-RepoRoot {
    param([string]$ExplicitRepoRoot)

    if (-not [string]::IsNullOrWhiteSpace($ExplicitRepoRoot)) {
        return (Resolve-Path -LiteralPath $ExplicitRepoRoot).Path
    }

    $current = if ($PSScriptRoot) {
        $PSScriptRoot
    }
    else {
        (Get-Location).Path
    }

    while ($current) {
        $sln = Join-Path $current 'Saucery.slnx'
        $coreDir = Join-Path $current 'Saucery.Core'

        if ((Test-Path -LiteralPath $sln) -or (Test-Path -LiteralPath $coreDir)) {
            return $current
        }

        $parent = Split-Path -Parent $current

        if ([string]::IsNullOrWhiteSpace($parent) -or $parent -eq $current) {
            break
        }

        $current = $parent
    }

    throw "Could not resolve repo root. Run from inside the Saucery repo or pass -RepoRoot C:\gitrepos\Saucery"
}

$RepoRoot = Resolve-RepoRoot $RepoRoot

Write-Host "Repo root: $RepoRoot"

function Backup-File {
    param([string]$Path)

    if (Test-Path -LiteralPath $Path) {
        $bak = "$Path.bak"
        Copy-Item -LiteralPath $Path -Destination $bak -Force
        Write-Host "Backed up $Path to $bak"
    }
}

function Read-File {
    param([string]$Path)

    Get-Content -Raw -LiteralPath $Path -ErrorAction Stop
}

function Write-File {
    param(
        [string]$Path,
        [string]$Text
    )

    Set-Content -LiteralPath $Path -Value $Text -Force
    Write-Host "Wrote $Path"
}

$productDirs = @(
    Join-Path $RepoRoot 'Saucery.Core\Dojo\Platforms\ConcreteProducts\Google'
    Join-Path $RepoRoot 'Saucery.Core\Dojo\Platforms\ConcreteCreators\Google'
)

$androidFiles = @()

foreach ($dir in $productDirs) {
    if (Test-Path -LiteralPath $dir) {
        $androidFiles += Get-ChildItem `
            -LiteralPath $dir `
            -Filter '*Android*Platform*.cs' `
            -File `
            -ErrorAction SilentlyContinue
    }
}

if ($androidFiles.Count -eq 0) {
    Write-Host "No Android platform implementation files found under:"
    $productDirs | ForEach-Object { Write-Host "  $_" }
    exit 0
}

$versions = $androidFiles |
    ForEach-Object {
        if ($_.Name -match 'Android(\d+)Platform') {
            [int]$matches[1]
        }
    } |
    Where-Object { $null -ne $_ } |
    Sort-Object -Unique

Write-Host "Discovered Android major versions: $($versions -join ', ')"

$factoryFile = Join-Path $RepoRoot 'Saucery.Core\Dojo\Platforms\AndroidPlatformFactory.cs'
$dojoFile = Join-Path $RepoRoot 'Saucery.Core\Dojo\DojoExtensions.cs'

$testPlatformTypes = @(
    Join-Path $RepoRoot 'Saucery.Core.Tests\REST\Data\PlatformTypes.cs'
    Join-Path $RepoRoot 'Saucery.Core.Tests.XUnitv3\REST\PlatformTypes.cs'
)

function Ensure-LineInFactory {
    param(
        [string]$Content,
        [int]$Major
    )

    $shortLine = "        `"$Major.0`" => new Android${Major}PlatformCreator(sp).Create(),"
    $realLine  = "        `"$Major`" => new Android${Major}PlatformCreator(sp).Create(),"

    $added = $false

    if ($Content -notmatch [regex]::Escape($shortLine)) {
        $pattern = '(sp\.short_version switch\s*\{[\s\S]*?)(\s*_ => null)'

        if ($Content -match $pattern) {
            $Content = [regex]::Replace(
                $Content,
                $pattern,
                {
                    param($m)
                    return $m.Groups[1].Value + "$shortLine`r`n" + $m.Groups[2].Value
                },
                1
            )

            $added = $true
        }
    }

    if ($Content -notmatch [regex]::Escape($realLine)) {
        $pattern = '(sp\.OsVersion\?\.Split\("\."\)\[0\] switch\s*\{[\s\S]*?)(\s*_ => null)'

        if ($Content -match $pattern) {
            $Content = [regex]::Replace(
                $Content,
                $pattern,
                {
                    param($m)
                    return $m.Groups[1].Value + "$realLine`r`n" + $m.Groups[2].Value
                },
                1
            )

            $added = $true
        }
    }

    return @{
        Content = $Content
        Added   = $added
    }
}

function Ensure-LineInDojo {
    param(
        [string]$Content,
        [int]$Major
    )

    $shortLine = "            `"Linux $Major.0`" => platforms.GetPlatform<Android${Major}Platform>().FirstOrDefault(),"
    $realLine  = "            `"Linux $Major`" => platforms.GetPlatform<Android${Major}Platform>().FirstOrDefault(),"

    $added = $false

    if ($Content -notmatch [regex]::Escape($shortLine)) {
        $pattern = '(string platformToSearchFor = \$"\{sp\.Os\} \{sp\.LongVersion\}";\s*PlatformBase\? platform = platformToSearchFor switch\s*\{[\s\S]*?)(\s*_ => null\s*\};)'

        if ($Content -match $pattern) {
            $Content = [regex]::Replace(
                $Content,
                $pattern,
                {
                    param($m)
                    return $m.Groups[1].Value + "$shortLine`r`n" + $m.Groups[2].Value
                },
                1
            )

            $added = $true
        }
    }

    if ($Content -notmatch [regex]::Escape($realLine)) {
        $pattern = '(string platformToSearchFor = \$"\{sp\.Os\} \{sp\.LongVersion\}";\s*return platformToSearchFor switch\s*\{[\s\S]*?)(\s*_ => null\s*\};)'

        if ($Content -match $pattern) {
            $Content = [regex]::Replace(
                $Content,
                $pattern,
                {
                    param($m)
                    return $m.Groups[1].Value + "$realLine`r`n" + $m.Groups[2].Value
                },
                1
            )

            $added = $true
        }
    }

    return @{
        Content = $Content
        Added   = $added
    }
}

function Ensure-LineInTestTypes {
    param(
        [string]$Content,
        [int]$Major,
        [string]$Section
    )

    $needle = "typeof(Android${Major}Platform),"

    if ($Content -match [regex]::Escape($needle)) {
        return @{
            Content = $Content
            Added   = $false
        }
    }

    $pattern = "($Section\s*=>\s*Select\(\s*\(\s*\[)([\s\S]*?)(\]\s*\)\s*\);)"

    if ($Content -match $pattern) {
        $Content = [regex]::Replace(
            $Content,
            $pattern,
            {
                param($m)

                $prefix = $m.Groups[1].Value
                $inner  = $m.Groups[2].Value.TrimEnd()
                $suffix = $m.Groups[3].Value

                return "$prefix$inner`r`n            typeof(Android${Major}Platform),`r`n        $suffix"
            },
            1
        )

        return @{
            Content = $Content
            Added   = $true
        }
    }

    return @{
        Content = $Content
        Added   = $false
    }
}

foreach ($version in $versions) {
    Write-Host "Processing Android $version..."

    if (Test-Path -LiteralPath $factoryFile) {
        $text = Read-File $factoryFile
        $result = Ensure-LineInFactory $text $version

        if ($result.Added) {
            Backup-File $factoryFile
            Write-File $factoryFile $result.Content
        }
    }

    if (Test-Path -LiteralPath $dojoFile) {
        $text = Read-File $dojoFile
        $result = Ensure-LineInDojo $text $version

        if ($result.Added) {
            Backup-File $dojoFile
            Write-File $dojoFile $result.Content
        }
    }

    foreach ($testFile in $testPlatformTypes) {
        if (Test-Path -LiteralPath $testFile) {
            $text = Read-File $testFile

            $result1 = Ensure-LineInTestTypes $text $version 'SupportedPlatformTypes'
            $text = $result1.Content

            $result2 = Ensure-LineInTestTypes $text $version 'SupportedRealDeviceTypes'
            $text = $result2.Content

            if ($result1.Added -or $result2.Added) {
                Backup-File $testFile
                Write-File $testFile $text
            }
        }
    }
}

Write-Host "Done."