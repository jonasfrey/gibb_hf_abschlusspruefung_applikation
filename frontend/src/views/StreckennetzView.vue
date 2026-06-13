<script setup>
import { ref, computed, onMounted } from 'vue'
import { useStore } from '../api/store.js'
import StationDetail from '../components/StationDetail.vue'

const store = useStore()
const selected = ref(null)

onMounted(() => {
  if (store.state.linien.length === 0) store.loadAll()
})

const LINIENFARBEN = ['var(--linie-haupt)', 'var(--linie-stadt)', '#9b59b6', '#e67e22']

// Stationsnamen, die auf mehr als einer Linie vorkommen = Umsteigestationen.
const umsteigeNamen = computed(() => {
  const proName = new Map()
  for (const linie of store.state.linien) {
    for (const s of linie.stationen) {
      proName.set(s.name, (proName.get(s.name) ?? new Set()).add(linie.id))
    }
  }
  return new Set([...proName.entries()].filter(([, ids]) => ids.size > 1).map(([n]) => n))
})

function farbe(index) { return LINIENFARBEN[index % LINIENFARBEN.length] }
function istUmsteige(name) { return umsteigeNamen.value.has(name) }

function fahrzeitZuVorheriger(linie, position) {
  // Dauer zwischen Station an (position-1) und position auf derselben Linie.
  const stationen = linie.stationen
  const aktuell = stationen.find(s => s.position === position)
  const vorher = stationen.find(s => s.position === position - 1)
  if (!aktuell || !vorher) return null
  const fz = store.state.fahrzeiten.find(f =>
    (f.vonStationId === vorher.id && f.nachStationId === aktuell.id) ||
    (f.vonStationId === aktuell.id && f.nachStationId === vorher.id))
  return fz ? fz.dauerMinuten : null
}

function openDetail(station, linie, index) {
  selected.value = {
    station,
    linienName: linie.name,
    farbe: farbe(index),
    istUmsteige: istUmsteige(station.name),
    anzahlStationen: linie.stationen.length,
  }
}
</script>

<template>
  <section>
    <h1>Streckennetz</h1>
    <p class="muted">Übersicht aller Linien. Klicke auf eine Station für Details. Umsteigestationen sind markiert.</p>

    <div v-if="store.state.loading" class="alert alert-info">Lade Streckennetz…</div>
    <div v-else-if="store.state.error" class="alert alert-error">Fehler: {{ store.state.error }}</div>

    <div v-else class="netz">
      <div
        v-for="(linie, index) in store.state.linien"
        :key="linie.id"
        class="linie-spalte card"
      >
        <header class="linie-kopf">
          <span class="linie-farbe" :style="{ background: farbe(index) }"></span>
          <h2>{{ linie.name }}</h2>
        </header>

        <ol class="track" :style="{ '--farbe': farbe(index) }">
          <li
            v-for="station in [...linie.stationen].sort((a, b) => a.position - b.position)"
            :key="station.id"
            class="halt"
          >
            <span
              v-if="fahrzeitZuVorheriger(linie, station.position) !== null"
              class="fahrzeit"
            >{{ fahrzeitZuVorheriger(linie, station.position) }} min</span>

            <button
              class="station-btn"
              :class="{ umsteige: istUmsteige(station.name) }"
              @click="openDetail(station, linie, index)"
            >
              <span class="punkt"></span>
              <span class="name">{{ station.name }}</span>
              <span v-if="istUmsteige(station.name)" class="umsteige-badge">⇄ Umsteigen</span>
            </button>
          </li>
        </ol>
      </div>
    </div>

    <div v-if="!store.state.loading && store.state.linien.length === 0" class="alert alert-warn">
      Keine Linien vorhanden. Lege im Admin-Bereich Linien und Stationen an.
    </div>

    <StationDetail
      v-if="selected"
      v-bind="selected"
      @close="selected = null"
    />
  </section>
</template>

<style scoped>
.netz {
  display: flex;
  gap: var(--sp-5);
  flex-wrap: wrap;
  align-items: flex-start;
  margin-top: var(--sp-4);
}
.linie-spalte { flex: 1 1 280px; min-width: 260px; }
.linie-kopf { display: flex; align-items: center; gap: var(--sp-2); margin-bottom: var(--sp-4); }
.linie-kopf h2 { margin: 0; }
.linie-farbe { width: 24px; height: 8px; border-radius: 4px; }

.track { list-style: none; margin: 0; padding: 0; }
.halt { position: relative; padding-left: var(--sp-5); }

/* Vertikale Linie als durchgehende Schiene */
.halt::before {
  content: '';
  position: absolute;
  left: 9px;
  top: 0; bottom: 0;
  width: 4px;
  background: var(--farbe);
}
.halt:first-child::before { top: 50%; }
.halt:last-child::before { bottom: 50%; }

.fahrzeit {
  display: block;
  font-size: .75rem;
  color: var(--text-mut);
  padding: 2px 0 2px var(--sp-1);
  margin-left: -8px;
}

.station-btn {
  display: flex;
  align-items: center;
  gap: var(--sp-3);
  width: 100%;
  background: none;
  border: none;
  font: inherit;
  text-align: left;
  padding: var(--sp-2) var(--sp-2);
  border-radius: var(--radius);
  cursor: pointer;
}
.station-btn:hover { background: #eef1f5; }

.punkt {
  width: 16px; height: 16px;
  border-radius: 50%;
  background: #fff;
  border: 4px solid var(--farbe);
  flex: none;
  margin-left: -1px;
  position: relative; z-index: 1;
}
.station-btn.umsteige .punkt {
  border-color: var(--umsteige);
  width: 20px; height: 20px;
  box-shadow: 0 0 0 3px #fff, 0 0 0 5px var(--umsteige);
}
.name { font-weight: 600; }
.umsteige-badge {
  margin-left: auto;
  font-size: .72rem;
  font-weight: 700;
  color: #fff;
  background: var(--umsteige);
  padding: 2px var(--sp-2);
  border-radius: 999px;
}
</style>
