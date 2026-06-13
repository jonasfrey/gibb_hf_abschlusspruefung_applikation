# PROG2 – Testing-Dokumentation

## Verwendetes Werkzeug

**VS Code REST Client / Endpoint Explorer** — Datei `ubahn-test.http`

Das Skript kann in VS Code mit der Extension "REST Client" oder direkt im Visual Studio Endpoint Explorer ausgeführt werden. Jeder Request ist einzeln ausführbar.

## Voraussetzungen

- Backend läuft auf `http://localhost:5000` (`dotnet run`)
- MariaDB läuft und Datenbank `UBahn` ist erreichbar

## Ablauf des Testskripts

| Schritt | Beschreibung | Erwartetes Ergebnis |
|---------|-------------|---------------------|
| 1 | Health Check `GET /health` | `200 OK { "status": "healthy" }` |
| 2 | Linie 1 anlegen (Hauptlinie) | `201 Created` mit Id 1 |
| 3 | Linie 2 anlegen (Stadtlinie) | `201 Created` mit Id 2 |
| 4–7 | 4 Stationen auf Hauptlinie (Pos. 1–4) | `201 Created` je Station |
| 8–11 | 4 Stationen auf Stadtlinie (Pos. 1–4) | `201 Created` je Station |
| 12–14 | Fahrzeiten Hauptlinie (4, 7, 9 min) | `201 Created` |
| 15–17 | Fahrzeiten Stadtlinie (5, 2, 3 min) | `201 Created` |
| 18 | Fahrtabfrage Parkhaus → Hauptbahnhof | `200 OK`, 11 min, 1 Zwischenstation |
| 19 | Station umbenennen (PUT) | `204 No Content` |
| 20 | Station zurückbenennen (PUT) | `204 No Content` |
| 21 | Teststation anlegen (POST) | `201 Created` |
| 22 | Teststation löschen (DELETE) | `204 No Content` |
| 23 | Fehlerfall: Stationen verschiedener Linien | `400 Bad Request` |

## Fahrtabfrage Parkhaus → Hauptbahnhof

```
GET /api/fahrt?start=1&ziel=3
```

Erwartete Antwort:
```json
{
  "startStation": "Parkhaus",
  "zielStation": "Hauptbahnhof",
  "anzahlZwischenstationen": 1,
  "zwischenstationen": ["Einkaufsstrasse"],
  "gesamtdauerMinuten": 11
}
```

Begründung: Parkhaus→Einkaufsstrasse (4 min) + Einkaufsstrasse→Hauptbahnhof (7 min) = 11 min, 1 Zwischenstation.

## Fehlerfälle

- **Start == Ziel**: `400 Bad Request` — "Start- und Zielstation sind identisch."
- **Verschiedene Linien**: `400 Bad Request` — Umsteigen nicht unterstützt.
- **Unbekannte ID**: `404 Not Found`
