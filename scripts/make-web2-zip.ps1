# make-web2-zip.ps1 – erstellt die Abgabe-ZIP fuer WEB2 (Windows VM).
# Ausfuehren aus dem Repo-Wurzelverzeichnis:  .\scripts\make-web2-zip.ps1
# Die ZIP landet auf dem Desktop. dokumentation.md liegt im ZIP-Wurzelverzeichnis.

$ErrorActionPreference = "Stop"
$repo     = Split-Path -Parent $PSScriptRoot
$frontend = Join-Path $repo "frontend"
$desktop  = [Environment]::GetFolderPath("Desktop")
$zip      = Join-Path $desktop "Abgabe_WEB2.zip"

Write-Host "Erstelle WEB2-ZIP..." -ForegroundColor Cyan

# Quellcode in ein temporaeres Verzeichnis kopieren – ohne node_modules/dist
$tmp = Join-Path $env:TEMP "web2_abgabe"
if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
New-Item -ItemType Directory -Path $tmp | Out-Null

robocopy $frontend $tmp /E /XD node_modules dist /XF .env.local | Out-Null

if (Test-Path $zip) { Remove-Item $zip }
# Inhalt von frontend zippen -> dokumentation.md liegt im ZIP-Root
Compress-Archive -Path (Join-Path $tmp "*") -DestinationPath $zip
Remove-Item $tmp -Recurse -Force

Write-Host "Fertig: $zip" -ForegroundColor Green
