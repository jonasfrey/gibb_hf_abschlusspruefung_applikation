<script setup>
import { ref, onMounted } from 'vue'
import { useStore } from '../api/store.js'
import LinienPanel from '../components/LinienPanel.vue'
import StationenPanel from '../components/StationenPanel.vue'
import FahrzeitenPanel from '../components/FahrzeitenPanel.vue'

const store = useStore()
const tab = ref('stationen')

onMounted(() => store.loadAll())
</script>

<template>
  <section>
    <div class="admin-head">
      <h1>Administration</h1>
      <span class="badge admin-badge">Mitarbeitende · Stammdatenverwaltung</span>
    </div>
    <p class="muted">Verwaltung der Linien, Stationen und Fahrzeiten. Änderungen sind sofort sichtbar.</p>

    <div class="tabs" role="tablist">
      <button :class="{ active: tab === 'stationen' }" role="tab" @click="tab = 'stationen'">Stationen</button>
      <button :class="{ active: tab === 'linien' }" role="tab" @click="tab = 'linien'">Linien</button>
      <button :class="{ active: tab === 'fahrzeiten' }" role="tab" @click="tab = 'fahrzeiten'">Fahrzeiten</button>
    </div>

    <div v-if="store.state.loading" class="alert alert-info">Lade Daten…</div>

    <div class="panel-bereich">
      <StationenPanel v-show="tab === 'stationen'" />
      <LinienPanel v-show="tab === 'linien'" />
      <FahrzeitenPanel v-show="tab === 'fahrzeiten'" />
    </div>
  </section>
</template>

<style scoped>
.admin-head { display: flex; align-items: center; gap: var(--sp-3); flex-wrap: wrap; }
.admin-badge { background: var(--text); }
.tabs {
  display: flex;
  gap: var(--sp-1);
  border-bottom: 2px solid var(--border);
  margin: var(--sp-4) 0 var(--sp-5);
  flex-wrap: wrap;
}
.tabs button {
  font: inherit;
  font-weight: 600;
  padding: var(--sp-3) var(--sp-4);
  border: none;
  background: none;
  cursor: pointer;
  color: var(--text-mut);
  border-bottom: 3px solid transparent;
  margin-bottom: -2px;
}
.tabs button.active { color: var(--primary); border-bottom-color: var(--primary); }
.tabs button:hover { color: var(--text); }
</style>
