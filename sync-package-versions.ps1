param(
    [string]$Root = (Split-Path -Parent $MyInvocation.MyCommand.Path),
    [string]$ScriptName = 'sync-package-version.ps1'
)

$Repo = 'C:\gitrepos\Saucery'

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

  # 2) Explicit PackageReference in a csproj
  $proj = Get-ChildItem -Path $ProjectRoot -Recurse -Filter '*.csproj' -File -ErrorAction SilentlyContinue
  foreach($p in $proj){
    try {
      [xml]$x = Get-Content -Raw -LiteralPath $p.FullName
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
function Invoke-Sync([string]$Project,[string]$Root,[string]$ScriptName) {
    $projectDir = Join-Path $Root $Project
    $scriptPath = Join-Path $projectDir $ScriptName
    if (-not (Test-Path -LiteralPath $projectDir)) { Write-Error "Project dir not found: $projectDir"; return }
    if (-not (Test-Path -LiteralPath $scriptPath)) { Write-Error "Script not found: $scriptPath";   return }
    Write-Host ("-> {0}" -f $Project)
    Write-Host ("   Script : {0}" -f $scriptPath)
    $exe  = Get-PowerShellExe
    $args = @('-NoProfile','-ExecutionPolicy','Bypass','-File', $scriptPath)
    try { Push-Location $projectDir; & $exe @args; $code = $LASTEXITCODE }
    catch { Write-Error ("X Failed {0}: {1}" -f $Project, $_.Exception.Message); return }
    finally { Pop-Location }
    if ($code -ne 0) { Write-Error ("X Failed {0}: child exited with code {1}" -f $Project, $code); return }
    Write-Host ("OK Synced {0}" -f $Project)
}

$Projects = @('Saucery','Saucery.Playwright.NUnit','Saucery.XUnit','Saucery.XUnit3','Saucery.TUnit')

cls

# ===================== Pre-update bumps (same as before) =====================

.\Update-NuGetNext-All.ps1 -PackageId 'Appium.WebDriver'                     -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Castle.Core'                           -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'DotNetSeleniumExtras.PageObjects.Core' -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'DotNetSeleniumExtras.WaitHelpers'      -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'RestSharp'                             -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Selenium.Support'                      -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Selenium.WebDriver'                    -Root (Join-Path $Repo 'Saucery.Core')
.\Update-NuGetNext-All.ps1 -PackageId 'Shouldly'                              -Root (Join-Path $Repo 'Saucery.Core')

.\Update-NuGetNext-All.ps1 -PackageId 'TUnit' -Root (Join-Path $Repo 'Saucery.Core.Tests')

.\Update-NuGetNext-All.ps1 -PackageId 'NUnit'                  -Root (Join-Path $Repo 'Saucery')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk' -Root (Join-Path $Repo 'Saucery')
.\Update-NuGetNext-All.ps1 -PackageId 'NUnit3TestAdapter'      -Root (Join-Path $Repo 'Saucery')

.\Update-NuGetNext-All.ps1 -PackageId 'TUnit' -Root (Join-Path $Repo 'Saucery.TUnit')

.\Update-NuGetNext-All.ps1 -PackageId 'Meziantou.Xunit.ParallelTestFramework' -Root (Join-Path $Repo 'Saucery.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Saucery.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit'                                 -Root (Join-Path $Repo 'Saucery.XUnit')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio'             -Root (Join-Path $Repo 'Saucery.XUnit')

.\Update-NuGetNext-All.ps1 -PackageId 'Microsoft.NET.Test.Sdk'                -Root (Join-Path $Repo 'Saucery.XUnit3')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.v3'                              -Root (Join-Path $Repo 'Saucery.XUnit3')
.\Update-NuGetNext-All.ps1 -PackageId 'xunit.runner.visualstudio'             -Root (Join-Path $Repo 'Saucery.XUnit3')

# (Merlin / ExternalMerlin blocks can remain here if you use them)

# ===================== Run child project syncs =====================
Write-Host ("Running syncs from {0} ..." -f $Root)
foreach ($p in $Projects) { Invoke-Sync -Project $p -Root $Root -ScriptName $ScriptName }

# ===================== Template sync (after projects) =====================

$TemplateSpecs = @(
  @{ Kind='NUnit';  ProjectRoot=(Join-Path $Repo 'Saucery');
     TemplatePath=(Join-Path $Root 'Templates\NUnit\MyTestProject.csproj');
     Packages=@('Saucery','NUnit','Microsoft.NET.Test.Sdk','NUnit3TestAdapter') },

  @{ Kind='TUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.TUnit');
     TemplatePath=(Join-Path $Root 'Templates\TUnit\MyTestProject.csproj');
     Packages=@('Saucery.TUnit','Microsoft.NET.Test.Sdk') },

  @{ Kind='XUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.XUnit');
     TemplatePath=(Join-Path $Root 'Templates\XUnit\MyTestProject.csproj');
     Packages=@('Saucery.XUnit','xunit','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk','Meziantou.Xunit.ParallelTestFramework') },

  @{ Kind='XUnit3'; ProjectRoot=(Join-Path $Repo 'Saucery.XUnit3');
     TemplatePath=(Join-Path $Root 'Templates\XUnit3\MyTestProject.csproj');
     Packages=@('Saucery.XUnit.v3','xunit.v3','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk') }
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
     SnapshotPath=(Join-Path $Root 'Template.Tests\Snapshots\GeneratesExpectedNUnitProject.saucery-nunit.--name#MyTestProject.verified\saucery-nunit\MyTestProject.csproj');
     Packages=@('Saucery','NUnit','Microsoft.NET.Test.Sdk','NUnit3TestAdapter') },

  @{ Kind='XUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.XUnit');
     SnapshotPath=(Join-Path $Root 'Template.Tests\Snapshots\GeneratesExpectedXUnitProject.saucery-xunit.--name#MyTestProject.verified\saucery-xunit\MyTestProject.csproj');
     Packages=@('Saucery.XUnit','xunit','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk','Meziantou.Xunit.ParallelTestFramework') },

  @{ Kind='TUnit';  ProjectRoot=(Join-Path $Repo 'Saucery.TUnit');
     SnapshotPath=(Join-Path $Root 'Template.Tests\Snapshots\GeneratesExpectedTUnitProject.saucery-tunit.--name#MyTestProject.verified\saucery-tunit\MyTestProject.csproj');
     Packages=@('Saucery.TUnit','Microsoft.NET.Test.Sdk') },

  @{ Kind='XUnit3'; ProjectRoot=(Join-Path $Repo 'Saucery.XUnit3');
     SnapshotPath=(Join-Path $Root 'Template.Tests\Snapshots\GeneratesExpectedXUnit3Project.saucery-xunit3.--name#MyTestProject.verified\saucery-xunit3\MyTestProject.csproj');
     Packages=@('Saucery.XUnit.v3','xunit.v3','xunit.runner.visualstudio','Microsoft.NET.Test.Sdk') }
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
