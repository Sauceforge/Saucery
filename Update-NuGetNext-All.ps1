[CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact='Low')]
param(
  [Parameter(Mandatory=$true)][string]$PackageId,
  [string]$Root = (Get-Location).Path,
  [switch]$IncludePrerelease
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

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
function Select-NodesNs([xml]$Xml,[string]$WithNs,[string]$NoNs){
  $ns = New-NamespaceManager $Xml
  if ($Xml.DocumentElement.NamespaceURI) { $Xml.SelectNodes($WithNs, $ns) } else { $Xml.SelectNodes($NoNs) }
}
function Find-PackageNodes([xml]$Xml,[string]$PackageId){
  $nodes=@()
  $cpm = Select-SingleNodeNs $Xml "//msbuild:PackageVersion[@Include='$PackageId']" "//*[local-name()='PackageVersion' and @Include='$PackageId']"
  if ($cpm) { $nodes += @{ Node=$cpm; Kind='CPM' } }
  $refs = Select-NodesNs $Xml "//msbuild:PackageReference[@Include='$PackageId' and @Version]" "//*[local-name()='PackageReference' and @Include='$PackageId' and @Version]"
  if ($refs) { foreach($n in $refs){ $nodes += @{ Node=$n; Kind='Csproj' } } }
  $nodes
}

function Is-Prerelease([string]$v){ $v -match '-' }
function To-DotNetVersion([string]$v){
  $core=$v.Split('-')[0]; $p=$core.Split('.'); $a=@(0,0,0,0)
  for($i=0;$i -lt [Math]::Min(4,$p.Count);$i++){ [int]$a[$i]=([int]($p[$i] -as [int])) }
  New-Object System.Version $a[0],$a[1],$a[2],$a[3]
}
function Get-NextVersion([string]$current,[string[]]$available,[switch]$IncludePrerelease){
  $filtered = if($IncludePrerelease){$available}else{$available | Where-Object { -not (Is-Prerelease $_) }}
  if(-not $filtered){return $null}
  $cur=To-DotNetVersion $current
  foreach($v in ($filtered | Sort-Object { To-DotNetVersion $_ })){
    if((To-DotNetVersion $v).CompareTo($cur) -gt 0){ return $v }
  }
  $null
}

function Get-AvailableVersions([string]$packageId){
  $url = "https://api.nuget.org/v3-flatcontainer/$($packageId.ToLowerInvariant())/index.json"
  try {
    $resp = Invoke-RestMethod -Method Get -Uri $url -TimeoutSec 30
    if(-not $resp.versions){ throw "No versions returned." }
    [string[]]$resp.versions
  } catch {
    throw "Failed to query nuget.org for '$packageId': $($_.Exception.Message)"
  }
}

# --- Encoding-preserving IO ---
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

# --- Safe, linear in-place edit (no catastrophic regex) ---
function Update-VersionValue-InStartTag {
  param(
    [string]$Text,
    [string]$PackageId,
    [string]$NewVersion
  )
  $pkgPattern = [regex]::new('Include\s*=\s*"'+[regex]::Escape($PackageId)+'"', 'IgnoreCase')
  $m = $pkgPattern.Match($Text)
  if(-not $m.Success){ return $Text, $false }

  # Find the start '<' of the tag and the next '>' to get the start-tag slice
  $start = $Text.LastIndexOf('<', $m.Index)
  if($start -lt 0){ return $Text, $false }
  $end = $Text.IndexOf('>', $m.Index)
  if($end -lt 0){ return $Text, $false }

  $tag = $Text.Substring($start, $end - $start + 1)

  # Replace Version="..." inside ONLY this tag; simple, non-catastrophic regex
  $verRegex = [regex]::new('\bVersion\s*=\s*"([^"]*)"', 'IgnoreCase')
  if(-not $verRegex.IsMatch($tag)){ return $Text, $false }

  $newTag = $verRegex.Replace($tag, { param($mm) 'Version="'+$NewVersion+'"' }, 1)

  if($newTag -eq $tag){ return $Text, $false }

  $newText = $Text.Substring(0,$start) + $newTag + $Text.Substring($end+1)
  return $newText, $true
}

# --- main ---
$files = @()
$files += Get-ChildItem -Path $Root -Recurse -Filter Directory.Packages.props -File -ErrorAction SilentlyContinue
$files += Get-ChildItem -Path $Root -Recurse -Filter *.csproj -File -ErrorAction SilentlyContinue
if(-not $files){ throw "No Directory.Packages.props or .csproj files found under '$Root'." }

$allVersions = Get-AvailableVersions -packageId $PackageId
$changes = New-Object System.Collections.Generic.List[object]

foreach($file in $files){
  # Read preserving encoding/BOM
  $rf = Read-FileWithEncoding -path $file.FullName
  $text = $rf.Text
  $encFactory = $rf.Encoder

  # Parse (read-only) just to discover current version(s)
  try { [xml]$xml = $text } catch { Write-Warning "Skipping unreadable XML: $($file.FullName) ($($_.Exception.Message))"; continue }
  $nodes = Find-PackageNodes -Xml $xml -PackageId $PackageId
  if(-not $nodes){ continue }

  $newText = $text
  $modified = $false

  foreach($info in $nodes){
    $current = $info.Node.GetAttribute('Version')
    if([string]::IsNullOrWhiteSpace($current)){ continue }
    $next = Get-NextVersion -current $current -available $allVersions -IncludePrerelease:$IncludePrerelease
    if(-not $next -or $next -eq $current){ continue }

    # Record proposed change (so -WhatIf still prints summary)
    $changes.Add([pscustomobject]@{
      File        = $file.FullName
      PackageId   = $PackageId
      FromVersion = $current
      ToVersion   = $next
      Kind        = $info.Kind
    })

    if($PSCmdlet.ShouldProcess($file.FullName, "Update $PackageId $current -> $next")){
      $res = Update-VersionValue-InStartTag -Text $newText -PackageId $PackageId -NewVersion $next
      $newText = $res[0]; $did = $res[1]
      if($did){ $modified = $true }
    }
  }

  if($modified -and $PSCmdlet.ShouldProcess($file.FullName, "Save updated text (preserve encoding/BOM)")){
    Write-FileWithEncoding -path $file.FullName -text $newText -EncoderFactory $encFactory
  }
}

if($changes.Count -eq 0){
  Write-Host "No updates found/applicable. Either no explicit Version attributes were found, or no version newer than your current pin exists." -ForegroundColor Yellow
  return
}

$changes | Sort-Object File, Kind | Format-Table -AutoSize
$changes | ForEach-Object {
  Write-Output "WOULD UPDATE `"$($_.PackageId)`" in `"$($_.File)`": $($_.FromVersion) -> $($_.ToVersion) [$($_.Kind)]"
}
