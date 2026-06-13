# PROG2 – REST-Backend U-Bahn API

## Projektstruktur

```
UBahnApi/
├── Controllers/
│   ├── LinienController.cs      – CRUD für Linien
│   ├── StationenController.cs   – CRUD für Stationen
│   ├── FahrzeitenController.cs  – CRUD für Fahrzeiten
│   └── FahrtController.cs       – Fahrtabfrage (GET /api/fahrt)
├── Data/
│   ├── UBahnContext.cs          – EF Core DbContext (MariaDB)
│   ├── DbBootstrapper.cs        – legt DB + admin-User beim Start an (als root)
│   └── DataSeeder.cs            – befüllt das U-Bahn-Netz, falls DB leer
├── DTOs/                        – Request/Response-Objekte (Records)
├── Models/                      – Entitäten (Linie, Station, Fahrzeit)
├── Migrations/                  – EF Core Code-First Migrationen
├── appsettings.json             – Verbindungsstring, Bootstrap, Port 5000
├── Program.cs                   – App-Setup: Swagger, CORS, EF, Bootstrap, Seed
├── ubahn-test.http              – HTTP-Testskript (VS Endpoint Explorer)
├── api-test.js                  – JS-Testskript für die Browser-Konsole
└── testing-dokumentation.md     – Dokumentation des Testings
```

## Datenbankschema

| Tabelle    | Spalten                                                  |
|------------|----------------------------------------------------------|
| Linien     | Id (PK), Name                                            |
| Stationen  | Id (PK), Name, LinieId (FK), Position                    |
| Fahrzeiten | Id (PK), VonStationId (FK), NachStationId (FK), DauerMinuten |

**Designentscheidungen:**
- `Position` in Station: ganzzahlige Reihenfolge auf der Linie, frei wählbar. Ermöglicht flexibles Einfügen ohne Renaming.
- `Fahrzeit` als separate Tabelle: die Anforderungen verlangen CRUD für Fahrzeiten unabhängig von Stationen.
- `OnDelete(Restrict)` bei Fahrzeit→Station: verhindert stilles Löschen von Fahrzeiten wenn Stationen gelöscht werden. Der StationenController löscht explizit betroffene Fahrzeiten vor dem Stationslöschen.
- `OnDelete(Cascade)` bei Station→Linie: wenn eine Linie gelöscht wird, werden alle Stationen automatisch entfernt. Da der FK Fahrzeit→Station auf `Restrict` steht, löscht der `LinienController` die abhängigen Fahrzeiten zuvor explizit.

## Lösch-Verhalten Linie mit Stationen

Beim Löschen einer Linie werden ihre Stationen **und** deren Fahrzeiten
mitgelöscht (kaskadierend). Begründung: Eine Linie ohne Stationen ist fachlich
bedeutungslos, verwaiste Stationen ohne Linie wären inkonsistent. Diese
Entscheidung ist identisch im Frontend (WEB2) umgesetzt.

## Automatische Initialisierung beim Start

Damit `dotnet run` ohne manuelle Schritte funktioniert, läuft beim Start:

1. **Bootstrap** (`DbBootstrapper`): verbindet sich als `root` und legt – falls
   nötig – die Datenbank `UBahn` sowie den User `admin` (mit Rechten) an. Sind
   beide bereits vorhanden (z.B. auf der Prüfungs-VM), wird der Schritt still
   übersprungen.
2. **Migration** (`db.Database.Migrate()`): wendet die EF-Migrationen an und
   erstellt die Tabellen.
3. **Seed** (`DataSeeder`): befüllt das vollständige U-Bahn-Netz (2 Linien,
   8 Stationen, 6 Fahrzeiten), sofern noch keine Linien existieren.

Das root-Passwort für den Bootstrap steht in `appsettings.json` unter
`Bootstrap:RootPassword` (Standard leer). Auf Maschinen mit gesetztem
root-Passwort wird es in `appsettings.Development.json` überschrieben.

## Endpunkte

| Methode | Pfad                        | Beschreibung                              |
|---------|----------------------------|-------------------------------------------|
| GET     | /api/linien                 | Alle Linien mit Stationen (sortiert)      |
| GET     | /api/linien/{id}            | Eine Linie mit ihren Stationen            |
| POST    | /api/linien                 | Neue Linie anlegen                        |
| PUT     | /api/linien/{id}            | Linie umbenennen                          |
| DELETE  | /api/linien/{id}            | Linie (+ Stationen) löschen              |
| GET     | /api/stationen              | Alle Stationen (mit Linienname)           |
| GET     | /api/stationen/{id}         | Eine Station                              |
| POST    | /api/stationen              | Station anlegen                           |
| PUT     | /api/stationen/{id}         | Station bearbeiten                        |
| DELETE  | /api/stationen/{id}         | Station (+ Fahrzeiten) löschen            |
| GET     | /api/fahrzeiten             | Alle Fahrzeiten                           |
| GET     | /api/fahrzeiten/{id}        | Eine Fahrzeit                             |
| POST    | /api/fahrzeiten             | Fahrzeit anlegen                          |
| PUT     | /api/fahrzeiten/{id}        | Fahrzeit bearbeiten                       |
| DELETE  | /api/fahrzeiten/{id}        | Fahrzeit löschen                          |
| GET     | /api/fahrt?start=X&ziel=Y   | Fahrtabfrage zwischen zwei Stationen      |
| GET     | /health                     | Health-Check für Docker/Deployment        |
| GET     | /swagger                    | Swagger UI                                |

## Fahrtabfrage-Logik

1. Prüfen ob Start == Ziel → 400
2. Prüfen ob Stationen auf der gleichen Linie → 400 wenn nicht
3. Alle Stationen der Linie nach Position sortieren
4. Teilstrecke von min(posStart, posZiel) bis max(posStart, posZiel) bestimmen
5. Fahrzeiten der Teilstrecke summieren (Richtungsunabhängig)
6. Zwischenstationen = Stationen ohne Start und Ziel

## Fehlerbehandlung

| Situation                          | HTTP-Code | Antwort                        |
|------------------------------------|-----------|--------------------------------|
| Ressource nicht gefunden           | 404       | Standard NotFound              |
| Ungültige LinieId bei Station      | 400       | "Linie nicht gefunden."        |
| Stationen auf verschiedenen Linien | 400       | Erklärende Meldung             |
| Start == Ziel                      | 400       | "Start- und Zielstation sind identisch." |
| Fahrzeit nicht zwischen Nachbarn   | 400       | Erklärende Meldung             |

## Starten

Voraussetzung: MariaDB läuft. Datenbank und User werden automatisch angelegt
(siehe «Automatische Initialisierung»).

```bash
cd backend/UBahnApi
dotnet run                  # Start auf http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

Migration und Seed laufen automatisch – kein manuelles `dotnet ef database update`
nötig.

# Testing 
Das Testing wird über einen Browser und die Dev-Tools durchgeführt.
Dazu wird das JS-Skript `api-test.js` in der Browser-Konsole ausgeführt; es legt
eine eigene Test-Linie an, prüft alle CRUD-Operationen samt Verifikation und
räumt am Ende wieder auf. Ergänzend bildet `ubahn-test.http` das vollständige
U-Bahn-Netz nach. Details siehe `testing-dokumentation.md`.

# KI 
es wurde das KI tool claude code verwendet um beim programmieren und dokumentieren zu helfen