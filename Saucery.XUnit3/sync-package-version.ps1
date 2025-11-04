param(
    [string]$CsprojPath = "Saucery.XUnit3.csproj"
)

# --- Resolve path & existence ---
if (-not (Test-Path -LiteralPath $CsprojPath)) {
    Write-Error "File not found: $CsprojPath"
    exit 1
}
$CsprojPath = (Resolve-Path -LiteralPath $CsprojPath).Path

# --- Read original bytes ---
[byte[]]$bytes = [System.IO.File]::ReadAllBytes($CsprojPath)

# --- Prepare encodings ---
$utf8NoBomStrict = New-Object System.Text.UTF8Encoding($false, $true) # throws on invalid
$utf8NoBom       = New-Object System.Text.UTF8Encoding($false)
$utf8WithBom     = New-Object System.Text.UTF8Encoding($true)
$utf16LE         = [System.Text.Encoding]::Unicode
$utf16BE         = [System.Text.Encoding]::BigEndianUnicode
$win1252         = [System.Text.Encoding]::GetEncoding(1252)

# --- Detect original encoding & BOM ---
$decodeEncoding = $null     # for decoding text
$writeEncoding  = $null     # for writing back
$preambleLen    = 0

if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
    # UTF-8 WITH BOM
    $decodeEncoding = $utf8NoBom    # decode skipping BOM
    $writeEncoding  = $utf8WithBom  # IMPORTANT: re-emit BOM on write
    $preambleLen    = 3
}
elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
    # UTF-16 LE WITH BOM
    $decodeEncoding = $utf16LE
    $writeEncoding  = $utf16LE      # GetPreamble() will re-emit BOM
    $preambleLen    = 2
}
elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
    # UTF-16 BE WITH BOM
    $decodeEncoding = $utf16BE
    $writeEncoding  = $utf16BE
    $preambleLen    = 2
}
else {
    # No BOM: prefer strict UTF-8 if valid, else treat as Windows-1252
    try {
        [void]$utf8NoBomStrict.GetString($bytes)
        $decodeEncoding = $utf8NoBom
        $writeEncoding  = $utf8NoBom  # no BOM
        $preambleLen    = 0
    }
    catch {
        $decodeEncoding = $win1252
        $writeEncoding  = $win1252
        $preambleLen    = 0
    }
}

# --- Decode original text (keep EOLs as-is) ---
$originalText = $decodeEncoding.GetString($bytes, $preambleLen, $bytes.Length - $preambleLen)

# --- Parse XML only to READ Xunit.v3 version ---
$xml = New-Object System.Xml.XmlDocument
$xml.PreserveWhitespace = $true
try { $xml.LoadXml($originalText) } catch {
    Write-Error "The project file is not valid XML: $($_.Exception.Message)"
    exit 1
}

# Collect PackageReferences
$pkgRefs = @()
foreach ($ig in $xml.Project.ItemGroup) {
    if ($ig -and $ig.PackageReference) { $pkgRefs += $ig.PackageReference }
}

$xunit3 = $pkgRefs | Where-Object { $_.Include -eq 'xunit.v3.mtp-v2' } | Select-Object -First 1
if (-not $xunit3) { Write-Error "XUnit3 PackageReference not found."; exit 1 }

# Prefer Version attribute, then <Version> child
$xunit3Version = $null
if ($xunit3.Attributes['Version']) { $xunit3Version = $xunit3.Attributes['Version'].Value }
if ([string]::IsNullOrWhiteSpace($xunit3Version)) {
    $verNode = $xunit3.SelectSingleNode('Version')
    if ($verNode -and -not [string]::IsNullOrWhiteSpace($verNode.InnerText)) {
        $xunit3Version = $verNode.InnerText
    }
}
if ([string]::IsNullOrWhiteSpace($xunit3Version)) {
    Write-Error "Could not determine XUnit3 version (no Version=... attribute or <Version> child)."
    exit 1
}

# --- Replace FIRST <PackageVersion>...</PackageVersion> in ORIGINAL TEXT (surgical) ---
$pattern = '(?s)(<\s*PackageVersion\s*>)(.*?)(<\s*/\s*PackageVersion\s*>)'
$re      = New-Object System.Text.RegularExpressions.Regex($pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)
$match   = $re.Match($originalText)

if (-not $match.Success) { Write-Error "<PackageVersion> property not found in the file."; exit 1 }

$before  = $match.Groups[2].Value
if ($before -eq $xunit3Version) {
    Write-Host "No change needed. <PackageVersion> already $xunit3Version"
    exit 0
}

# Manual single-replace to avoid touching anything else
$prefix  = $originalText.Substring(0, $match.Index)
$middle  = $match.Groups[1].Value + $xunit3Version + $match.Groups[3].Value
$suffix  = $originalText.Substring($match.Index + $match.Length)
$newText = $prefix + $middle + $suffix

# --- Ensure writable ---
$item = Get-Item -LiteralPath $CsprojPath
if ($item.Attributes -band [IO.FileAttributes]::ReadOnly) { attrib -r $CsprojPath | Out-Null }

# --- Write back with EXACT original encoding (including BOM if present) ---
[byte[]]$contentBytes = $writeEncoding.GetBytes($newText)
[byte[]]$preamble     = $writeEncoding.GetPreamble()  # non-empty only if BOM applies
if ($preamble -and $preamble.Length -gt 0) {
    $outBytes = New-Object byte[] ($preamble.Length + $contentBytes.Length)
    [Array]::Copy($preamble, 0, $outBytes, 0, $preamble.Length)
    [Array]::Copy($contentBytes, 0, $outBytes, $preamble.Length, $contentBytes.Length)
    [System.IO.File]::WriteAllBytes($CsprojPath, $outBytes)
} else {
    [System.IO.File]::WriteAllBytes($CsprojPath, $contentBytes)
}

# --- Verify (decode again using the same writeEncoding) ---
[byte[]]$verifyBytes  = [System.IO.File]::ReadAllBytes($CsprojPath)
$pLen2 = 0
$p2    = $writeEncoding.GetPreamble()
if ($p2 -and $verifyBytes.Length -ge $p2.Length) { $pLen2 = $p2.Length }
$verifyText = $writeEncoding.GetString($verifyBytes, $pLen2, $verifyBytes.Length - $pLen2)

$vm = $re.Match($verifyText)
$after = $null
if ($vm.Success) { $after = $vm.Groups[2].Value }

if ($after -ne $xunit3Version) {
    Write-Error "Save verification failed. Expected '$xunit3Version', found '$after'."
    exit 1
}

Write-Host "Updated <PackageVersion> $before -> $xunit3Version in '$CsprojPath' (encoding & BOM preserved exactly)."

