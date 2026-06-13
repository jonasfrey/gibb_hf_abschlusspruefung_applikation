// Zentrale Validierungsregeln (Frontend).
// Das Backend (PRG2) validiert NICHT – daher liegt die gesamte Validierung hier.
// Jede Funktion liefert null (= gueltig) oder eine Fehlermeldung (String).

export const NAME_MIN = 2
export const NAME_MAX = 40
export const FAHRZEIT_MIN = 1
export const FAHRZEIT_MAX = 120

// Regel 1: Pflichtfeld – darf nicht leer sein.
export function pflicht(wert) {
  if (wert === null || wert === undefined || String(wert).trim() === '') {
    return 'Dieses Feld darf nicht leer sein.'
  }
  return null
}

// Regel 2: Stationsname – Mindest- und Maximallaenge.
export function nameLaenge(wert) {
  const v = String(wert ?? '').trim()
  if (v.length < NAME_MIN) return `Mindestens ${NAME_MIN} Zeichen.`
  if (v.length > NAME_MAX) return `Höchstens ${NAME_MAX} Zeichen.`
  return null
}

// Regel 3: Fahrzeit – positive Ganzzahl in plausiblem Bereich.
export function fahrzeitGueltig(wert) {
  if (wert === '' || wert === null || wert === undefined) return 'Fahrzeit ist erforderlich.'
  const n = Number(wert)
  if (!Number.isInteger(n)) return 'Fahrzeit muss eine Ganzzahl sein.'
  if (n < FAHRZEIT_MIN || n > FAHRZEIT_MAX) {
    return `Fahrzeit muss zwischen ${FAHRZEIT_MIN} und ${FAHRZEIT_MAX} Minuten liegen.`
  }
  return null
}

// Regel 4 (selbst gewaehlt): Stationsname innerhalb einer Linie eindeutig.
// vorhandene = Liste { id, name, linieId }; ignoreId fuer Bearbeiten der eigenen Station.
export function nameEindeutigInLinie(name, linieId, vorhandene, ignoreId = null) {
  const v = String(name ?? '').trim().toLowerCase()
  const kollision = vorhandene.some(s =>
    s.linieId === Number(linieId) &&
    s.id !== ignoreId &&
    s.name.trim().toLowerCase() === v)
  return kollision ? 'Auf dieser Linie existiert bereits eine Station mit diesem Namen.' : null
}

// Regel 5 (selbst gewaehlt): Position innerhalb einer Linie eindeutig & positiv.
export function positionGueltig(position, linieId, vorhandene, ignoreId = null) {
  const n = Number(position)
  if (!Number.isInteger(n) || n < 1) return 'Position muss eine positive Ganzzahl sein.'
  const kollision = vorhandene.some(s =>
    s.linieId === Number(linieId) &&
    s.id !== ignoreId &&
    s.position === n)
  return kollision ? 'Diese Position ist auf der Linie bereits belegt.' : null
}
