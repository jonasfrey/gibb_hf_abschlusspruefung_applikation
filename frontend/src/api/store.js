// Zentraler reaktiver Zustand (lightweight Store via Composition API).
// Buendelt die Stammdaten an einer Stelle und kapselt die Lade-/Mutations-
// Logik. Komponenten rufen ausschliesslich diese Actions auf und bleiben so
// frei von API-Details. Nach jeder Mutation wird neu geladen, damit Aenderungen
// ohne Seiten-Reload sichtbar sind.

import { reactive, readonly } from 'vue'
import { api } from './client.js'

const state = reactive({
  linien: [],
  stationen: [],
  fahrzeiten: [],
  loading: false,
  error: null,
})

async function loadAll() {
  state.loading = true
  state.error = null
  try {
    const [linien, stationen, fahrzeiten] = await Promise.all([
      api.getLinien(),
      api.getStationen(),
      api.getFahrzeiten(),
    ])
    state.linien = linien
    state.stationen = stationen
    state.fahrzeiten = fahrzeiten
  } catch (e) {
    state.error = e.message
  } finally {
    state.loading = false
  }
}

// Linien
async function createLinie(dto) { await api.createLinie(dto); await loadAll() }
async function updateLinie(id, dto) { await api.updateLinie(id, dto); await loadAll() }
async function deleteLinie(id) { await api.deleteLinie(id); await loadAll() }

// Stationen
async function createStation(dto) { await api.createStation(dto); await loadAll() }
async function updateStation(id, dto) { await api.updateStation(id, dto); await loadAll() }
async function deleteStation(id) { await api.deleteStation(id); await loadAll() }

// Fahrzeiten
async function createFahrzeit(dto) { await api.createFahrzeit(dto); await loadAll() }
async function updateFahrzeit(id, dto) { await api.updateFahrzeit(id, dto); await loadAll() }
async function deleteFahrzeit(id) { await api.deleteFahrzeit(id); await loadAll() }

export function useStore() {
  return {
    state: readonly(state),
    loadAll,
    createLinie, updateLinie, deleteLinie,
    createStation, updateStation, deleteStation,
    createFahrzeit, updateFahrzeit, deleteFahrzeit,
  }
}
