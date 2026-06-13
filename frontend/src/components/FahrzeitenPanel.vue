<script setup>
import { ref, computed } from 'vue'
import { useStore } from '../api/store.js'
import { pflicht, fahrzeitGueltig } from '../api/validators.js'

const store = useStore()
const fehler = ref(null)

const form = ref({ vonStationId: '', nachStationId: '', dauerMinuten: '' })
const bearbeiteId = ref(null)

// Stationen nach Linie + Position fuer die Dropdowns.
const stationenSortiert = computed(() =>
  [...store.state.stationen].sort((a, b) => a.linieId - b.linieId || a.position - b.position))

function stationLabel(s) {
  const linie = store.state.linien.find(l => l.id === s.linieId)?.name ?? '?'
  return `${linie} · ${s.position}. ${s.name}`
}
function stationById(id) {
  return store.state.stationen.find(s => s.id === Number(id))
}

const errVon = computed(() => pflicht(form.value.vonStationId))
const errNach = computed(() => {
  const p = pflicht(form.value.nachStationId)
  if (p) return p
  if (form.value.vonStationId && form.value.nachStationId === form.value.vonStationId) {
    return 'Von- und Nach-Station müssen unterschiedlich sein.'
  }
  // Plausibilitaet: gleiche Linie und direkt benachbart (Backend verlangt das).
  const von = stationById(form.value.vonStationId)
  const nach = stationById(form.value.nachStationId)
  if (von && nach) {
    if (von.linieId !== nach.linieId) return 'Beide Stationen müssen auf derselben Linie liegen.'
    if (Math.abs(von.position - nach.position) !== 1) return 'Nur direkt aufeinanderfolgende Stationen sind erlaubt.'
  }
  return null
})
const errDauer = computed(() => fahrzeitGueltig(form.value.dauerMinuten))

const istGueltig = computed(() => !errVon.value && !errNach.value && !errDauer.value)

function reset() {
  bearbeiteId.value = null
  form.value = { vonStationId: '', nachStationId: '', dauerMinuten: '' }
}

function bearbeitenStarten(fz) {
  bearbeiteId.value = fz.id
  form.value = {
    vonStationId: String(fz.vonStationId),
    nachStationId: String(fz.nachStationId),
    dauerMinuten: String(fz.dauerMinuten),
  }
}

async function speichern() {
  if (!istGueltig.value) return
  fehler.value = null
  const dto = {
    vonStationId: Number(form.value.vonStationId),
    nachStationId: Number(form.value.nachStationId),
    dauerMinuten: Number(form.value.dauerMinuten),
  }
  try {
    if (bearbeiteId.value) await store.updateFahrzeit(bearbeiteId.value, dto)
    else await store.createFahrzeit(dto)
    reset()
  } catch (e) { fehler.value = e.message }
}

async function loeschen(fz) {
  if (!confirm(`Fahrzeit ${fz.vonStationName} → ${fz.nachStationName} löschen?`)) return
  fehler.value = null
  try {
    await store.deleteFahrzeit(fz.id)
    if (bearbeiteId.value === fz.id) reset()
  } catch (e) { fehler.value = e.message }
}
</script>

<template>
  <div>
    <h2>Fahrzeiten</h2>
    <p class="muted">Fahrzeiten gelten zwischen direkt aufeinanderfolgenden Stationen derselben Linie.</p>

    <div v-if="fehler" class="alert alert-error">{{ fehler }}</div>

    <div class="card formular">
      <h3>{{ bearbeiteId ? 'Fahrzeit bearbeiten' : 'Neue Fahrzeit' }}</h3>

      <div class="field">
        <label for="fz-von">Von Station</label>
        <select id="fz-von" v-model="form.vonStationId" :class="{ invalid: form.vonStationId === '' && errVon && form.dauerMinuten }">
          <option value="" disabled>– wählen –</option>
          <option v-for="s in stationenSortiert" :key="s.id" :value="String(s.id)">{{ stationLabel(s) }}</option>
        </select>
      </div>

      <div class="field">
        <label for="fz-nach">Nach Station</label>
        <select id="fz-nach" v-model="form.nachStationId" :class="{ invalid: form.nachStationId && errNach }">
          <option value="" disabled>– wählen –</option>
          <option v-for="s in stationenSortiert" :key="s.id" :value="String(s.id)">{{ stationLabel(s) }}</option>
        </select>
        <div v-if="form.nachStationId && errNach" class="err">{{ errNach }}</div>
      </div>

      <div class="field">
        <label for="fz-dauer">Dauer (Minuten)</label>
        <input id="fz-dauer" v-model="form.dauerMinuten" type="number" min="1"
               :class="{ invalid: form.dauerMinuten && errDauer }" />
        <div v-if="form.dauerMinuten && errDauer" class="err">{{ errDauer }}</div>
      </div>

      <div class="row">
        <button class="btn" :disabled="!istGueltig" @click="speichern">
          {{ bearbeiteId ? 'Speichern' : 'Anlegen' }}
        </button>
        <button v-if="bearbeiteId" class="btn btn-secondary" @click="reset">Abbrechen</button>
      </div>
    </div>

    <div class="card">
      <table>
        <thead>
          <tr><th>Von</th><th>Nach</th><th>Dauer</th><th class="aktionen">Aktionen</th></tr>
        </thead>
        <tbody>
          <tr v-for="fz in store.state.fahrzeiten" :key="fz.id">
            <td>{{ fz.vonStationName }}</td>
            <td>{{ fz.nachStationName }}</td>
            <td>{{ fz.dauerMinuten }} min</td>
            <td class="aktionen">
              <button class="btn btn-secondary btn-sm" @click="bearbeitenStarten(fz)">Bearbeiten</button>
              <button class="btn btn-danger btn-sm" @click="loeschen(fz)">Löschen</button>
            </td>
          </tr>
          <tr v-if="store.state.fahrzeiten.length === 0">
            <td colspan="4" class="muted">Noch keine Fahrzeiten.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.formular { margin-bottom: var(--sp-4); }
.aktionen { white-space: nowrap; display: flex; gap: var(--sp-2); }
</style>
