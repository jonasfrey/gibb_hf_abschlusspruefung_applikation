# WEB2 – U-Bahn Frontend (Vue 3)

## Startanleitung

Voraussetzung: Das PRG2-Backend läuft und ist erreichbar (Standard: `http://localhost:5000`).

```bash
cd frontend
npm install
npm run dev      # Entwicklungsserver auf http://localhost:5173
```

Produktions-Build:
```bash
npm run build    # erzeugt statische Dateien in dist/
npm run preview  # Vorschau des Builds
```

Die Basis-URL der API wird über die Umgebungsvariable `VITE_API_BASE` gesetzt
(Datei `.env`). Standard ist `http://localhost:5000`. Im Docker-Stack (DEP) wird
sie auf den Reverse-Proxy-Pfad gesetzt.

## Technologie

- **Vue 3** mit **Composition API** (`<script setup>`)
- **Vue Router** für die Navigation (drei Routen)
- **Vite** als Build-Werkzeug
- **Kein UI-Framework** – eigenes CSS-Design-System mit CSS-Variablen

## Projektaufbau

```
frontend/
├── index.html
├── vite.config.js
├── .env / .env.example          – API-Basis-URL
└── src/
    ├── main.js                  – App-Einstieg, Router + globales CSS
    ├── App.vue                  – Layout, Navigation (Reisende / Admin)
    ├── assets/main.css          – Design-System (Farben, Buttons, Felder, …)
    ├── router/index.js          – Routendefinition
    ├── api/
    │   ├── client.js            – API-Client (gekapselte fetch-Aufrufe)
    │   ├── store.js             – zentraler reaktiver Zustand + Actions
    │   └── validators.js        – Validierungsregeln (zentral)
    ├── views/
    │   ├── StreckennetzView.vue – Liniennetz, interaktive Stationen
    │   ├── FahrtView.vue        – Fahrtabfrage
    │   └── AdminView.vue        – Admin-Bereich mit Tabs
    └── components/
        ├── StationDetail.vue    – Detail-Panel einer Station
        ├── LinienPanel.vue      – CRUD Linien
        ├── StationenPanel.vue   – CRUD Stationen + Reihenfolge
        └── FahrzeitenPanel.vue  – CRUD Fahrzeiten
```

## Architekturentscheidungen

**Trennung Logik / Darstellung.** Sämtliche HTTP-Aufrufe liegen in
`api/client.js`. Die Komponenten kennen keine URLs oder `fetch`, sondern rufen
nur Funktionen wie `api.getLinien()` auf. Dadurch ist ein Austausch gegen einen
Mock (WEB2-Mock-Fallback) trivial: ein Objekt mit identischen Signaturen genügt.

**Zentraler Zustand.** `api/store.js` ist ein leichtgewichtiger Store auf Basis
von `reactive`/`readonly` (Composition API). Er bündelt Linien, Stationen und
Fahrzeiten an einer Stelle. Bewusst kein Pinia: für diesen Umfang wäre das
Overhead. Nach jeder Mutation lädt der Store die Daten neu (`loadAll`), wodurch
Änderungen **ohne Seiten-Reload** überall sichtbar sind.

**Streckennetz als schematische Grafik.** Stationen liegen als Punkte auf einer
vertikalen, farbigen Linie (CSS). Umsteigestationen werden automatisch erkannt
(Stationsname kommt auf mehr als einer Linie vor) und orange hervorgehoben.

**Komponentenschnitt.** Jede CRUD-Ressource hat ein eigenes Panel. Der
Admin-Bereich gruppiert sie über Tabs. Wiederkehrende UI (Buttons, Felder,
Karten, Tabellen, Alerts) ist als CSS-Klassen im Design-System definiert.

## Validierungsregeln

Das Backend (PRG2) validiert **nicht** – die gesamte Validierung liegt im
Frontend (`api/validators.js`). Umgesetzte Regeln:

| Regel | Ort | Begründung |
|-------|-----|------------|
| **Pflichtfeld** (Name, Linie, Position dürfen nicht leer sein) | alle Formulare | Verhindert das Anlegen unvollständiger Datensätze, die im Netz/in der Abfrage zu Lücken führen. |
| **Namenslänge** 2–40 Zeichen | Station, Linie | Schützt vor leeren/sinnlosen Einträgen und vor zu langen Namen, die das Layout (Liniengrafik) sprengen. |
| **Fahrzeit: positive Ganzzahl 1–120** | Fahrzeit | Eine Fahrzeit von 0, negativ oder unrealistisch hoch ist fachlich falsch; die Fahrtabfrage summiert Fahrzeiten. |
| **Stationsname je Linie eindeutig** *(selbst gewählt)* | Station | Zwei gleichnamige Stationen auf einer Linie wären für Reisende nicht unterscheidbar und brächen die Fahrtabfrage. |
| **Position je Linie eindeutig & positiv** *(selbst gewählt)* | Station | Die Reihenfolge auf einer Linie muss eindeutig sein, sonst ist die Stationsabfolge und damit die Fahrtberechnung mehrdeutig. |

Bei ungültiger Eingabe ist der Speichern-Button **deaktiviert** und die
Fehlermeldung erscheint **direkt am betroffenen Feld** (rote Umrandung + Text).

## Lösch-Verhalten Linie mit Stationen

Beim Löschen einer Linie werden ihre Stationen und deren Fahrzeiten
**mitgelöscht** (kaskadierend). Vor dem Löschen erscheint eine Bestätigung mit
Angabe, wie viele Stationen betroffen sind. Begründung: Eine Linie ohne ihre
Stationen ist fachlich bedeutungslos; verwaiste Stationen ohne Linie wären
inkonsistent. Diese Entscheidung ist im Backend ebenso umgesetzt.

## Reihenfolge der Stationen

Im Stationen-Panel lässt sich die Reihenfolge über ↑/↓-Buttons verändern. Dabei
werden die Positionswerte zweier benachbarter Stationen getauscht und einzeln
per PUT gespeichert.

## Mock-Fallback

Es wird das echte Backend verwendet, kein Mock. Die Architektur erlaubt jedoch
einen Mock ohne Änderung der UI: man ersetzt das exportierte `api`-Objekt in
`api/client.js` durch ein Objekt mit denselben Funktionssignaturen.

## Offene Punkte / Einschränkungen

- Es gibt keine Authentifizierung im Admin-Bereich (laut Aufgabe nicht gefordert).
- Die Fahrtabfrage unterstützt **kein Umsteigen** (gemäss Anforderung). Liegen
  Start und Ziel auf verschiedenen Linien, erscheint eine erklärende Meldung.
- Die Eindeutigkeits-Validierung (Name/Position) prüft gegen die aktuell
  geladenen Daten im Store. Bei gleichzeitiger Bearbeitung durch mehrere
  Personen könnte es theoretisch zu Kollisionen kommen (kein Sperrmechanismus).
- Die Reihenfolge-Änderung macht zwei PUT-Aufrufe; bei einem Fehler dazwischen
  könnte ein inkonsistenter Zwischenzustand entstehen (für den Prüfungsumfang
  vertretbar).

## KI-Deklaration

Für die Erstellung von Code und Dokumentation wurde das KI-Werkzeug
**Claude Code** unterstützend eingesetzt.
