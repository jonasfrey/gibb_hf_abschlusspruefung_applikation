# db-setup.ps1 – MariaDB fuer UBahn-Backend vorbereiten (Windows VM)
# Ausfuehren: .\scripts\setup-db.ps1
# Root-Passwort eingeben wenn gefragt (oft leer – einfach Enter)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlFile   = Join-Path $scriptDir "db-setup.sql"

Write-Host "Fuehre db-setup.sql als root aus..." -ForegroundColor Cyan

mysql -u root -p --execute="source $sqlFile"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Datenbank erfolgreich eingerichtet." -ForegroundColor Green
    Write-Host "Du kannst jetzt 'dotnet run' im backend/UBahnApi Verzeichnis ausfuehren."
} else {
    Write-Host "Fehler beim Ausfuehren des Skripts. Pruefen ob MariaDB laeuft." -ForegroundColor Red
}
