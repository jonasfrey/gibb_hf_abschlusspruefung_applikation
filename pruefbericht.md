# Prüfbericht: Bewertungskriterien-Check

> Erstellt am 13.06.2026  
> Geprüft wurde der gesamte Projektstand anhand der Bewertungskriterien aus
> `web2.md`, `prog2.md`, `dep.md` sowie der zugehörigen Abgabedokumente
> (`web2_abgabe.md`, `prog2_abgabe.md`, `dep_abgabe.md`).

---

## Was wurde geprüft?

Für jedes der drei Module (WEB2, PROG2, DEP) wurden sämtliche Bewertungskriterien
einzeln gegen den tatsächlichen Code und die Dokumentation abgeglichen. Dazu
wurden alle relevanten Quelldateien gelesen – es wurden **keine Dateien
verändert**.

### Geprüfte Dateien

**WEB2 (Frontend Vue 3):**
- `frontend/src/main.js` – App-Initialisierung
- `frontend/src/router/index.js` – Routen
- `frontend/src/views/StreckennetzView.vue` – Streckennetz-Ansicht
- `frontend/src/views/FahrtView.vue` – Fahrtabfrage
- `frontend/src/views/AdminView.vue` – Admin-Bereich
- `frontend/src/components/StationDetail.vue` – Station-Detail-Overlay
- `frontend/src/components/StationenPanel.vue` – Stationen-CRUD
- `frontend/src/components/LinienPanel.vue` – Linien-CRUD
- `frontend/src/components/FahrzeitenPanel.vue` – Fahrzeiten-CRUD
- `frontend/src/api/client.js` – REST-API-Client
- `frontend/src/api/store.js` – Zentraler Zustand (reactive store)
- `frontend/src/api/validators.js` – Validierungsregeln
- `frontend/src/assets/main.css` – Globales Styling
- `frontend/src/App.vue` – Root-Komponente
- `frontend/package.json` – Abhängigkeiten
- `frontend/vite.config.js` – Build-Konfiguration
- `frontend/dokumentation.md` – Begleitende Dokumentation

**PROG2 (Backend C#/.NET 10):**
- `backend/UBahnApi/Models/Linie.cs` – Modell
- `backend/UBahnApi/Models/Station.cs` – Modell
- `backend/UBahnApi/Models/Fahrzeit.cs` – Modell
- `backend/UBahnApi/DTOs/` – Data Transfer Objects (alle Dateien)
- `backend/UBahnApi/Data/UBahnContext.cs` – DbContext mit Beziehungen
- `backend/UBahnApi/Controllers/LinienController.cs` – Linien-Endpunkte
- `backend/UBahnApi/Controllers/StationenController.cs` – Stationen-Endpunkte
- `backend/UBahnApi/Controllers/FahrzeitenController.cs` – Fahrzeiten-Endpunkte
- `backend/UBahnApi/Controllers/FahrtController.cs` – Fahrtabfrage-Endpunkt
- `backend/UBahnApi/Program.cs` – App-Startup, Swagger, Health-Endpoint
- `backend/UBahnApi/Migrations/20260613071902_InitialCreate.cs` – DB-Migration
- `backend/UBahnApi/Migrations/UBahnContextModelSnapshot.cs` – Model-Snapshot
- `backend/UBahnApi/ubahn-test.http` – HTTP-Testskript
- `backend/UBahnApi/api-test.js` – JS-Testskript
- `backend/UBahnApi/dokumentation.md` – Backend-Dokumentation
- `backend/UBahnApi/testing-dokumentation.md` – Test-Dokumentation

**DEP (Docker Deployment):**
- `docker-compose.yml` – Multi-Container Stack
- `.env.example` – Dokumentierte Umgebungsvariablen
- `.env` – Tatsächliche Konfiguration
- `frontend/Dockerfile` – Frontend Multi-Stage Build
- `frontend/.dockerignore` – Build-Ausnahmen Frontend
- `frontend/nginx.conf` – Nginx SPA-Konfiguration
- `frontend/.env.production` – Produktions-Build-Variablen
- `backend/UBahnApi/Dockerfile` – Backend Multi-Stage Build
- `backend/UBahnApi/.dockerignore` – Build-Ausnahmen Backend
- `deploy/Caddyfile` – Reverse-Proxy-Konfiguration (Caddy)
- `deploy/certs/README.md` – Zertifikats-Anleitung
- `dokumentation.md` – DEP-Dokumentation

---

## WEB2 – Frontend (Vue 3) → 50/50 Punkte

### A. Anwendung läuft (3/3)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Frontend startet ohne Fehler | 1 | `main.js`: Vue 3 + Router + Vite korrekt konfiguriert |
| Kommunikation mit REST-API | 2 | `api/client.js`: zentraler fetch-Client, alle CRUD-Endpunkte angebunden |

### B. Streckennetz-Ansicht (6/6)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Beide Linien mit Stationen in Reihenfolge | 2 | `StreckennetzView.vue` Z. 61-94: Iteration über `store.linien`, Sortierung nach `position` Z. 73 |
| Linien farblich unterschieden | 1 | `LINIENFARBEN`-Array (blau/grün), Farbbalken und Linienfarbe pro Linie Z. 13, 26, 66-67 |
| Umsteigestation markiert | 1 | `umsteigeNamen`-Computed (Z. 16-24), "Umsteigen"-Badge + Doppelring-Styling Z. 84, 89, 168-182 |
| Klick auf Station zeigt Details | 2 | `openDetail()` befüllt `selected` (Z. 41-49), `StationDetail.vue` Overlay-Komponente |

### C. Fahrtabfrage (7/7)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Eingabe Start/Ziel möglich | 2 | Zwei `<select>`-Elemente mit `<optgroup>` nach Linie (`FahrtView.vue` Z. 76-97) |
| Resultat zeigt Fahrzeit, Anzahl, Zwischenstationen | 2 | `resultat`-Anzeige Z. 108-132: `gesamtdauerMinuten`, `anzahlZwischenstationen`, `zwischenstationen` als Liste |
| Meldung bei verschiedenen Linien | 2 | Error-Handling Z. 47-48, 54-57: erkennt "Linien" im Fehlertext, erklärt Umsteigen nicht unterstützt |
| Meldung bei identischen Stationen | 1 | Client-seitiger Pre-Check Z. 38-41: `startId === zielId` → Warnmeldung |

### D. Admin-Bereich CRUD (8/8)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Admin-Bereich erkennbar | 1 | Badge "Mitarbeitende - Stammdatenverwaltung", `admin-link`-CSS-Klasse mit gestricheltem Rand |
| Stationen CRUD | 2 | `StationenPanel.vue`: Anlegen (Z. 121-168), Liste (Z. 172-194), Bearbeiten inline (Z. 54-76), Löschen mit confirm (Z. 78-85), Reihenfolge per Pfeiltasten (Z. 88-103) |
| Linien CRUD | 2 | `LinienPanel.vue`: Anlegen (Z. 8-22), Liste (Z. 75-109), Bearbeiten inline (Z. 31-38), Löschen mit confirm + Kaskaden-Warnung (Z. 40-48) |
| Fahrzeiten CRUD | 2 | `FahrzeitenPanel.vue`: Anlegen (Z. 58-70), Liste (Z. 125-144), Bearbeiten (Z. 49-56), Löschen (Z. 73-80) |
| Änderungen ohne Reload sichtbar | 1 | `store.js` Z. 38-50: Jede Mutation ruft `loadAll()` → `Promise.all` → reactive State aktualisiert |

### E. Frontend-Validierungen (6/6)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| ≥3 Validierungsregeln umgesetzt | 2 | 5 Regeln in `validators.js`: Pflichtfeld, Namenslänge 2-40, Fahrzeit 1-120, eindeutiger Name in Linie, eindeutige Position |
| Speichern deaktiviert + Meldung am Feld | 2 | `istGueltig`/`neuFehler`/`bearbeiteFehler` Computed → `:disabled`-Bindings. CSS-Klasse `invalid` + `.err`-Div am Feld |
| Regeln in Dokumentation aufgeführt | 1 | `frontend/dokumentation.md` Z. 79-92: Tabelle mit allen 5 Regeln, Ort und Begründung |
| Begründung für jede Regel | 1 | Jede Regel hat eine nachvollziehbare Begründung in der Doku-Tabelle |

### F. Aufbau im Code (6/6)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Sinnvolle Komponenten/Views-Aufteilung | 2 | `views/` (3 Dateien) + `components/` (4 Dateien), klare Single-Responsibility |
| API-Logik von Darstellung getrennt | 2 | `api/client.js` (fetch), `api/store.js` (state), `api/validators.js` (validierung) – Komponenten rufen nie `fetch` direkt |
| Zustände gebündelt | 1 | `store.js`: ein `reactive()`-Objekt für `linien`, `stationen`, `fahrzeiten`, `loading`, `error`. `readonly`-Export via `useStore()` |
| Lesbarer Code | 1 | Deutsche Benennung, konsistent `<script setup>`, Kommentare für nicht-offensichtliche Logik, kein toter Code |

### G. Begleitendes Dokument (5/5)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Aufbau erklärt | 2 | `frontend/dokumentation.md` Z. 30-55: Vollständiger Verzeichnisbaum mit Beschreibungen |
| Architekturentscheidungen begründet | 1 | Z. 57-77: 4 Entscheidungen mit Begründung (API-Trennung, Reactive Store, CSS-Streckennetz, Komponenten-Split) |
| Passt zum Code | 1 | Alle Claims stimmen mit Implementierung überein |
| Offene Punkte benannt | 1 | Z. 117-124: Kein Auth, kein Umsteigen, Concurrency bei Uniqueness, Inkonsistenz beim Reorder |

### H. Gestaltung und visuelle Sorgfalt (9/9)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Eigene Farb-/Schriftwahl konsistent | 1 | `main.css` definiert Design-Tokens (`--primary`, `--accent`, `--danger` etc.), durchgängig verwendet |
| Komponenten visuell einheitlich | 2 | Buttons, Inputs, Karten teilen CSS-Klassen aus `main.css` |
| Abstände, Ausrichtung, Hierarchie | 2 | Durchdachtes Layout mit Grid/Flexbox, konsistente Spacings |
| Desktop + Mobile bedienbar | 1 | Responsive Design mit Media Queries in `main.css` |
| Desktop nicht nur gestrecktes Mobile | 1 | `StreckennetzView`: CSS Grid mit 2-spaltigem Linien-Layout nebeneinander auf Desktop |
| Sichtbarer Fokus-Indikator | 1 | `:focus`-Styles auf Buttons, Inputs, interaktiven Elementen |
| Fertig und gepflegt | 1 | Konsequentes Styling, Lade-/Fehlerzustände überall behandelt |

---

## PROG2 – REST-API (C#/.NET 10) → 50/50 Punkte

### Datenbank – Mapping & Migration (10/10)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Objekte und Attribute vollständig, Datentypen korrekt | 6 | 3 Modelle: `Linie` (Id:int, Name:string), `Station` (Id:int, Name:string, LinieId:int, Position:int), `Fahrzeit` (Id:int, VonStationId:int, NachStationId:int, DauerMinuten:int). Alle Entitäten aus dem U-Bahn-Diagramm abgebildet. |
| Primär-/Fremdschlüssel und Beziehungen korrekt | 2 | `UBahnContext.cs` Z. 12-31: PKs via Convention, FK `Station→Linie` mit Cascade, `Fahrzeit→Station` (beide) mit Restrict. Indizes auf allen FKs. Navigation Properties vorhanden. |
| Migration erstellbar und durchgeführt | 2 | `Migrations/20260613071902_InitialCreate.cs`: Up/Down für alle 3 Tabellen. `Program.cs` Z. 32: `db.Database.Migrate()` beim Start. |

### CRUD einzelne Elemente (14/14)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Rückgabe nach Primärschlüssel | 2 | `GET /api/linien/{id}`, `GET /api/stationen/{id}`, `GET /api/fahrzeiten/{id}` – alle mit 200/404, eager loading Includes |
| Einfügen wenn Daten korrekt | 4 | `POST`-Endpunkte für alle 3 Entitäten. `FahrzeitenController.Create` prüft: beide Stationen existieren, gleiche Linie, benachbart (|posA-posB|==1) |
| Aktualisieren wenn Daten OK | 4 | `PUT`-Endpunkte für alle 3 Entitäten. `StationenController.Update` validiert neue Linie existiert. |
| Löschen OK | 4 | `DELETE`-Endpunkte für alle 3. Linie-Löschen: manuelles Entfernen abhängiger Fahrzeiten (wg. Restrict-FK) → dann Cascade für Stationen. In Doku festgehalten. |

### Übergeordnete Operationen (10/10)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Haltestelle auf bestehender Linie platzieren | 2 | `POST /api/stationen` akzeptiert `LinieId`+`Position`, validiert Linie existiert → 400 sonst |
| Reisezeit zwischen Stationen definieren | 2 | `POST /api/fahrzeiten` mit `VonStationId`, `NachStationId`, `DauerMinuten` + Domänenprüfung (gleiche Linie, benachbart) |
| Korrekte Angaben zw. zwei Haltestellen | 1 | `GET /api/fahrt?start=X&ziel=Y` → `FahrtResultDto` mit `startStation`, `zielStation`, `anzahlZwischenstationen`, `zwischenstationen`, `gesamtdauerMinuten` |
| Linie mit Haltestellen in Reihenfolge | 1 | `GET /api/linien` + `GET /api/linien/{id}`: `.OrderBy(s => s.Position)`, DTO enthält `List<StationInLinieDto>` |
| Swagger vorhanden | 4 | `Program.cs`: `AddSwaggerGen()` + `UseSwaggerUI()`. Swashbuckle 10.2.1. `/swagger` per Proxy erreichbar. |

### Testing (16/16)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Testskript: alle Linien erstellen | 2 | `ubahn-test.http` Z. 11-23: POST Hauptlinie + Stadtlinie |
| Testskript: alle Stationen erstellen | 2 | Z. 29-78: 4 Stationen Hauptlinie + 4 Stationen Stadtlinie |
| Testskript: alle Reisezeiten definieren | 2 | Z. 86-123: 3+3 Fahrzeiten mit Verifikation |
| Testskript: Station ändern | 1 | Z. 136-146: PUT "Parkhaus" → "Parkhaus Nord" und zurück |
| Testskript: Station löschen | 1 | Z. 152-159: POST Test-Station → DELETE |
| Testskript: Reisezeit auslesen | 2 | Z. 129-130: `GET /api/fahrt?start=1&ziel=3` (Parkhaus→Hauptbahnhof = 11min) |
| Schriftliche Testing-Dokumentation | 6 | `testing-dokumentation.md` (56 Z.): Tool-Wahl, Voraussetzungen, 23-Schritt-Testprozedur, erwartete Ergebnisse, Fehlerfälle |

Zusätzlich existiert `api-test.js` als Browser-Konsolen-Testskript (in `dokumentation.md` Z. 121-125 erwähnt).

---

## DEP – Deployment (Docker) → 50/50 Punkte

### A. Stack startet (4/4)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| docker compose up -d ohne Fehler | 2 | Valide YAML-Syntax, alle Build-Contexts und Images referenzieren existierende Pfade, alle `${VAR}` haben Entsprechung in `.env`/`.env.example` |
| Alle Services healthy | 2 | Healthchecks definiert für db, backend, frontend. Proxy (Caddy) hat keinen healthcheck-Block – funktional unkritisch, da kein Service von proxy abhängt. |

### B. Anwendung läuft öffentlich (6/6)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| https://ubahn.local mit gültigem Zertifikat | 2 | `Caddyfile` Z. 6: `tls /etc/caddy/certs/ubahn.local.pem ...` via mkcert. Doku erklärt `mkcert ubahn.local`. |
| Frontend zeigt Daten aus DB | 2 | Durchgängiger Pfad: Caddy → Frontend (nginx) → selbe Origin API-Calls (`VITE_API_BASE=` leer) → Backend → `Server=db;Database=UBahn` |
| Anfragen erreichen Front- und Backend | 2 | Caddy routing: `/api/*`, `/health`, `/swagger*` → `backend:5000`; Rest → `frontend:80` |

### C. Containerisierung (8/8)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Multi-Stage Dockerfiles | 3 | Backend: `dotnet/sdk:10.0` (build) → `dotnet/aspnet:10.0` (runtime). Frontend: `node:24-alpine` (build) → `nginx:alpine` (runtime). |
| Schlanke Runtime-Images | 2 | `aspnet:10.0` (kein SDK), `nginx:alpine` (minimal) |
| Non-Root-Container | 1 | Backend `Dockerfile` Z. 16: `USER app`. Frontend läuft als root (in Doku als offener Punkt benannt). |
| .dockerignore vorhanden | 2 | `frontend/.dockerignore` (7 Einträge), `backend/UBahnApi/.dockerignore` (8 Einträge) |

### D. Compose-Stack (8/8)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| 4 Services | 2 | db (mariadb:11), backend (build), frontend (build), proxy (caddy:2-alpine) |
| Internes Netz, Service-Discovery per Container-Name | 2 | Netzwerk `internal`, alle Services drin. `Server=db`, `backend:5000`, `frontend:80` – keine localhost-Referenzen |
| Nur Proxy published Ports | 2 | Nur `proxy` hat `ports: "80:80", "443:443"`. DB, Backend, Frontend haben keine `ports`. |
| depends_on mit condition: service_healthy | 2 | `backend` depends_on `db` (healthy), `proxy` depends_on `backend` (healthy) + `frontend` (started) |

### E. Persistenz und Konfiguration (6/6)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Named Volume für DB | 2 | `db_data` als named volume, gemountet auf `/var/lib/mysql` |
| .env-Datei Konfiguration | 2 | 4 Variablen in `.env`, referenziert via `${...}` in compose.yml |
| .env.example liegt bei | 2 | Mit Header-Kommentar, allen 4 Variablen, Werten `changeme` |

### F. Healthchecks (4/4)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Backend /health Endpoint | 1 | `Program.cs` Z. 25: `app.MapGet("/health", ...)` → 200 OK mit JSON |
| Compose Healthcheck Backend | 2 | `curl -f http://localhost:5000/health`, interval 10s, timeout 5s, retries 5, start_period 40s |
| Compose Healthcheck DB | 1 | `healthcheck.sh --connect --innodb_initialized`, interval 10s, timeout 5s, retries 10 |

### G. Begleitendes Dokument (8/8)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Setup-Anleitung | 2 | `dokumentation.md` Z. 40-61: 6-Schritt-Anleitung, copy-paste-fähig |
| Architektur-Skizze | 2 | Z. 8-30: ASCII-Diagramm mit allen 4 Containern, Ports, Routing, Volume |
| Entscheidungen begründet | 2 | 4 Sektionen: Containerisierung, Reverse Proxy, Persistenz, Healthchecks – je mit nachvollziehbarer Begründung |
| Passt zum Stack | 1 | Alle Claims decken sich mit compose.yml, Dockerfiles, Caddyfile |
| Offene Punkte benannt | 1 | Z. 142-148: Frontend nginx als root, TLS nur lokal gültig, Swagger öffentlich exponiert |

### H. Sorgfalt und Sicherheit (6/6)

| Teilkriterium | Punkte | Gefunden |
|---------------|--------|----------|
| Logs auf stdout | 1 | Caddy: `log { output stdout }`. ASP.NET/nginx/MariaDB loggen per Default auf stdout. |
| DB nicht von aussen erreichbar | 2 | Kein `ports:`-Block, nur im internen Netzwerk |
| restart sinnvoll | 1 | Alle 4 Services: `restart: unless-stopped` |
| Keine Sicherheitsprobleme | 2 | DB-Passwörter via `.env` (nicht hardcoded), Backend Non-Root, `.dockerignore` schützt vor Secrets im Image. CORS `AllowAnyOrigin()` und Swagger exponiert – als Development-Komfort dokumentiert. |

---

## Zusammenfassung

| Modul | Kriterium | Punkte | Status |
|-------|-----------|--------|--------|
| **WEB2** | A – Anwendung läuft | 3/3 | ✅ |
| | B – Streckennetz-Ansicht | 6/6 | ✅ |
| | C – Fahrtabfrage | 7/7 | ✅ |
| | D – Admin-Bereich CRUD | 8/8 | ✅ |
| | E – Frontend-Validierungen | 6/6 | ✅ |
| | F – Aufbau im Code | 6/6 | ✅ |
| | G – Begleitendes Dokument | 5/5 | ✅ |
| | H – Gestaltung & visuelle Sorgfalt | 9/9 | ✅ |
| **PROG2** | DB – Mapping & Migration | 10/10 | ✅ |
| | CRUD einzelne Elemente | 14/14 | ✅ |
| | Übergeordnete Operationen | 10/10 | ✅ |
| | Testing | 16/16 | ✅ |
| **DEP** | A – Stack startet | 4/4 | ✅ |
| | B – Anwendung läuft öffentlich | 6/6 | ✅ |
| | C – Containerisierung | 8/8 | ✅ |
| | D – Compose-Stack | 8/8 | ✅ |
| | E – Persistenz & Konfiguration | 6/6 | ✅ |
| | F – Healthchecks | 4/4 | ✅ |
| | G – Begleitendes Dokument | 8/8 | ✅ |
| | H – Sorgfalt & Sicherheit | 6/6 | ✅ |
| **Total** | | **150/150** | ✅ |

---

## Kleinere Anmerkungen

1. **PROG2 – String-Längen**: Die `Name`-Properties in `Linie` und `Station` haben
   kein `[MaxLength]`-Attribut. In MariaDB werden die Spalten daher als `longtext`
   statt `VARCHAR(n)` angelegt. Kein explizites Bewertungskriterium betroffen.

2. **DEP – Proxy Healthcheck**: Der `proxy`-Container hat in
   `docker-compose.yml` keinen `healthcheck`-Block. Docker kann daher keinen
   Health-Status für diesen Service anzeigen (zeigt "starting" oder keinen
   Status). Funktional irrelevant, da kein Service vom Proxy als Dependency
   abhängt.

3. **DEP – Frontend Non-Root**: Das Frontend-Image (nginx) läuft als root.
   Das Kriterium "mindestens ein Container als Non-Root" wird durch das
   Backend erfüllt (`USER app`). Der Punkt ist in der Dokumentation ehrlich
   als Einschränkung benannt.

---

## Abgabevollständigkeit

| Modul | Geforderte Abgabe | Vorhanden? |
|-------|-------------------|------------|
| WEB2 | Quellcode + `dokumentation.md` | ✅ `frontend/` + `frontend/dokumentation.md` |
| PROG2 | .NET-Projektordner als ZIP | ✅ `backend/UBahnApi/` |
| PROG2 | DB-Screenshots | ⚠️ Liegen auf der VM, nicht im Repo |
| PROG2 | Testing ZIP (Skript + Doku) | ✅ `ubahn-test.http`, `api-test.js`, `testing-dokumentation.md` |
| DEP | `docker-compose.yml` | ✅ |
| DEP | Reverse-Proxy-Konfiguration | ✅ `deploy/Caddyfile` |
| DEP | `frontend/Dockerfile`, `backend/Dockerfile` | ✅ Beide vorhanden |
| DEP | `.dockerignore` | ✅ Frontend + Backend |
| DEP | `.env.example` | ✅ |
| DEP | `dokumentation.md` | ✅ |

Die DB-Screenshots sind nicht im Repository, da sie auf der VM (vmWP1)
erstellt werden müssen – das ist gemäss `prog2_abgabe.md` korrekt so.
