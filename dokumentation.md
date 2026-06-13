# DEP – Deployment U-Bahn-Stack

Multi-Container-Deployment der U-Bahn-Anwendung (Frontend + Backend + Datenbank
+ Reverse Proxy) mit Docker Compose und HTTPS über `https://ubahn.local`.

## Architektur-Skizze

```
                    Internet / Browser
                          │  HTTPS (443)
                          ▼
        ┌─────────────────────────────────────┐
        │      proxy  (Caddy)   :80/:443        │  ← einziger nach aussen
        │   TLS-Terminierung ubahn.local        │     veröffentlichter Dienst
        └───────────────┬───────────┬───────────┘
              /api/*     │           │   alles andere
              /health    │           │
              /swagger*  ▼           ▼
        ┌──────────────────┐   ┌──────────────────┐
        │ backend (.NET)   │   │ frontend (nginx) │
        │   :5000          │   │   :80            │
        └────────┬─────────┘   └──────────────────┘
                 │ Server=db
                 ▼
        ┌──────────────────┐
        │ db (MariaDB) :3306│   Named Volume: db_data
        └──────────────────┘

        Internes Netzwerk "internal" – Service-Discovery über Container-Namen.
```

## Voraussetzungen (vmLP1)

- Docker Engine + Docker Compose Plugin (vorhanden)
- mkcert installiert, Root-CA im System-Trust (`mkcert -install` ausgeführt)
- `/etc/hosts` enthält `127.0.0.1 ubahn.local` (vorhanden)

## Setup-Anleitung

```bash
# 1) Repository entpacken / wechseln in das Abgabeverzeichnis
cd ubahn

# 2) Umgebungsvariablen anlegen
cp .env.example .env
#   .env bei Bedarf anpassen (Passwörter)

# 3) TLS-Zertifikat für ubahn.local erzeugen
cd deploy/certs
mkcert ubahn.local          # erzeugt ubahn.local.pem + ubahn.local-key.pem
cd ../..

# 4) Stack starten
docker compose up -d

# 5) Status prüfen (alle Services healthy)
docker compose ps

# 6) Im Browser öffnen
#    https://ubahn.local
```

## Services

| Service   | Image / Build            | Aufgabe                              | Port nach aussen |
|-----------|--------------------------|--------------------------------------|------------------|
| proxy     | caddy:2-alpine           | HTTPS-Terminierung, Routing          | 80, 443          |
| frontend  | build ./frontend         | statisches Vue-Build via nginx       | – (nur intern)   |
| backend   | build ./backend/UBahnApi | REST-API (.NET 10)                   | – (nur intern)   |
| db        | mariadb:11               | Datenbank                            | – (nur intern)   |

Nur der **Proxy** veröffentlicht Ports. Backend, Frontend und DB sind
ausschliesslich im internen Netz `internal` erreichbar (Service-Discovery über
die Container-Namen `backend`, `frontend`, `db` – kein `localhost`).

## Containerisierung

**Backend** (`backend/UBahnApi/Dockerfile`) – Multi-Stage:
- Build-Stage `sdk:10.0`: restore + publish
- Runtime-Stage `aspnet:10.0`: nur das Publish-Ergebnis, **kein SDK**
- Läuft als **Non-Root-User** (`USER app`)
- `curl` für den Healthcheck nachinstalliert

**Frontend** (`frontend/Dockerfile`) – Multi-Stage:
- Build-Stage `node:24-alpine`: `npm ci` + `npm run build` (Vite)
- Runtime-Stage `nginx:alpine`: liefert das statische `dist/` aus
- `nginx.conf` mit SPA-Fallback (`try_files … /index.html`) für den Vue Router

Beide Projekte haben eine `.dockerignore`, damit `node_modules`, Build-Artefakte
und lokale Konfigurationsdateien nicht ins Image wandern.

## Reverse Proxy und HTTPS

- HTTPS-Terminierung für `ubahn.local` mit dem lokalen mkcert-Zertifikat
  (`deploy/Caddyfile`, Zertifikate unter `deploy/certs/`).
- **Routing-Schema:** `/api/*`, `/health` und `/swagger*` gehen ans Backend,
  alle übrigen Anfragen ans Frontend.
- Das Frontend wird mit **leerer API-Basis-URL** gebaut (`.env.production`),
  sodass es API-Aufrufe **same-origin** (`/api/...`) macht – diese landen über
  den Proxy beim Backend. Dadurch ist keine CORS-/Host-Konfiguration im Browser
  nötig.

## Persistenz und Konfiguration

- **Named Volume `db_data`** für die MariaDB-Daten → Daten überleben
  `docker compose down` und ein erneutes `up`.
- Konfiguration ausschliesslich über `.env` + `environment`-Block; **keine
  Secrets** in der `docker-compose.yml`.
- `.env.example` dokumentiert die nötigen Variablen, die echte `.env` ist nicht
  Teil der Abgabe.

## Healthchecks

- **Backend:** `/health`-Endpunkt, Compose-Healthcheck via `curl`.
- **Datenbank:** Compose-Healthcheck via `healthcheck.sh --connect
  --innodb_initialized` (MariaDB-Image).
- **Frontend:** zusätzlicher Healthcheck via `wget --spider`.
- **Proxy:** interner Health-Endpunkt `:2021/healthz` (nur im Container), via
  `wget --spider` geprüft, damit `docker compose ps` auch für den Proxy
  `healthy` zeigt.
- `depends_on` mit `condition: service_healthy` stellt sicher, dass das Backend
  erst nach der DB und der Proxy erst nach gesundem Backend startet.

## Sicherheit / Sorgfalt

- DB ist **nicht** von aussen erreichbar (kein veröffentlichter Port).
- Container-Logs gehen auf **stdout** (Docker-Logging; Caddy-Log explizit auf
  stdout konfiguriert).
- `restart: unless-stopped` für alle Services.
- Backend läuft als Non-Root.

## Reproduzierbarkeit

`docker compose down && docker compose up -d` (auch zweimal hintereinander)
reproduziert den Stack **ohne Datenverlust**, da die DB im Named Volume liegt
und das Backend Migration + Seed idempotent ausführt (Seed nur, wenn keine
Linien existieren).

## Eigener Code vs. Fallback

Es wird der **eigene Code** aus PRG2 (`backend/`) und WEB2 (`frontend/`)
verwendet, nicht die Fallback-Anwendung.

## Offene Punkte / Einschränkungen

- Das Frontend-Image (nginx) läuft mit dem Standard-nginx-User; der Non-Root-
  Nachweis wird über das Backend erbracht.
- Das TLS-Zertifikat ist an `ubahn.local` und die lokale mkcert-CA gebunden;
  ausserhalb dieser VM ist es nicht gültig.
- Die `/swagger`-Route ist auch im Stack erreichbar (Entwicklungskomfort); für
  ein echtes Produktivdeployment würde man sie schliessen.

## KI-Deklaration

Für die Erstellung von Code und Dokumentation wurde das KI-Werkzeug
**Claude Code** unterstützend eingesetzt.
