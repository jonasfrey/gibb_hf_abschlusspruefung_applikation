<script setup>
import { ref, computed, onMounted } from 'vue'
import { useStore } from '../api/store.js'
import { api } from '../api/client.js'

const store = useStore()

const startId = ref('')
const zielId = ref('')
const resultat = ref(null)
const meldung = ref(null)   // { typ: 'info'|'warn'|'error', text }
const loading = ref(false)

onMounted(() => {
  if (store.state.stationen.length === 0) store.loadAll()
})

// Stationen gruppiert nach Linie fuer das Dropdown.
const stationenNachLinie = computed(() => {
  const gruppen = []
  for (const linie of store.state.linien) {
    gruppen.push({
      linie: linie.name,
      stationen: [...linie.stationen].sort((a, b) => a.position - b.position)
        .map(s => ({ id: s.id, name: s.name })),
    })
  }
  return gruppen
})

const koennenAbfragen = computed(() => startId.value !== '' && zielId.value !== '')

async function abfragen() {
  resultat.value = null
  meldung.value = null

  // Frontend-Vorpruefung: identische Stationen (Backend prueft ebenfalls).
  if (startId.value === zielId.value) {
    meldung.value = { typ: 'warn', text: 'Start- und Zielstation sind identisch. Bitte zwei verschiedene Stationen wählen.' }
    return
  }

  loading.value = true
  try {
    resultat.value = await api.getFahrt(Number(startId.value), Number(zielId.value))
  } catch (e) {
    // Backend liefert bei verschiedenen Linien / Fehlern eine Klartext-Meldung.
    meldung.value = { typ: 'warn', text: uebersetzeFehler(e.message) }
  } finally {
    loading.value = false
  }
}

function uebersetzeFehler(msg) {
  if (/Linien/i.test(msg)) {
    return 'Die gewählten Stationen liegen auf unterschiedlichen Linien. Umsteigen wird nicht unterstützt – bitte zwei Stationen derselben Linie wählen.'
  }
  if (/identisch/i.test(msg)) {
    return 'Start- und Zielstation sind identisch.'
  }
  return msg
}

function tauschen() {
  [startId.value, zielId.value] = [zielId.value, startId.value]
}
</script>

<template>
  <section class="wrap">
    <h1>Fahrt abfragen</h1>
    <p class="muted">Wähle Start- und Zielstation derselben Linie.</p>

    <div class="card form-card">
      <div class="field">
        <label for="start">Startstation</label>
        <select id="start" v-model="startId">
          <option value="" disabled>– wählen –</option>
          <optgroup v-for="g in stationenNachLinie" :key="g.linie" :label="g.linie">
            <option v-for="s in g.stationen" :key="s.id" :value="String(s.id)">{{ s.name }}</option>
          </optgroup>
        </select>
      </div>

      <div class="tausch">
        <button class="btn btn-secondary btn-sm" @click="tauschen" title="Start und Ziel tauschen">⇅ Tauschen</button>
      </div>

      <div class="field">
        <label for="ziel">Zielstation</label>
        <select id="ziel" v-model="zielId">
          <option value="" disabled>– wählen –</option>
          <optgroup v-for="g in stationenNachLinie" :key="g.linie" :label="g.linie">
            <option v-for="s in g.stationen" :key="s.id" :value="String(s.id)">{{ s.name }}</option>
          </optgroup>
        </select>
      </div>

      <button class="btn" :disabled="!koennenAbfragen || loading" @click="abfragen">
        {{ loading ? 'Suche…' : 'Fahrt suchen' }}
      </button>
    </div>

    <div v-if="meldung" class="alert" :class="`alert-${meldung.typ === 'error' ? 'error' : meldung.typ}`">
      {{ meldung.text }}
    </div>

    <div v-if="resultat" class="card resultat">
      <h2>Resultat</h2>
      <div class="route">
        <span class="von">{{ resultat.startStation }}</span>
        <span class="pfeil">→</span>
        <span class="nach">{{ resultat.zielStation }}</span>
      </div>
      <div class="kennzahlen">
        <div class="kpi">
          <span class="zahl">{{ resultat.gesamtdauerMinuten }}</span>
          <span class="label">Minuten</span>
        </div>
        <div class="kpi">
          <span class="zahl">{{ resultat.anzahlZwischenstationen }}</span>
          <span class="label">Zwischenstationen</span>
        </div>
      </div>
      <div v-if="resultat.zwischenstationen.length" class="zwischen">
        <h3>Zwischenstationen</h3>
        <ol>
          <li v-for="(z, i) in resultat.zwischenstationen" :key="i">{{ z }}</li>
        </ol>
      </div>
      <p v-else class="muted">Direkte Verbindung ohne Zwischenstationen.</p>
    </div>
  </section>
</template>

<style scoped>
.wrap { max-width: 560px; margin: 0 auto; }
.form-card { display: grid; gap: var(--sp-2); }
.tausch { display: flex; justify-content: center; }
.resultat { margin-top: var(--sp-4); }
.route {
  display: flex; align-items: center; gap: var(--sp-3);
  font-size: 1.25rem; font-weight: 700; margin-bottom: var(--sp-4);
  flex-wrap: wrap;
}
.pfeil { color: var(--primary); }
.kennzahlen { display: flex; gap: var(--sp-4); margin-bottom: var(--sp-4); }
.kpi {
  flex: 1;
  background: var(--bg);
  border-radius: var(--radius);
  padding: var(--sp-4);
  text-align: center;
}
.kpi .zahl { display: block; font-size: 2rem; font-weight: 800; color: var(--primary); }
.kpi .label { color: var(--text-mut); font-size: .85rem; }
.zwischen ol { margin: var(--sp-2) 0 0; padding-left: var(--sp-5); }
.zwischen li { padding: 2px 0; }
</style>
