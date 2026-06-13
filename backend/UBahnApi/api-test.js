// U-Bahn API Testskript fuer die Browser-Konsole (DevTools -> Console)
// Voraussetzung: Backend laeuft auf http://localhost:5000
// Ausfuehren: gesamtes Skript in die Konsole einfuegen und Enter.
//
// Das Skript ist eigenstaendig: es legt eine eigene Test-Linie mit Stationen
// und Fahrzeiten an, testet Abfrage / Aenderung / Loeschung und raeumt am
// Ende wieder auf. Es ist damit unabhaengig von den Seed-Daten.

const BASE = "http://localhost:5000";

let pass = 0, fail = 0;
const log = (ok, name, info = "") => {
  console.log(`%c${ok ? "PASS" : "FAIL"}%c  ${name}${info ? "  ->  " + info : ""}`,
    `color:white;background:${ok ? "#2e7d32" : "#c62828"};padding:2px 6px;border-radius:3px`,
    "color:inherit");
  ok ? pass++ : fail++;
};

async function api(method, path, body) {
  const res = await fetch(BASE + path, {
    method,
    headers: body ? { "Content-Type": "application/json" } : undefined,
    body: body ? JSON.stringify(body) : undefined,
  });
  let data = null;
  try { data = await res.json(); } catch { /* 204 etc. */ }
  return { status: res.status, data };
}

(async () => {
  console.log("%c=== U-Bahn API Test ===", "font-weight:bold;font-size:14px");

  // 1) Health
  let r = await api("GET", "/health");
  log(r.status === 200 && r.data?.status === "healthy", "Health-Check", `status ${r.status}`);

  // 2) Linie anlegen
  r = await api("POST", "/api/linien", { name: "Testlinie" });
  const linieId = r.data?.id;
  log(r.status === 201 && linieId > 0, "Linie anlegen (POST)", `id ${linieId}`);

  // 3) Stationen anlegen
  r = await api("POST", "/api/stationen", { name: "TestA", linieId, position: 1 });
  const sA = r.data?.id;
  log(r.status === 201, "Station A anlegen (POST)", `id ${sA}`);

  r = await api("POST", "/api/stationen", { name: "TestB", linieId, position: 2 });
  const sB = r.data?.id;
  log(r.status === 201, "Station B anlegen (POST)", `id ${sB}`);

  r = await api("POST", "/api/stationen", { name: "TestC", linieId, position: 3 });
  const sC = r.data?.id;
  log(r.status === 201, "Station C anlegen (POST)", `id ${sC}`);

  // 4) Fahrzeiten anlegen
  r = await api("POST", "/api/fahrzeiten", { vonStationId: sA, nachStationId: sB, dauerMinuten: 5 });
  log(r.status === 201, "Fahrzeit A->B anlegen (POST)", `5 min`);

  r = await api("POST", "/api/fahrzeiten", { vonStationId: sB, nachStationId: sC, dauerMinuten: 8 });
  log(r.status === 201, "Fahrzeit B->C anlegen (POST)", `8 min`);

  // 5) Linie mit Stationen in Reihenfolge abrufen
  r = await api("GET", `/api/linien/${linieId}`);
  const reihenfolge = (r.data?.stationen || []).map(s => s.name).join(" -> ");
  log(reihenfolge === "TestA -> TestB -> TestC", "Linie mit Stationen in Reihenfolge (GET)", reihenfolge);

  // 6) Fahrtabfrage A -> C (erwartet 13 min, 1 Zwischenstation: TestB)
  r = await api("GET", `/api/fahrt?start=${sA}&ziel=${sC}`);
  const ok6 = r.status === 200 && r.data?.gesamtdauerMinuten === 13
    && r.data?.anzahlZwischenstationen === 1 && r.data?.zwischenstationen?.[0] === "TestB";
  log(ok6, "Fahrtabfrage A->C (GET)", `${r.data?.gesamtdauerMinuten} min, zwischen: ${r.data?.zwischenstationen}`);

  // 7) Station bearbeiten (PUT)
  r = await api("PUT", `/api/stationen/${sB}`, { name: "TestB-neu", linieId, position: 2 });
  log(r.status === 204, "Station bearbeiten (PUT)", `status ${r.status}`);
  r = await api("GET", `/api/stationen/${sB}`);
  log(r.data?.name === "TestB-neu", "Aenderung verifizieren (GET)", r.data?.name);

  // 8) Fehlerfall: gleiche Start/Ziel
  r = await api("GET", `/api/fahrt?start=${sA}&ziel=${sA}`);
  log(r.status === 400, "Fehlerfall Start==Ziel (GET -> 400)", `status ${r.status}`);

  // 9) Aufraeumen: Linie loeschen (kaskadiert Stationen + Fahrzeiten)
  r = await api("DELETE", `/api/linien/${linieId}`);
  log(r.status === 204, "Testlinie loeschen (DELETE, kaskadiert)", `status ${r.status}`);
  r = await api("GET", `/api/linien/${linieId}`);
  log(r.status === 404, "Loeschung verifizieren (GET -> 404)", `status ${r.status}`);

  console.log(`%c=== Ergebnis: ${pass} bestanden, ${fail} fehlgeschlagen ===`,
    `font-weight:bold;font-size:14px;color:${fail ? "#c62828" : "#2e7d32"}`);
})();
