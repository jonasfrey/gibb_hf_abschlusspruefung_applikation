# make-prog2-zips.ps1 – erstellt die beiden Abgabe-ZIPs fuer PRG2 (Windows VM).
# Ausfuehren aus dem Repo-Wurzelverzeichnis:  .\scripts\make-prog2-zips.ps1
# Die ZIPs landen auf dem Desktop.

$ErrorActionPreference = "Stop"
$repo    = Split-Path -Parent $PSScriptRoot
$api     = Join-Path $repo "backend\UBahnApi"
$desktop = [Environment]::GetFolderPath("Desktop")

$backendZip = Join-Path $desktop "Abgabe_PRG2_Backend.zip"
$testingZip = Join-Path $desktop "Abgabe_PRG2_Testing.zip"

# --- 1) Backend-ZIP: gesamtes Projekt ohne Build-Artefakte ---
Write-Host "Erstelle Backend-ZIP..." -ForegroundColor Cyan
$tmp = Join-Path $env:TEMP "prg2_backend"
if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
New-Item -ItemType Directory -Path $tmp | Out-Null

# Projekt kopieren, bin/obj/.vs und lokale Settings auslassen
robocopy $api (Join-Path $tmp "UBahnApi") /E `
  /XD bin obj .vs `
  /XF appsettings.Development.json | Out-Null

if (Test-Path $backendZip) { Remove-Item $backendZip }
Compress-Archive -Path (Join-Path $tmp "UBahnApi") -DestinationPath $backendZip
Remove-Item $tmp -Recurse -Force

# --- 2) Testing-ZIP: Skripte + Doku ---
Write-Host "Erstelle Testing-ZIP..." -ForegroundColor Cyan
if (Test-Path $testingZip) { Remove-Item $testingZip }
Compress-Archive -Path `
  (Join-Path $api "ubahn-test.http"), `
  (Join-Path $api "api-test.js"), `
  (Join-Path $api "testing-dokumentation.md") `
  -DestinationPath $testingZip

Write-Host "Fertig:" -ForegroundColor Green
Write-Host "  $backendZip"
Write-Host "  $testingZip"
