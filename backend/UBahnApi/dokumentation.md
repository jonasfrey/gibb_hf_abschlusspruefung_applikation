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
│   └── UBahnContext.cs          – EF Core DbContext (MariaDB)
├── DTOs/                        – Request/Response-Objekte (Records)
├── Models/                      – Entitäten (Linie, Station, Fahrzeit)
├── Migrations/                  – EF Core Code-First Migrationen
├── appsettings.json             – Verbindungsstring, Port 5000
├── Program.cs                   – App-Setup: Swagger, CORS, EF, Routing
└── ubahn-test.http              – HTTP-Testskript (VS Endpoint Explorer)
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
- `OnDelete(Cascade)` bei Station→Linie: wenn eine Linie gelöscht wird, werden alle Stationen (und deren Fahrzeiten) automatisch entfernt.

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

```bash
cd backend/UBahnApi
dotnet ef database update   # Migration auf DB anwenden
dotnet run                  # Start auf http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

# Testing 
Das testing wird ueber einen browser und die dev tools durchgefuehrt. 
Dann wird das js script api-test.js verwendet. 

# KI 
es wurde das KI tool claude code verwendet um beim programmieren und dokumentieren zu helfen