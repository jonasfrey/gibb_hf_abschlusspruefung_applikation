// Zentrale Datenzugriffsschicht (API-Client).
// Alle HTTP-Aufrufe an die PROG2-REST-API laufen ueber dieses Modul.
// Die UI-Komponenten kennen nur diese Funktionen, nicht fetch/URLs.
// Dadurch ist ein Austausch gegen einen Mock (siehe mock.js) trivial.

const BASE = import.meta.env.VITE_API_BASE ?? ''

async function request(method, path, body) {
  const res = await fetch(BASE + path, {
    method,
    headers: body ? { 'Content-Type': 'application/json' } : undefined,
    body: body ? JSON.stringify(body) : undefined,
  })

  if (!res.ok) {
    let message = `HTTP ${res.status}`
    try {
      const text = await res.text()
      if (text) message = text
    } catch { /* ignore */ }
    throw new Error(message)
  }

  if (res.status === 204) return null
  const text = await res.text()
  return text ? JSON.parse(text) : null
}

export const api = {
  // Linien
  getLinien: () => request('GET', '/api/linien'),
  getLinie: (id) => request('GET', `/api/linien/${id}`),
  createLinie: (dto) => request('POST', '/api/linien', dto),
  updateLinie: (id, dto) => request('PUT', `/api/linien/${id}`, dto),
  deleteLinie: (id) => request('DELETE', `/api/linien/${id}`),

  // Stationen
  getStationen: () => request('GET', '/api/stationen'),
  getStation: (id) => request('GET', `/api/stationen/${id}`),
  createStation: (dto) => request('POST', '/api/stationen', dto),
  updateStation: (id, dto) => request('PUT', `/api/stationen/${id}`, dto),
  deleteStation: (id) => request('DELETE', `/api/stationen/${id}`),

  // Fahrzeiten
  getFahrzeiten: () => request('GET', '/api/fahrzeiten'),
  createFahrzeit: (dto) => request('POST', '/api/fahrzeiten', dto),
  updateFahrzeit: (id, dto) => request('PUT', `/api/fahrzeiten/${id}`, dto),
  deleteFahrzeit: (id) => request('DELETE', `/api/fahrzeiten/${id}`),

  // Fahrtabfrage
  getFahrt: (start, ziel) => request('GET', `/api/fahrt?start=${start}&ziel=${ziel}`),
}
