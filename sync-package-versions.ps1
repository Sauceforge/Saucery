# sync-package-versions.ps1
# Cross-platform (Windows + GitHub Actions) package sync script
# - Auto-detects both the script directory ($Root) and the repository root ($Repo)
# - No parameters are required or expected
# - Executes per-project child scripts named: sync-package-version.ps1

# ===================== Discover script dir ($Root) =====================
if ($PSVersionTable.PSVersion.Major -ge 3 -and $PSScriptRoot) {
    $Root = $PSScriptRoot
} elseif ($MyInvocation.MyCommand.Path) {
    $Root = Split-Path -Parent $MyInvocation.MyCommand.Path
} else {
    $Root = (Get-Location).Path
}

# Child script name INSIDE EACH PROJECT (singular)
$ScriptName = 'sync-package-version.ps1'

# ===================== Repo root detection (cross-platform) =====================
function Resolve-RepoRoot([string]$start) {
    $git = Get-Command git -ErrorAction SilentlyContinue
    if ($git) {
        try {
            $top = & $git.Path -C $start rev-parse --show-toplevel 2>$null
            if ($LASTEXITCODE -eq 0 -and $top) { return (Resolve-Path -LiteralPath $top).Path }
        } catch { }
    }
    $current = Resolve-Path -LiteralPath $start
    while ($current) {
        if (Test-Path -LiteralPath (Join-Path $current '.git')) { return $current }
        $parent = Split-Path -Parent $current
        if (-not $parent -or $parent -eq $current) { break }
        $current = $parent
    }
    return (Resolve-Path -LiteralPath $start).Path
}

$Repo = Resolve-RepoRoot -start $Root
Write-Host "Using repository root: $Repo"
Write-Host "Using script root    : $Root"

# ===================== Helpers =====================

function Read-FileWithEncoding([string]$path){
  $bytes=[System.IO.File]::ReadAllBytes($path)
  $hasUtf8Bom=($bytes.Length -ge 3 -and $bytes[0]-eq0xEF -and $bytes[1]-eq0xBB -and $bytes[2]-eq0xBF)
  $isLE=($bytes.Length -ge 2 -and $bytes[0]-eq0xFF -and $bytes[1]-eq0xFE)
  $isBE=($bytes.Length -ge 2 -and $bytes[0]-eq0xFE -and $bytes[1]-eq0xFF)
  if($hasUtf8Bom){
    $enc=New-Object System.Text.UTF8Encoding($true)
    @{ Text = $enc.GetString($bytes,3,$bytes.Length-3); Encoder = { New-Object System.Text.UTF8Encoding($true) } }
  } elseif($isLE){
    $enc=New-Object System.Text.UnicodeEncoding($false,$true)
    @{ Text = $enc.GetString($bytes,2,$bytes.Length-2); Encoder = { New-Object System.Text.UnicodeEncoding($false,$true) } }
  } elseif($isBE){
    $enc=New-Object System.Text.UnicodeEncoding($true,$true)
    @{ Text = $enc.GetString($bytes,2,$bytes.Length-2); Encoder = { New-Object System.Text.UnicodeEncoding($true,$true) } }
  } else {
    $enc=New-Object System.Text.UTF8Encoding($false)
    @{ Text = $enc.GetString($bytes); Encoder = { New-Object System.Text.UTF8Encoding($false) } }
  }
}
function Write-FileWithEncoding([string]$path,[string]$text,[scriptblock]$EncoderFactory){
  $enc = & $EncoderFactory
  [System.IO.File]::WriteAllText($path,$text,$enc)
}

function New-NamespaceManager([xml]$xml) {
  $nsMgr = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
  $uri = $xml.DocumentElement.NamespaceURI
  if ($uri) { $nsMgr.AddNamespace('msbuild', $uri) }
  $nsMgr
}
function Select-SingleNodeNs([xml]$Xml,[string]$WithNs,[string]$NoNs){
  $ns = New-NamespaceManager $Xml
  if ($Xml.DocumentElement.NamespaceURI) { $Xml.SelectSingleNode($WithNs, $ns) } else { $Xml.SelectSingleNode($NoNs) }
}

function Update-VersionValue-InStartTag {
  param([string]$Text,[string]$PackageId,[string]$NewVersion)
  $pkgPattern = [regex]::new('Include\s*=\s*"'+[regex]::Escape($PackageId)+'"', 'IgnoreCase')
  $m = $pkgPattern.Match($Text)
  if(-not $m.Success){ return $Text, $false }
  $start = $Text.LastIndexOf('<', $m.Index)
  if($start -lt 0){ return $Text, $false }
  $end = $Text.IndexOf('>', $m.Index)
  if($end -lt 0){ return $Text, $false }
  $tag = $Text.Substring($start, $end - $start + 1)
  $verRegex = [regex]::new('\bVersion\s*=\s*"([^"]*)"', 'IgnoreCase')
  if(-not $verRegex.IsMatch($tag)){ return $Text, $false }
  $newTag = $verRegex.Replace($tag, { param($mm) 'Version="'+$NewVersion+'"' }, 1)
  if($newTag -eq $tag){ return $Text, $false }
  $newText = $Text.Substring(0,$start) + $newTag + $Text.Substring($end+1)
  return $newText, $true
}

# Parse PackageReference Include/Version mapping from an XML string
function Get-PackageReferencesFromXml([string]$xmlText) {
  $map = @{}
  if ([string]::IsNullOrWhiteSpace($xmlText)) { return $map }
  try {
    [xml]$x = $xmlText
  } catch {
    try { [xml]$x = [xml](Get-Content -Raw -LiteralPath $xmlText) } catch { return $map }
  }
  $ns = New-NamespaceManager $x
  if ($x.DocumentElement.NamespaceURI) {
    $nodes = $x.SelectNodes("//msbuild:PackageReference", $ns)
  } else {
    $nodes = $x.SelectNodes("//*[local-name()='PackageReference']")
  }
  foreach ($n in $nodes) {
    $inc = $n.GetAttribute('Include')
    $ver = $n.GetAttribute('Version')
    if (-not $ver -and $n.HasChildNodes) {
      if ($x.DocumentElement.NamespaceURI) {
        $vnode = $n.SelectSingleNode("msbuild:Version", $ns)
      } else {
        $vnode = $n.SelectSingleNode("*[local-name()='Version']")
      }
      if ($vnode) { $ver = $vnode.InnerText.Trim() }
    }
    if ($inc) { $map[$inc] = $ver }
  }
  return $map
}

# Get PackageReference mapping from a csproj file path
function Get-PackageReferencesFromPath([string]$path) {
  if (-not (Test-Path -LiteralPath $path)) { return @{} }
  $rf = Read-FileWithEncoding -path $path
  return Get-PackageReferencesFromXml -xmlText $rf.Text
}

# normalize package reference map: trim, lowercase keys, trim values, empty => $null
function Normalize-PackageRefs([hashtable]$map) {
  $norm = @{}
  if (-not $map) { return $norm }
  foreach ($rawKey in $map.Keys) {
    $k = $rawKey.ToString().Trim().ToLowerInvariant()
    $v = $map[$rawKey]
    if ([string]::IsNullOrWhiteSpace($v)) { $v = $null } else { $v = $v.ToString().Trim() }
    $norm[$k] = $v
  }
  return $norm
}

# Diagnostic helper: print before/after normalized maps and diffs
function Write-PackageRefDiffs([hashtable]$beforeNorm, [hashtable]$afterNorm, [string]$context) {
  Write-Host ""
  Write-Host "=== PackageReference DIAGNOSTIC ($context) ==="
  Write-Host "Keys in before: $($beforeNorm.Keys -join ', ')"
  Write-Host "Keys in after : $($afterNorm.Keys -join ', ')"
  $all = @($beforeNorm.Keys + $afterNorm.Keys) | Sort-Object -Unique
  foreach ($k in $all) {
    $b = $null; $a = $null
    if ($beforeNorm.ContainsKey($k)) { $b = $beforeNorm[$k] }
    if ($afterNorm.ContainsKey($k))  { $a = $afterNorm[$k] }
    if ($b -ne $a) {
      Write-Host ("  DIFF: {0} => before: '{1}'  after: '{2}'" -f $k, ($b -ne $null ? $b : '<null>'), ($a -ne $null ? $a : '<null>'))
    }
  }
  Write-Host "=== END DIAGNOSTIC ($context) ==="
  Write-Host ""
}

# Extract <PackageVersion> value from XML text
function Get-PackageVersionFromXmlText([string]$xmlText) {
  if ([string]::IsNullOrWhiteSpace($xmlText)) { return $null }
  $pattern = '(?ms)<PackageVersion\b[^>]*>\s*([^<\r\n]+?)\s*</PackageVersion>'
  $m = [regex]::Match($xmlText, $pattern)
  if ($m.Success) { return $m.Groups[1].Value.Trim() }
  return $null
}

# Get <PackageVersion> from csproj path
function Get-PackageVersionFromPath([string]$path) {
  if (-not (Test-Path -LiteralPath $path)) { return $null }
  $rf = Read-FileWithEncoding -path $path
  return Get-PackageVersionFromXmlText -xmlText $rf.Text
}

# Replace <PackageVersion> value in a csproj path with the provided new version (preserve formatting)
function Replace-PackageVersionInPath([string]$path, [string]$newVersion) {
  if (-not (Test-Path -LiteralPath $path)) { Write-Warning "Project file not found: $path"; return $false }
  $rf = Read-FileWithEncoding -path $path
  $text = $rf.Text
  $enc  = $rf.Encoder

  $pattern = '(?ms)(<PackageVersion\b[^>]*>\s*)([^<\r\n]+?)(\s*</PackageVersion>)'
  $m = [regex]::Match($text, $pattern)
  if (-not $m.Success) {
    Write-Warning "No <PackageVersion> element found in $path"
    return $false
  }

  $old = $m.Groups[2].Value.Trim()
  $newText = [regex]::Replace($text, $pattern, { param($mm) $mm.Groups[1].Value + $newVersion + $mm.Groups[3].Value }, 1)
  Write-Host ("Setting PackageVersion in {0}: {1} -> {2}" -f $path, $old, $newVersion)
  Write-FileWithEncoding -path $path -text $newText -EncoderFactory $enc
  return $true
}

# Compute a SemVer-style bump of the last numeric segment (e.g. 4.10.6 -> 4.10.7)
function Bump-VersionString([string]$baseVersion) {
  if ([string]::IsNullOrWhiteSpace($baseVersion)) { return $null }
  $segments = $baseVersion.Trim() -split '\.'
  if ($segments.Length -eq 0) { return $null }

  $lastIndex = $segments.Length - 1
  $parsed = 0
  if (-not [int]::TryParse($segments[$lastIndex], [ref]$parsed)) {
    Write-Warning "Cannot bump non-numeric version segment in base version '$baseVersion'"
    return $null
  }

  $parsed = $parsed + 1
  $segments[$lastIndex] = $parsed.ToString()

  return ($segments -join '.')
}

function Get-SelfPackageVersionFromProject([string]$ProjectRoot){
  $proj = Get-ChildItem -Path $ProjectRoot -Recurse -Filter '*.csproj' -File -ErrorAction SilentlyContinue | Select-Object -First 1
  if(-not $proj){ return $null }
  try { [xml]$x = Get-Content -Raw -LiteralPath $proj.FullName } catch { return $null }
  $verNode = Select-SingleNodeNs $x "//msbuild:PackageVersion" "//*[local-name()='PackageVersion']"
  if($verNode -and $verNode.InnerText){ return $verNode.InnerText.Trim() }
  $verNode = Select-SingleNodeNs $x "//msbuild:Version" "//*[local-name()='Version']"
  if($verNode -and $verNode.InnerText){ return $verNode.InnerText.Trim() }
  return $null
}

function Get-PackageVersionFromRoot {
  param([string]$ProjectRoot,[string]$PackageId)

  # 1) Directory.Packages.props (central pkg mgmt)
  $props = Get-ChildItem -Path $ProjectRoot -Recurse -Filter 'Directory.Packages.props' -File -ErrorAction SilentlyContinue | Select-Object -First 1
  if ($props) {
    try {
      [xml]$xml = Get-Content -Raw -LiteralPath $props.FullName
      $node = Select-SingleNodeNs $xml "//msbuild:PackageVersion[@Include='$PackageId']" "//*[local-name()='PackageVersion' and @Include='$PackageId']"
      if ($node) { return $node.GetAttribute('Version') }
    } catch { }
  }

  # 2) Explicit PackageReference in a csproj (with @Version)
  $proj = Get-ChildItem -Path $ProjectRoot -Recurse -Filter '*.csproj' -File -ErrorAction SilentlyContinue
  foreach($p in $proj){
    try {
      [xml]$x = Get-Content -Raw -LiteralPath $p.FulLName
      $n = Select-SingleNodeNs $x "//msbuild:PackageReference[@Include='$PackageId' and @Version]" "//*[local-name()='PackageReference' and @Include='$PackageId' and @Version]"
      if ($n) { return $n.GetAttribute('Version') }
    } catch { }
  }

  # 3) First-party package fallback: project PackageVersion/Version
  $self = Get-SelfPackageVersionFromProject -ProjectRoot $ProjectRoot
  if ($self) { return $self }

  return $null
}

function Set-TemplatePackages {
  param([string]$TemplatePath,[hashtable]$PackageVersions)
  if (-not (Test-Path -LiteralPath $TemplatePath)) {
    Write-Warning "Template not found: $TemplatePath"
    return $false
  }
  $rf = Read-FileWithEncoding -path $TemplatePath
  $text = $rf.Text
  $enc  = $rf.Encoder
  $modified = $false
  foreach($k in $PackageVersions.Keys){
    $v = $PackageVersions[$k]
    if ([string]::IsNullOrWhiteSpace($v)) { continue }
    $res = Update-VersionValue-InStartTag -Text $text -PackageId $k -NewVersion $v
    $text = $res[0]; $did = $res[1]
    if ($did) { $modified = $true; Write-Host ("   - {0} => {1}" -f $k, $v) }
  }
  if ($modified) {
    Write-Host ("   Saving {0}" -f $TemplatePath)
    Write-FileWithEncoding -path $TemplatePath -text $text -EncoderFactory $enc
  } else {
    Write-Host '   No template changes needed.'
  }
  return $modified
}

function Get-PowerShellExe {
    $pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
    if ($pwsh) { return $pwsh.Path }
    (Get-Command powershell).Path
}

function Invoke-Sync([string]$Project,[string]$Repo,[string]$ScriptName) {
    $projectDir = Join-Path $Repo $Project
    $scriptPath = Join-Path $projectDir $ScriptName
    if (-not (Test-Path -LiteralPath $projectDir)) { Write-Error "Project dir not found: $projectDir"; return }
    if (-not (Test-Path -LiteralPath $scriptPath)) { Write-Error "Script not found: $scriptPath";   return }

    Write-Host ("-> {0}" -f $Project)
    Write-Host ("   Using script: {0}" -f $scriptPath)

    $exe  = Get-PowerShellExe
    $args = @('-NoProfile','-ExecutionPolicy','Bypass','-File', $scriptPath)

    try { Push-Location $projectDir; & $exe @args; $code = $LASTEXITCODE }
    catch { Write-Error ("X Failed {0}: {1}" -f $Project, $_.Exception.Message); return }
    finally { Pop-Location }

    if ($code -ne 0) { Write-Error ("X Failed {0}: child exited with code {1}" -f $Project, $code); return }
    Write-Host ("OK Synced {0}" -f $Project)
}

$Projects = @('Saucery','Saucery.Playwright.NUnit','Saucery.XUnit','Saucery.XUnit3','Saucery.TUnit')

try { Clear-Host } catch {}

# ---------------------------------------------------------------------
# Saucery.Core: capture package-ref snapshot, run update block, then
# bump PackageVersion ONLY if package-reference versions actually changed.
# ---------------------------------------------------------------------
$sauceryCoreCsprojPath = Join-Path (Join-Path $Repo 'Saucery.Core') 'Saucery.Core.csproj'
$SauceryCoreBumped = $false
$origSauceryCoreRefsNorm = @{}
$PostUpdateRefsChanged = $false

if (Test-Path -LiteralPath $sauceryCoreCsprojPath) {
  $orig = Get-PackageReferencesFromPath -path $sauceryCoreCsprojPath
  $origSauceryCoreRefsNorm = Normalize-PackageRefs -map $orig
} else {
  Write-Warning "Saucery.Core.csproj not found at expected path before updates: $sauceryCoreCsprojPath"
}

# ===== The Update-NuGetNext-All calls for Saucery.Core =====
.\Update-NuGetNext-All.ps1 -PackageId 'Appium.WebDriver'                     -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Castle.Core'                           -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'DotNetSeleniumExtras.PageObjects.Core' -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'DotNetSeleniumExtras.WaitHelpers'      -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'RestSharp'                             -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Selenium.Support'                      -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Selenium.WebDriver'                    -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Shouldly'                              -Root (Join-Path $Repo 'Saucery.Core')
# ================================================================================

# Re-capture package refs and compare normalized maps
if (Test-Path -LiteralPath $sauceryCoreCsprojPath) {
  $new = Get-PackageReferencesFromPath -path $sauceryCoreCsprojPath
  $newSauceryCoreRefsNorm = Normalize-PackageRefs -map $new

  $refsChanged = $false
  $allKeys = @($origSauceryCoreRefsNorm.Keys + $newSauceryCoreRefsNorm.Keys) | Sort-Object -Unique
  foreach ($k in $allKeys) {
    $o = $null; $n = $null
    if ($origSauceryCoreRefsNorm.ContainsKey($k)) { $o = $origSauceryCoreRefsNorm[$k] }
    if ($newSauceryCoreRefsNorm.ContainsKey($k))  { $n = $newSauceryCoreRefsNorm[$k] }
    if ($o -ne $n) { $refsChanged = $true; break }
  }

  # record for the HEAD-based step below
  $PostUpdateRefsChanged = $refsChanged

  if ($refsChanged) {
    Write-Host "Saucery.Core PackageReference versions changed as a result of the Update-NuGetNext-All calls. Bumping PackageVersion."
    Write-PackageRefDiffs -beforeNorm $origSauceryCoreRefsNorm -afterNorm $newSauceryCoreRefsNorm -context "post-update"

    $currentVersion = Get-PackageVersionFromPath -path $sauceryCoreCsprojPath
    if (-not $currentVersion) {
      Write-Warning "Cannot determine current <PackageVersion> in Saucery.Core.csproj after updates; skipping bump."
    } else {
      $bumped = Bump-VersionString -baseVersion $currentVersion
      if (-not $bumped) {
        Write-Warning "Failed to compute bumped version from base '$currentVersion'; skipping bump."
      } else {
        if (Replace-PackageVersionInPath -path $sauceryCoreCsprojPath -newVersion $bumped) {
          Write-Host "Saucery.Core.csproj PackageVersion updated to $bumped (post-update block)."
          $SauceryCoreBumped = $true
        } else {
          Write-Warning "Failed to write bumped PackageVersion to $sauceryCoreCsprojPath"
        }
      }
    }
  } else {
    Write-Host "Saucery.Core PackageReference versions were NOT changed by the Update-NuGetNext-All calls; no PackageVersion bump."
  }
} else {
  Write-Host "Skipping post-update change detection for Saucery.Core.csproj (file missing)."
}

# -----------------------
# Immediate HEAD-based check (run after the above but do not double-bump)
# - ONLY run HEAD-based bump if the Update-NuGetNext-All block actually changed package refs.
# -----------------------
if (-not $PostUpdateRefsChanged) {
  Write-Host "Skipping HEAD-based PackageVersion bump because Update-NuGetNext-All block did not change package-reference versions."
} elseif (Test-Path -LiteralPath $sauceryCoreCsprojPath) {
  $currentRefs = Get-PackageReferencesFromPath -path $sauceryCoreCsprojPath
  $currentRefsNorm = Normalize-PackageRefs -map $currentRefs

  $gitCmd = Get-Command git -ErrorAction SilentlyContinue
  if ($gitCmd) {
    $relPath = 'Saucery.Core/Saucery.Core.csproj' -replace '\\','/'
    try {
      $headText = & $gitCmd.Path -C $Repo show "HEAD:$relPath" 2>$null
      if ($LASTEXITCODE -eq 0 -and $headText) {
        $headRefs = Get-PackageReferencesFromXml -xmlText $headText
        $headRefsNorm = Normalize-PackageRefs -map $headRefs

        $headDiff = $false
        $allKeys = @($currentRefsNorm.Keys + $headRefsNorm.Keys) | Sort-Object -Unique
        foreach ($k in $allKeys) {
          $c = $null; $h = $null
          if ($currentRefsNorm.ContainsKey($k)) { $c = $currentRefsNorm[$k] }
          if ($headRefsNorm.ContainsKey($k))    { $h = $headRefsNorm[$k] }
          if ($c -ne $h) { $headDiff = $true; break }
        }

        if ($headDiff -and -not $SauceryCoreBumped) {
          Write-Host "Detected PackageReference differences vs HEAD. Bumping PackageVersion based on HEAD package version."
          Write-PackageRefDiffs -beforeNorm $headRefsNorm -afterNorm $currentRefsNorm -context "HEAD vs working copy"

          $headVersion = Get-PackageVersionFromXmlText -xmlText $headText
          if (-not $headVersion) {
            Write-Warning "HEAD copy of Saucery.Core.csproj does not contain a <PackageVersion>; skipping HEAD-based bump."
          } else {
            $newVersion = Bump-VersionString -baseVersion $headVersion
            if (-not $newVersion) {
              Write-Warning "Failed to compute bumped version from HEAD base '$headVersion'."
            } else {
              if (Replace-PackageVersionInPath -path $sauceryCoreCsprojPath -newVersion $newVersion) {
                Write-Host "Saucery.Core.csproj PackageVersion updated to $newVersion (HEAD-based bump)."
                $SauceryCoreBumped = $true
              } else {
                Write-Warning "Failed to write HEAD-based bumped PackageVersion to $sauceryCoreCsprojPath"
              }
            }
          }
        } else {
          if ($headDiff -and $SauceryCoreBumped) {
            Write-Host "PackageReference difference vs HEAD detected but Saucery.Core was already bumped by post-update block; skipping."
          } else {
            Write-Host "No PackageReference differences vs HEAD; no HEAD-based bump."
          }
        }
      } else {
        Write-Host "No HEAD copy available for Saucery.Core.csproj; skipping HEAD comparison."
      }
    } catch {
      Write-Warning "Error while comparing Saucery.Core.csproj with HEAD: $_"
    }
  } else {
    Write-Host "git not available; skipping HEAD comparison."
  }
}

# ===================== Continue with rest of the script =====================
.\Update-NuGetNext-All.ps1 -PackageId 'BenchmarkDotNet'                     -Root (Join-Path $Repo 'Saucery.Benchmark')
.\Update-NuGetNext-All.ps1 -PackageId 'BenchmarkDotNet.Diagnostics.Windows' -Root (Join-Path $Repo 'Saucery.Benchmark')
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery.Core'                        -Root (Join-Path $Repo 'Saucery.Benchmark')

.\Update-NuGetNext-All.ps1 -PackageId 'NUnit'                  -Root (Join-Path $Repo 'Saucery')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk' -Root (Join-Path $Repo 'Saucery')
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit3TestAdapter'      -Root (Join-Path $Repo 'Saucery')

.\Update-NuGetNext-All.ps1 -PackageId 'TUnit' -Root (Join-Path $Repo 'Saucery.TUnit')

.\Update-NuGetNext-All.ps1 -PackageId 'Meziantou.Xunit.ParallelTestFramework' -Root (Join-Path $Repo 'Saucery.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Saucery.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit'                                 -Root (Join-Path $Repo 'Saucery.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio'             -Root (Join-Path $Repo 'Saucery.XUnit')

.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Saucery.XUnit3')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.v3.mtp-v2'                       -Root (Join-Path $Repo 'Saucery.XUnit3')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio'             -Root (Join-Path $Repo 'Saucery.XUnit3')

.\Update-NuGetNext-All.ps1 -PackageId 'TUnit'                                     -Root (Join-Path $Repo 'Saucery.Core.Tests')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.CodeCoverage' -Root (Join-Path $Repo 'Saucery.Core.Tests')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.TrxReport'    -Root (Join-Path $Repo 'Saucery.Core.Tests')

.\Update-NuGetNext-All.ps1 -PackageId 'TUnit'                                               -Root (Join-Path $Repo 'Template.Tests')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.CodeCoverage'           -Root (Join-Path $Repo 'Template.Tests')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.TrxReport'              -Root (Join-Path $Repo 'Template.Tests')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.TemplateEngine.Authoring.TemplateVerifier' -Root (Join-Path $Repo 'Template.Tests')
.\Update-NuGetNext-All.ps1 -PackageId 'Shouldly'                                            -Root (Join-Path $Repo 'Template.Tests')

# ===================== ADDED to match previous script =====================

# MERLINS
# Merlin.NUnit
.\Update-NuGetNext-All.ps1 -PackageId 'DotNetSeleniumExtras.PageObjects.Core' -Root (Join-Path $Repo 'Merlin.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Merlin.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Web.Xdt'                     -Root (Join-Path $Repo 'Merlin.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit3TestAdapter'                     -Root (Join-Path $Repo 'Merlin.NUnit')

# Merlin.NUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'coverlet.msbuild'                      -Root (Join-Path $Repo 'Merlin.NUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'DotNetSeleniumExtras.PageObjects.Core' -Root (Join-Path $Repo 'Merlin.NUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Merlin.NUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Web.Xdt'                     -Root (Join-Path $Repo 'Merlin.NUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit3TestAdapter'                     -Root (Join-Path $Repo 'Merlin.NUnit.RealDevices')

# Merlin.Playwright.NUnit
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'      -Root (Join-Path $Repo 'Merlin.Playwright.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit'                       -Root (Join-Path $Repo 'Merlin.Playwright.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit3TestAdapter'           -Root (Join-Path $Repo 'Merlin.Playwright.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Playwright.NUnit'  -Root (Join-Path $Repo 'Merlin.Playwright.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.SourceLink.GitHub' -Root (Join-Path $Repo 'Merlin.Playwright.NUnit')
#.\Update-NuGetNext-All.ps1 -PackageId 'NUnit.Analyzers'        -Root (Join-Path $Repo 'Merlin.Playwright.NUnit')
#.\Update-NuGetNext-All.ps1 -PackageId 'coverlet.collector'     -Root (Join-Path $Repo 'Merlin.Playwright.NUnit')

# Merlin.TUnit
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.CodeCoverage' -Root (Join-Path $Repo 'Merlin.TUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.TrxReport'    -Root (Join-Path $Repo 'Merlin.TUnit')

# Merlin.TUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.CodeCoverage' -Root (Join-Path $Repo 'Merlin.TUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.TrxReport'    -Root (Join-Path $Repo 'Merlin.TUnit.RealDevices')

# Merlin.XUnit
.\Update-NuGetNext-All.ps1 -PackageId 'Meziantou.Xunit.ParallelTestFramework' -Root (Join-Path $Repo 'Merlin.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Merlin.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'coverlet.collector'                    -Root (Join-Path $Repo 'Merlin.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'coverlet.msbuild'                      -Root (Join-Path $Repo 'Merlin.XUnit')

# Merlin.XUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'Meziantou.Xunit.ParallelTestFramework' -Root (Join-Path $Repo 'Merlin.XUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Merlin.XUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'coverlet.collector'                    -Root (Join-Path $Repo 'Merlin.XUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'coverlet.msbuild'                      -Root (Join-Path $Repo 'Merlin.XUnit.RealDevices')

# Merlin.XUnit3
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                    -Root (Join-Path $Repo 'Merlin.XUnit3')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.CodeCoverage' -Root (Join-Path $Repo 'Merlin.XUnit3')

# Merlin.XUnit3.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                    -Root (Join-Path $Repo 'Merlin.XUnit3.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.Testing.Extensions.CodeCoverage' -Root (Join-Path $Repo 'Merlin.XUnit3.RealDevices')

# EXTERNALMERLINS
# ExternalMerlin.NUnit
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit3TestAdapter'      -Root (Join-Path $Repo 'ExternalMerlin.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery'                -Root (Join-Path $Repo 'ExternalMerlin.NUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk' -Root (Join-Path $Repo 'ExternalMerlin.NUnit')

# ExternalMerlin.NUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit3TestAdapter'      -Root (Join-Path $Repo 'ExternalMerlin.NUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery'                -Root (Join-Path $Repo 'ExternalMerlin.NUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk' -Root (Join-Path $Repo 'ExternalMerlin.NUnit.RealDevices')

# ExternalMerlin.TUnit
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery.TUnit' -Root (Join-Path $Repo 'ExternalMerlin.TUnit')

# ExternalMerlin.TUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery.TUnit' -Root (Join-Path $Repo 'ExternalMerlin.TUnit.RealDevices')

# ExternalMerlin.XUnit
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio' -Root (Join-Path $Repo 'ExternalMerlin.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery.XUnit'             -Root (Join-Path $Repo 'ExternalMerlin.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'    -Root (Join-Path $Repo 'ExternalMerlin.XUnit')

# ExternalMerlin.XUnit.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio' -Root (Join-Path $Repo 'ExternalMerlin.XUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery.XUnit'             -Root (Join-Path $Repo 'ExternalMerlin.XUnit.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'    -Root (Join-Path $Repo 'ExternalMerlin.XUnit.RealDevices')

# ExternalMerlin.XUnit3
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio' -Root (Join-Path $Repo 'ExternalMerlin.XUnit3')
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery.XUnit.v3'          -Root (Join-Path $Repo 'ExternalMerlin.XUnit3')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'    -Root (Join-Path $Repo 'ExternalMerlin.XUnit3')

# ExternalMerlin.XUnit3.RealDevices
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio' -Root (Join-Path $Repo 'ExternalMerlin.XUnit3.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Saucery.XUnit.v3'          -Root (Join-Path $Repo 'ExternalMerlin.XUnit3.RealDevices')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'    -Root (Join-Path $Repo 'ExternalMerlin.XUnit3.RealDevices')

# ===================== Run child project syncs =====================
Write-Host ("Running syncs from {0} ..." -f $Repo)
foreach ($p in $Projects) { Invoke-Sync -Project $p -Repo $Repo -ScriptName $ScriptName }

# ===================== Template sync (after projects) =====================

$TemplateSpecs = @(
  @{ Kind='NUnit';  ProjectRoot=(Join-Path $Repo 'Saucery');
     TemplatePath=(Join-Path $Root -ChildPath (Join-Path 'Templates' (Join-Path 'NUnit' 'MyTestProject.csproj')));
     Packages=@('Saucery','NUnit','Microsoft.NET.Test.Sdk','NUnit3TestAdapter') },

  @{ Kind='TUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.TUnit');
     TemplatePath=(Join-Path $Root -ChildPath (Join-Path 'Templates' (Join-Path 'TUnit' 'MyTestProject.csproj')));
     Packages=@('Saucery.TUnit','Microsoft.NET.Test.Sdk') },

  @{ Kind='XUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.XUnit');
     TemplatePath=(Join-Path $Root -ChildPath (Join-Path 'Templates' (Join-Path 'XUnit' 'MyTestProject.csproj')));
     Packages=@('Saucery.XUnit','xunit','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk','Meziantou.Xunit.ParallelTestFramework') },

  @{ Kind='XUnit3'; ProjectRoot=(Join-Path $Repo 'Saucery.XUnit3');
     TemplatePath=(Join-Path $Root -ChildPath (Join-Path 'Templates' (Join-Path 'XUnit3' 'MyTestProject.csproj')));
     Packages=@('Saucery.XUnit.v3','xunit.v3.mtp-v2','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk') }
)

Write-Host 'Syncing templates (after project updates) ...'
foreach($spec in $TemplateSpecs){
  $versions = @{}
  foreach($pkg in $spec.Packages){
    $v = Get-PackageVersionFromRoot -ProjectRoot $spec.ProjectRoot -PackageId $pkg
    if ($v) { $versions[$pkg] = $v }
  }
  if ($versions.Count -eq 0) {
    Write-Warning ("  {0}: no package versions discovered; template unchanged." -f $spec.Kind)
    continue
  }
  Write-Host ("- {0}: applying discovered versions to template" -f $spec.Kind)
  Set-TemplatePackages -TemplatePath $spec.TemplatePath -PackageVersions $versions | Out-Null
}

# ===================== Snapshot verified files sync (last) =====================

$SnapshotSpecs = @(
  @{ Kind='NUnit';  ProjectRoot=(Join-Path $Repo 'Saucery');
     SnapshotPath=(Join-Path $Root -ChildPath (Join-Path 'Template.Tests' (Join-Path 'Snapshots' (Join-Path 'GeneratesExpectedNUnitProject.saucery-nunit.--name#MyTestProject.verified' (Join-Path 'saucery-nunit' 'MyTestProject.csproj')))));
     Packages=@('Saucery','NUnit','Microsoft.NET.Test.Sdk','NUnit3TestAdapter') },

  @{ Kind='XUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.XUnit');
     SnapshotPath=(Join-Path $Root -ChildPath (Join-Path 'Template.Tests' (Join-Path 'Snapshots' (Join-Path 'GeneratesExpectedXUnitProject.saucery-xunit.--name#MyTestProject.verified' (Join-Path 'saucery-xunit' 'MyTestProject.csproj')))));
     Packages=@('Saucery.XUnit','xunit','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk','Meziantou.Xunit.ParallelTestFramework') },

  @{ Kind='TUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.TUnit');
     SnapshotPath=(Join-Path $Root -ChildPath (Join-Path 'Template.Tests' (Join-Path 'Snapshots' (Join-Path 'GeneratesExpectedTUnitProject.saucery-tunit.--name#MyTestProject.verified' (Join-Path 'saucery-tunit' 'MyTestProject.csproj')))));
     Packages=@('Saucery.TUnit','Microsoft.NET.Test.Sdk') },

  @{ Kind='XUnit3'; ProjectRoot=(Join-Path $Repo 'Saucery.XUnit3');
     SnapshotPath=(Join-Path $Root -ChildPath (Join-Path 'Template.Tests' (Join-Path 'Snapshots' (Join-Path 'GeneratesExpectedXUnit3Project.saucery-xunit3.--name#MyTestProject.verified' (Join-Path 'saucery-xunit3' 'MyTestProject.csproj')))));
     Packages=@('Saucery.XUnit.v3','xunit.v3.mtp-v2','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk') }
)

Write-Host 'Syncing snapshot verified files (final step) ...'
foreach($spec in $SnapshotSpecs){
  $versions = @{}
  foreach($pkg in $spec.Packages){
    $v = Get-PackageVersionFromRoot -ProjectRoot $spec.ProjectRoot -PackageId $pkg
    if ($v) { $versions[$pkg] = $v }
  }
  if ($versions.Count -eq 0) {
    Write-Warning ("  {0}: no package versions discovered; snapshot unchanged." -f $spec.Kind)
    continue
  }
  Write-Host ("- {0}: applying discovered versions to snapshot" -f $spec.Kind)
  Set-TemplatePackages -TemplatePath $spec.SnapshotPath -PackageVersions $versions | Out-Null
}

Write-Host 'All done!'
