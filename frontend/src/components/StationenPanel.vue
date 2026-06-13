<script setup>
import { ref, computed } from 'vue'
import { useStore } from '../api/store.js'
import {
  pflicht, nameLaenge, nameEindeutigInLinie, positionGueltig,
} from '../api/validators.js'

const store = useStore()
const fehler = ref(null)

// Formularzustand (sowohl fuer Neuanlage als auch Bearbeiten).
const form = ref(leeresFormular())
const bearbeiteId = ref(null)

function leeresFormular() {
  return { name: '', linieId: '', position: '' }
}

const alleStationen = computed(() =>
  store.state.stationen.map(s => ({ id: s.id, name: s.name, linieId: s.linieId, position: s.position })))

// Einzelne Feldfehler (am Feld angezeigt).
const errName = computed(() =>
  pflicht(form.value.name)
  ?? nameLaenge(form.value.name)
  ?? nameEindeutigInLinie(form.value.name, form.value.linieId, alleStationen.value, bearbeiteId.value))

const errLinie = computed(() => pflicht(form.value.linieId))

const errPosition = computed(() => {
  const p = pflicht(form.value.position)
  if (p) return p
  if (!form.value.linieId) return null
  return positionGueltig(form.value.position, form.value.linieId, alleStationen.value, bearbeiteId.value)
})

const istGueltig = computed(() => !errName.value && !errLinie.value && !errPosition.value)

// Naechste freie Position auf der gewaehlten Linie vorschlagen.
function naechstePosition(linieId) {
  const auf = alleStationen.value.filter(s => s.linieId === Number(linieId))
  return auf.length ? Math.max(...auf.map(s => s.position)) + 1 : 1
}

function neuStarten(linieId = '') {
  bearbeiteId.value = null
  form.value = leeresFormular()
  if (linieId) {
    form.value.linieId = String(linieId)
    form.value.position = String(naechstePosition(linieId))
  }
}

function bearbeitenStarten(station) {
  bearbeiteId.value = station.id
  form.value = {
    name: station.name,
    linieId: String(station.linieId),
    position: String(station.position),
  }
}

async function speichern() {
  if (!istGueltig.value) return
  fehler.value = null
  const dto = {
    name: form.value.name.trim(),
    linieId: Number(form.value.linieId),
    position: Number(form.value.position),
  }
  try {
    if (bearbeiteId.value) await store.updateStation(bearbeiteId.value, dto)
    else await store.createStation(dto)
    neuStarten()
  } catch (e) { fehler.value = e.message }
}

async function loeschen(station) {
  if (!confirm(`Station "${station.name}" löschen? Zugehörige Fahrzeiten werden entfernt.`)) return
  fehler.value = null
  try {
    await store.deleteStation(station.id)
    if (bearbeiteId.value === station.id) neuStarten()
  } catch (e) { fehler.value = e.message }
}

// Reihenfolge: Station eine Position nach oben/unten tauschen.
async function verschieben(station, richtung) {
  const aufLinie = alleStationen.value
    .filter(s => s.linieId === station.linieId)
    .sort((a, b) => a.position - b.position)
  const idx = aufLinie.findIndex(s => s.id === station.id)
  const nachbarIdx = idx + richtung
  if (nachbarIdx < 0 || nachbarIdx >= aufLinie.length) return
  const nachbar = aufLinie[nachbarIdx]
  fehler.value = null
  try {
    // Positionen tauschen. Zwischenschritt mit temporaerer Position vermeidet
    // Eindeutigkeitskonflikt waere DB-seitig kein Problem (kein Unique-Index),
    // wir tauschen daher direkt.
    await store.updateStation(station.id, { name: station.name, linieId: station.linieId, position: nachbar.position })
    await store.updateStation(nachbar.id, { name: nachbar.name, linieId: nachbar.linieId, position: station.position })
  } catch (e) { fehler.value = e.message }
}

const stationenSortiert = computed(() =>
  [...store.state.stationen].sort((a, b) =>
    a.linieId - b.linieId || a.position - b.position))

function linieName(id) {
  return store.state.linien.find(l => l.id === id)?.name ?? '—'
}
</script>

<template>
  <div>
    <h2>Stationen</h2>

    <div v-if="fehler" class="alert alert-error">{{ fehler }}</div>

    <!-- Formular (Neu / Bearbeiten) – entspricht Mockup "Station bearbeiten" -->
    <div class="card formular">
      <h3>{{ bearbeiteId ? 'Station bearbeiten' : 'Neue Station' }}</h3>

      <div class="field">
        <label for="st-name">Name</label>
        <input
          id="st-name"
          v-model="form.name"
          :class="{ invalid: form.name && errName }"
          placeholder="z.B. Hauptbahnhof"
        />
        <div v-if="form.name && errName" class="err">{{ errName }}</div>
      </div>

      <div class="row2">
        <div class="field">
          <label for="st-linie">Linie</label>
          <select
            id="st-linie"
            v-model="form.linieId"
            :class="{ invalid: form.linieId === '' && errLinie && form.name }"
            @change="!bearbeiteId && (form.position = String(naechstePosition(form.linieId)))"
          >
            <option value="" disabled>– wählen –</option>
            <option v-for="l in store.state.linien" :key="l.id" :value="String(l.id)">{{ l.name }}</option>
          </select>
          <div v-if="form.name && errLinie" class="err">{{ errLinie }}</div>
        </div>

        <div class="field">
          <label for="st-pos">Position</label>
          <input
            id="st-pos"
            v-model="form.position"
            type="number" min="1"
            :class="{ invalid: form.position && errPosition }"
          />
          <div v-if="form.position && errPosition" class="err">{{ errPosition }}</div>
        </div>
      </div>

      <div class="row">
        <button class="btn" :disabled="!istGueltig" @click="speichern">
          {{ bearbeiteId ? 'Speichern' : 'Anlegen' }}
        </button>
        <button v-if="bearbeiteId" class="btn btn-secondary" @click="neuStarten()">Abbrechen</button>
      </div>
    </div>

    <!-- Liste -->
    <div class="card">
      <table>
        <thead>
          <tr><th>Linie</th><th>Pos.</th><th>Name</th><th class="aktionen">Aktionen</th></tr>
        </thead>
        <tbody>
          <tr v-for="station in stationenSortiert" :key="station.id" :class="{ aktiv: bearbeiteId === station.id }">
            <td>{{ linieName(station.linieId) }}</td>
            <td>{{ station.position }}</td>
            <td>{{ station.name }}</td>
            <td class="aktionen">
              <button class="btn btn-secondary btn-sm" title="nach oben" @click="verschieben(station, -1)">↑</button>
              <button class="btn btn-secondary btn-sm" title="nach unten" @click="verschieben(station, 1)">↓</button>
              <button class="btn btn-secondary btn-sm" @click="bearbeitenStarten(station)">Bearbeiten</button>
              <button class="btn btn-danger btn-sm" @click="loeschen(station)">Löschen</button>
            </td>
          </tr>
          <tr v-if="store.state.stationen.length === 0">
            <td colspan="4" class="muted">Noch keine Stationen.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.formular { margin-bottom: var(--sp-4); }
.row2 { display: flex; gap: var(--sp-4); }
.row2 .field { flex: 1; }
.aktionen { white-space: nowrap; display: flex; gap: var(--sp-1); }
tr.aktiv { background: #eef5fc; }
</style>
