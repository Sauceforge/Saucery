param(
    [string]$TargetCsprojPath = "Saucery.Playwright.NUnit.csproj",
    [string]$CoreCsprojPath   = "../Saucery.Core/Saucery.Core.csproj"
)

# ---------- helpers ----------
function Resolve-ExistingFile([string]$p) {
    if (-not (Test-Path -LiteralPath $p)) { throw "File not found: $p" }
    (Resolve-Path -LiteralPath $p).Path
}

function Read-FileWithEncoding([string]$path) {
    [byte[]]$bytes = [System.IO.File]::ReadAllBytes($path)

    # Prepare encodings
    $utf8NoBomStrict = New-Object System.Text.UTF8Encoding($false, $true)
    $utf8NoBom       = New-Object System.Text.UTF8Encoding($false)
    $utf8WithBom     = New-Object System.Text.UTF8Encoding($true)
    $utf16LE         = [System.Text.Encoding]::Unicode
    $utf16BE         = [System.Text.Encoding]::BigEndianUnicode
    $win1252         = [System.Text.Encoding]::GetEncoding(1252)

    $decodeEncoding = $null
    $writeEncoding  = $null
    $preambleLen    = 0

    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        $decodeEncoding = $utf8NoBom
        $writeEncoding  = $utf8WithBom
        $preambleLen    = 3
    }
    elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
        $decodeEncoding = $utf16LE
        $writeEncoding  = $utf16LE
        $preambleLen    = 2
    }
    elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
        $decodeEncoding = $utf16BE
        $writeEncoding  = $utf16BE
        $preambleLen    = 2
    }
    else {
        try {
            [void]$utf8NoBomStrict.GetString($bytes)
            $decodeEncoding = $utf8NoBom
            $writeEncoding  = $utf8NoBom
            $preambleLen    = 0
        }
        catch {
            $decodeEncoding = $win1252
            $writeEncoding  = $win1252
            $preambleLen    = 0
        }
    }

    $text = $decodeEncoding.GetString($bytes, $preambleLen, $bytes.Length - $preambleLen)
    [pscustomobject]@{
        Bytes          = $bytes
        Text           = $text
        DecodeEncoding = $decodeEncoding
        WriteEncoding  = $writeEncoding
        PreambleLen    = $preambleLen
    }
}

function Get-CorePackageVersion([string]$xmlText) {
    $xml = New-Object System.Xml.XmlDocument
    $xml.PreserveWhitespace = $true
    try { $xml.LoadXml($xmlText) } catch {
        throw "Core project file is not valid XML: $($_.Exception.Message)"
    }

    # Namespace-agnostic lookup for <PackageVersion>
    $n = $xml.SelectSingleNode("/*[local-name()='Project']/*[local-name()='PropertyGroup']/*[local-name()='PackageVersion']")
    if ($n -and -not [string]::IsNullOrWhiteSpace($n.InnerText)) {
        return $n.InnerText.Trim()
    }
    throw "Could not find <PackageVersion> in '$CoreCsprojPath'."
}

# ---------- main ----------
try {
    $TargetCsprojPath = Resolve-ExistingFile $TargetCsprojPath
    $CoreCsprojPath   = Resolve-ExistingFile $CoreCsprojPath
}
catch {
    Write-Error $_.Exception.Message
    exit 1
}

# Read core csproj to get PackageVersion
$core = Read-FileWithEncoding $CoreCsprojPath
try {
    $coreVersion = Get-CorePackageVersion $core.Text
} catch {
    Write-Error $_.Exception.Message
    exit 1
}
if ([string]::IsNullOrWhiteSpace($coreVersion)) {
    Write-Error "Determined core <PackageVersion> is empty."
    exit 1
}

# Read target csproj (Saucery.csproj)
$target = Read-FileWithEncoding $TargetCsprojPath
$originalText = $target.Text

# Replace FIRST <PackageVersion>...</PackageVersion> in ORIGINAL TEXT (surgical)
$pattern = '(?s)(<\s*PackageVersion\s*>)(.*?)(<\s*/\s*PackageVersion\s*>)'
$re      = New-Object System.Text.RegularExpressions.Regex($pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)
$match   = $re.Match($originalText)

if (-not $match.Success) {
    Write-Error "Target file does not contain a <PackageVersion> property."
    exit 1
}

$before = $match.Groups[2].Value
if ($before -eq $coreVersion) {
    Write-Host "No change needed. <PackageVersion> already '$coreVersion'."
    exit 0
}

$prefix  = $originalText.Substring(0, $match.Index)
$middle  = $match.Groups[1].Value + $coreVersion + $match.Groups[3].Value
$suffix  = $originalText.Substring($match.Index + $match.Length)
$newText = $prefix + $middle + $suffix

# Ensure writable
$item = Get-Item -LiteralPath $TargetCsprojPath
if ($item.Attributes -band [IO.FileAttributes]::ReadOnly) { attrib -r $TargetCsprojPath | Out-Null }

# Write back with EXACT original encoding (including BOM if present)
[byte[]]$contentBytes = $target.WriteEncoding.GetBytes($newText)
[byte[]]$preamble     = $target.WriteEncoding.GetPreamble()
if ($preamble -and $preamble.Length -gt 0) {
    $outBytes = New-Object byte[] ($preamble.Length + $contentBytes.Length)
    [Array]::Copy($preamble, 0, $outBytes, 0, $preamble.Length)
    [Array]::Copy($contentBytes, 0, $outBytes, $preamble.Length, $contentBytes.Length)
    [System.IO.File]::WriteAllBytes($TargetCsprojPath, $outBytes)
} else {
    [System.IO.File]::WriteAllBytes($TargetCsprojPath, $contentBytes)
}

# Verify
[byte[]]$verifyBytes  = [System.IO.File]::ReadAllBytes($TargetCsprojPath)
$p2 = $target.WriteEncoding.GetPreamble()
$pLen2 = 0
if ($p2 -and $verifyBytes.Length -ge $p2.Length) { $pLen2 = $p2.Length }
$verifyText = $target.WriteEncoding.GetString($verifyBytes, $pLen2, $verifyBytes.Length - $pLen2)

$vm = $re.Match($verifyText)
$after = $null
if ($vm.Success) { $after = $vm.Groups[2].Value }

if ($after -ne $coreVersion) {
    Write-Error "Save verification failed. Expected '$coreVersion', found '$after'."
    exit 1
}

Write-Host "Updated <PackageVersion> $before -> $coreVersion in '$TargetCsprojPath' (encoding & BOM preserved)."
