# Delete-AllErroredSauceJobs.ps1

param(
  [string]$Region = "us-west-1",
  [string]$SauceUser = $env:SAUCE_USER_NAME,
  [string]$SauceKey  = $env:SAUCE_API_KEY,

  [datetime]$From = "2026-02-01",
  [datetime]$To   = [datetime]::UtcNow,

  [int]$PageSize = 100,
  [int]$ChunkDays = 14,
  [int]$TimeoutSec = 30,
  [int]$MaxRdcPages = 50,

  [switch]$DryRun,
  [switch]$IncludeFailed,
  [switch]$SkipAnalytics,
  [switch]$SkipRdcDirect,
  [switch]$VerboseJobDump
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($SauceUser)) { throw "SAUCE_USER_NAME is not set." }
if ([string]::IsNullOrWhiteSpace($SauceKey))  { throw "SAUCE_API_KEY is not set." }

$auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$SauceUser`:$SauceKey"))

$headers = @{
  Authorization = "Basic $auth"
  Accept        = "application/json"
}

$analyticsBase   = "https://api.$Region.saucelabs.com/v1/analytics/tests"
$virtualJobsBase = "https://api.$Region.saucelabs.com/rest/v1/$SauceUser/jobs"
$rdcJobsBase     = "https://api.$Region.saucelabs.com/v1/rdc/jobs"

$analyticsStatuses = @("error")
if ($IncludeFailed) { $analyticsStatuses += "failed" }

function Write-Log {
  param([string]$Message = "")
  Write-Host $Message
}

function Invoke-SauceWeb {
  param(
    [ValidateSet("GET", "DELETE")]
    [string]$Method,
    [string]$Url
  )

  try {
    $response = Invoke-WebRequest `
      -Uri $Url `
      -Headers $headers `
      -Method $Method `
      -TimeoutSec $TimeoutSec `
      -SkipHttpErrorCheck

    return [pscustomobject]@{
      StatusCode = [int]$response.StatusCode
      Content    = [string]$response.Content
      Success    = ([int]$response.StatusCode -ge 200 -and [int]$response.StatusCode -lt 300)
    }
  }
  catch {
    return [pscustomobject]@{
      StatusCode = 0
      Content    = $_.Exception.Message
      Success    = $false
    }
  }
}

function Invoke-SauceJsonGet {
  param([string]$Url)

  $result = Invoke-SauceWeb -Method GET -Url $Url

  if (-not $result.Success) {
    Write-Warning "GET failed [$($result.StatusCode)]: $Url"
    Write-Warning $result.Content
    return $null
  }

  if ([string]::IsNullOrWhiteSpace($result.Content)) {
    return $null
  }

  try {
    return $result.Content | ConvertFrom-Json
  }
  catch {
    Write-Warning "Failed to parse JSON from: $Url"
    Write-Warning $_.Exception.Message
    return $null
  }
}

function Get-ArrayFromResponse {
  param($Response)

  if ($null -eq $Response) { return @() }

  foreach ($name in @("items", "entities", "data", "results", "tests", "jobs")) {
    if ($Response.PSObject.Properties.Name -contains $name) {
      return @($Response.$name)
    }
  }

  if ($Response -is [array]) { return @($Response) }

  return @($Response)
}

function Get-StringProperty {
  param($Object, [string[]]$Names)

  if ($null -eq $Object) { return "" }
  if ($Object -is [string]) { return "" }

  foreach ($name in $Names) {
    if ($Object.PSObject.Properties.Name -contains $name) {
      $value = [string]$Object.$name
      if (-not [string]::IsNullOrWhiteSpace($value)) {
        return $value
      }
    }
  }

  return ""
}

function Get-DatePropertyUtc {
  param($Object, [string[]]$Names)

  if ($null -eq $Object) { return $null }
  if ($Object -is [string]) { return $null }

  foreach ($name in $Names) {
    if (-not ($Object.PSObject.Properties.Name -contains $name)) { continue }

    $value = $Object.$name
    if ($null -eq $value) { continue }

    if ($value -is [datetime]) {
      return $value.ToUniversalTime()
    }

    $text = [string]$value
    if ([string]::IsNullOrWhiteSpace($text)) { continue }

    $num = 0L
    if ([int64]::TryParse($text, [ref]$num)) {
      if ($num -gt 999999999999) {
        return [DateTimeOffset]::FromUnixTimeMilliseconds($num).UtcDateTime
      }

      if ($num -gt 999999999) {
        return [DateTimeOffset]::FromUnixTimeSeconds($num).UtcDateTime
      }
    }

    $parsed = [datetime]::MinValue
    if ([datetime]::TryParse($text, [ref]$parsed)) {
      return $parsed.ToUniversalTime()
    }
  }

  return $null
}

function Add-UniqueValue {
  param(
    [System.Collections.Generic.List[string]]$List,
    [string]$Value
  )

  if ([string]::IsNullOrWhiteSpace($Value)) { return }

  if (-not $List.Contains($Value)) {
    [void]$List.Add($Value)
  }
}

function Get-CandidateJobIds {
  param($Item)

  $ids = [System.Collections.Generic.List[string]]::new()

  if ($null -eq $Item) { return @() }
  if ($Item -is [string]) { return @() }

  foreach ($name in @(
    "job_id",
    "jobId",
    "job_uuid",
    "jobUuid",
    "session_id",
    "sessionId",
    "selenium_session_id",
    "seleniumSessionId",
    "appium_session_id",
    "appiumSessionId",
    "device_session_id",
    "deviceSessionId",
    "id",
    "uuid"
  )) {
    if ($Item.PSObject.Properties.Name -contains $name) {
      Add-UniqueValue -List $ids -Value ([string]$Item.$name)
    }
  }

  foreach ($nestedName in @("job", "session", "metadata", "details", "automation_backend")) {
    if (-not ($Item.PSObject.Properties.Name -contains $nestedName)) { continue }

    $nested = $Item.$nestedName
    if ($null -eq $nested) { continue }
    if ($nested -is [string]) { continue }

    foreach ($name in @(
      "job_id",
      "jobId",
      "id",
      "uuid",
      "session_id",
      "sessionId",
      "appium_session_id",
      "appiumSessionId",
      "device_session_id",
      "deviceSessionId"
    )) {
      if ($nested.PSObject.Properties.Name -contains $name) {
        Add-UniqueValue -List $ids -Value ([string]$nested.$name)
      }
    }
  }

  return @($ids)
}

function Test-InAnalyticsWindow {
  param($Item)

  $dt = Get-DatePropertyUtc $Item @(
    "creation_time",
    "creationTime",
    "created_at",
    "createdAt",
    "start_time",
    "startTime",
    "end_time",
    "endTime",
    "modification_time",
    "modificationTime"
  )

  if ($null -eq $dt) {
    return $true
  }

  return $dt -ge $From.ToUniversalTime() -and $dt -le $To.ToUniversalTime()
}

function Test-RdcJobLooksErrored {
  param($Job)

  if ($null -eq $Job) { return $false }
  if ($Job -is [string]) { return $false }

  $status = (Get-StringProperty $Job @("status")).ToLowerInvariant()
  $consolidated = (Get-StringProperty $Job @("consolidated_status")).ToLowerInvariant()
  $result = (Get-StringProperty $Job @("result", "outcome")).ToLowerInvariant()

  if ($status -eq "error") { return $true }
  if ($consolidated -eq "error") { return $true }
  if ($result -eq "error") { return $true }

  if ($status -eq "errored") { return $true }
  if ($consolidated -eq "errored") { return $true }
  if ($result -eq "errored") { return $true }

  if ($IncludeFailed) {
    if ($status -eq "failed") { return $true }
    if ($consolidated -eq "failed") { return $true }
    if ($result -eq "failed") { return $true }
  }

  $hasCrashed = (Get-StringProperty $Job @("has_crashed", "hasCrashed")).ToLowerInvariant()
  if ($hasCrashed -eq "true") { return $true }

  return $false
}

function Get-TestResultsForWindowAndStatus {
  param(
    [datetime]$WindowStart,
    [datetime]$WindowEnd,
    [string]$Status
  )

  $start = $WindowStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
  $end   = $WindowEnd.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

  $all = @()
  $fromIndex = 0

  while ($true) {
    $url =
      "${analyticsBase}" +
      "?start=$([uri]::EscapeDataString($start))" +
      "&end=$([uri]::EscapeDataString($end))" +
      "&size=$PageSize" +
      "&from=$fromIndex" +
      "&descending=true" +
      "&status=$Status"

    Write-Log "Fetching Analytics Test Results: $url"

    $response = Invoke-SauceJsonGet -Url $url
    if ($null -eq $response) { break }

    $items = @(Get-ArrayFromResponse -Response $response)
    if ($items.Count -eq 0) { break }

    $all += $items

    if ($response.PSObject.Properties.Name -contains "has_more") {
      if ($response.has_more -ne $true) { break }
    }
    elseif ($items.Count -lt $PageSize) {
      break
    }

    $fromIndex += $PageSize
  }

  return @($all)
}

function Resolve-DeletableJob {
  param([string[]]$CandidateIds)

  foreach ($id in $CandidateIds) {
    if ([string]::IsNullOrWhiteSpace($id)) { continue }

    $virtualUrl = "${virtualJobsBase}/$id"
    $rdcUrl     = "${rdcJobsBase}/$id"

    $virtual = Invoke-SauceWeb -Method GET -Url $virtualUrl
    if ($virtual.Success) {
      return [pscustomobject]@{
        Id        = $id
        Kind      = "virtual"
        DeleteUrl = $virtualUrl
      }
    }

    $rdc = Invoke-SauceWeb -Method GET -Url $rdcUrl
    if ($rdc.Success) {
      return [pscustomobject]@{
        Id        = $id
        Kind      = "rdc"
        DeleteUrl = $rdcUrl
      }
    }
  }

  return $null
}

function Add-RdcItems {
  param(
    [array]$Items,
    [hashtable]$Seen,
    [System.Collections.Generic.List[object]]$All
  )

  foreach ($item in $Items) {
    if ($null -eq $item) { continue }
    if ($item -is [string]) { continue }

    $ids = @(Get-CandidateJobIds $item)
    $key = if ($ids.Count -gt 0) { $ids[0] } else { ($item | ConvertTo-Json -Compress -Depth 5) }

    if (-not $Seen.ContainsKey($key)) {
      $Seen[$key] = $true
      [void]$All.Add($item)
    }
  }
}

function Get-RdcJobsDirect {
  $all = [System.Collections.Generic.List[object]]::new()
  $seen = @{}

  $urls = [System.Collections.Generic.List[string]]::new()

  [void]$urls.Add("${rdcJobsBase}?limit=$PageSize")
  [void]$urls.Add("${rdcJobsBase}/?limit=$PageSize")

  for ($page = 0; $page -lt $MaxRdcPages; $page++) {
    $offsetZeroBased = $page * $PageSize
    $offsetOneBased = ($page * $PageSize) + 1

    if ($offsetZeroBased -ne 0) {
      [void]$urls.Add("${rdcJobsBase}?limit=$PageSize&offset=$offsetZeroBased")
    }

    [void]$urls.Add("${rdcJobsBase}?limit=$PageSize&offset=$offsetOneBased")
  }

  foreach ($url in $urls) {
    Write-Log "Fetching RDC Jobs Direct: $url"

    $response = Invoke-SauceJsonGet -Url $url
    if ($null -eq $response) { continue }

    $items = @(Get-ArrayFromResponse -Response $response)
    if ($items.Count -eq 0) { continue }

    Add-RdcItems -Items $items -Seen $seen -All $all
  }

  return @($all.ToArray())
}

function New-JobRecord {
  param(
    [string]$Source,
    [string]$Kind,
    [string]$JobId,
    [string]$DeleteUrl,
    $Raw
  )

  return [pscustomobject]@{
    Source       = $Source
    JobId        = $JobId
    Kind         = $Kind
    DeleteUrl    = $DeleteUrl
    CandidateIds = (@(Get-CandidateJobIds $Raw) -join ",")
    Owner        = Get-StringProperty $Raw @("owner", "owner_sauce", "user", "username")
    Name         = Get-StringProperty $Raw @("name", "test_name", "testName")
    Status       = Get-StringProperty $Raw @("status", "consolidated_status", "result", "outcome")
    Created      = Get-StringProperty $Raw @("creation_time", "creationTime", "created_at", "createdAt", "start_time", "startTime")
    Error        = Get-StringProperty $Raw @("error", "error_message", "errorMessage", "failure_reason", "failureReason")
    Raw          = $Raw
  }
}

function Write-RdcDiagnostics {
  param([array]$RdcJobs)

  Write-Log ""
  Write-Log "RDC direct jobs returned: $($RdcJobs.Count)"

  if ($RdcJobs.Count -eq 0) {
    return
  }

  $statusGroups =
    $RdcJobs |
    Where-Object { $_ -isnot [string] } |
    Group-Object -Property consolidated_status, status |
    Sort-Object Count -Descending

  Write-Log ""
  Write-Log "RDC status groups:"
  foreach ($group in $statusGroups) {
    Write-Log "  $($group.Name): $($group.Count)"
  }

  $errorJobs = @($RdcJobs | Where-Object { Test-RdcJobLooksErrored $_ })

  Write-Log ""
  Write-Log "RDC errored jobs detected by status: $($errorJobs.Count)"

  foreach ($job in @($errorJobs | Select-Object -First 10)) {
    Write-Log "  ERROR Id=$((Get-StringProperty $job @('id','job_id','jobId','uuid'))) Status=$((Get-StringProperty $job @('status'))) Consolidated=$((Get-StringProperty $job @('consolidated_status'))) Created=$((Get-StringProperty $job @('creation_time','start_time'))) Name=$((Get-StringProperty $job @('name','test_name','testName')))"
  }
}

Write-Log ""
Write-Log "Sauce region:       $Region"
Write-Log "Sauce user:         $SauceUser"
Write-Log "From UTC:           $($From.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"
Write-Log "To UTC:             $($To.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"
Write-Log "Analytics statuses: $($analyticsStatuses -join ', ')"
Write-Log "Dry run:            $DryRun"
Write-Log "Skip Analytics:     $SkipAnalytics"
Write-Log "Skip RDC Direct:    $SkipRdcDirect"
Write-Log ""

$deletable = @()
$notDeletable = @()
$seenDeleteIds = @{}

if (-not $SkipAnalytics) {
  $analyticsSeen = @{}
  $analyticsRecords = @()

  $cursor = $From

  while ($cursor -lt $To) {
    $windowEnd = $cursor.AddDays($ChunkDays)
    if ($windowEnd -gt $To) { $windowEnd = $To }

    Write-Log ""
    Write-Log "Analytics Window: $($cursor.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")) -> $($windowEnd.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"))"

    foreach ($status in $analyticsStatuses) {
      $items = @(Get-TestResultsForWindowAndStatus -WindowStart $cursor -WindowEnd $windowEnd -Status $status)

      foreach ($item in $items) {
        $candidateIds = @(Get-CandidateJobIds -Item $item)
        if ($candidateIds.Count -eq 0) { continue }

        $key = $candidateIds -join "|"
        if ($analyticsSeen.ContainsKey($key)) { continue }

        $analyticsSeen[$key] = $true
        $analyticsRecords += $item
      }
    }

    $cursor = $windowEnd
  }

  Write-Log ""
  Write-Log "Analytics matching records found: $($analyticsRecords.Count)"
  Write-Log "Resolving Analytics records to deletable jobs..."
  Write-Log ""

  $count = 0

  foreach ($item in $analyticsRecords) {
    $count++
    $candidateIds = @(Get-CandidateJobIds -Item $item)

    Write-Log "Resolving Analytics [$count / $($analyticsRecords.Count)]: $($candidateIds -join ', ')"

    $resolved = Resolve-DeletableJob -CandidateIds $candidateIds

    if ($null -eq $resolved) {
      $notDeletable += New-JobRecord `
        -Source "analytics" `
        -Kind "unknown" `
        -JobId "" `
        -DeleteUrl "" `
        -Raw $item

      continue
    }

    if (-not $seenDeleteIds.ContainsKey($resolved.Id)) {
      $seenDeleteIds[$resolved.Id] = $true

      $deletable += New-JobRecord `
        -Source "analytics" `
        -Kind $resolved.Kind `
        -JobId $resolved.Id `
        -DeleteUrl $resolved.DeleteUrl `
        -Raw $item
    }
  }
}

if (-not $SkipRdcDirect) {
  Write-Log ""
  Write-Log "Fetching RDC jobs directly..."
  Write-Log ""

  $rdcJobs = @(Get-RdcJobsDirect)

  Write-RdcDiagnostics -RdcJobs $rdcJobs

  Write-Log ""
  Write-Log "Filtering RDC jobs by errored status only..."
  Write-Log ""

  $rdcMatching = @(
    $rdcJobs |
      Where-Object { $_ -isnot [string] } |
      Where-Object { Test-RdcJobLooksErrored $_ }
  )

  Write-Log "RDC direct matching errored jobs found: $($rdcMatching.Count)"
  Write-Log ""

  foreach ($job in $rdcMatching) {
    $candidateIds = @(Get-CandidateJobIds $job)

    if ($candidateIds.Count -eq 0) {
      $notDeletable += New-JobRecord `
        -Source "rdc-direct" `
        -Kind "rdc" `
        -JobId "" `
        -DeleteUrl "" `
        -Raw $job

      continue
    }

    $jobId = $candidateIds[0]

    if ($seenDeleteIds.ContainsKey($jobId)) {
      continue
    }

    $seenDeleteIds[$jobId] = $true

    $deletable += New-JobRecord `
      -Source "rdc-direct" `
      -Kind "rdc" `
      -JobId $jobId `
      -DeleteUrl "${rdcJobsBase}/$jobId" `
      -Raw $job
  }
}

Write-Log ""
Write-Log "Accurate summary before delete"
Write-Log "------------------------------"
Write-Log "Deletable jobs found:          $($deletable.Count)"
Write-Log "Analytics/RDC not deletable:   $($notDeletable.Count)"
Write-Log ""

$deleted = 0
$failedDeletes = @()

foreach ($job in $deletable) {
  Write-Log "DELETABLE JOB:"
  Write-Log "  Source:     $($job.Source)"
  Write-Log "  Job Id:     $($job.JobId)"
  Write-Log "  Kind:       $($job.Kind)"
  Write-Log "  Name:       $($job.Name)"
  Write-Log "  Status:     $($job.Status)"
  Write-Log "  Created:    $($job.Created)"
  Write-Log "  Error:      $($job.Error)"

  if ($VerboseJobDump) {
    Write-Log "  Raw:"
    Write-Log ($job.Raw | ConvertTo-Json -Depth 50)
  }

  if ($DryRun) {
    Write-Log "DRY RUN: would delete $($job.DeleteUrl)"
    Write-Log ""
    continue
  }

  $deleteResult = Invoke-SauceWeb -Method DELETE -Url $job.DeleteUrl

  if ($deleteResult.Success -or $deleteResult.StatusCode -eq 404) {
    Write-Log "Deleted or already gone: $($job.JobId)"
    $deleted++
  }
  else {
    Write-Warning "Delete failed [$($deleteResult.StatusCode)]: $($job.DeleteUrl)"
    Write-Warning $deleteResult.Content
    $failedDeletes += $job
  }

  Write-Log ""
}

Write-Log ""
Write-Log "Done."
Write-Log "-----"
Write-Log "Deletable jobs found:          $($deletable.Count)"
Write-Log "Deleted:                       $deleted"
Write-Log "Not deletable:                 $($notDeletable.Count)"
Write-Log "Failed deletes:                $($failedDeletes.Count)"

if ($DryRun) {
  Write-Log ""
  Write-Log "Dry run only. Nothing was deleted."
}