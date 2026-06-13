<script setup>
// Detail-Panel fuer eine angeklickte Station (Streckennetz-Ansicht).
defineProps({
  station: { type: Object, required: true },
  linienName: { type: String, default: '' },
  farbe: { type: String, default: 'var(--primary)' },
  istUmsteige: { type: Boolean, default: false },
  anzahlStationen: { type: Number, default: 0 },
})
defineEmits(['close'])
</script>

<template>
  <div class="overlay" @click.self="$emit('close')">
    <div class="panel card" role="dialog" aria-modal="true">
      <div class="head">
        <span class="dot" :style="{ background: farbe }"></span>
        <h2>{{ station.name }}</h2>
        <button class="btn btn-secondary btn-sm" @click="$emit('close')">Schliessen</button>
      </div>
      <dl class="details">
        <dt>Linie</dt><dd>{{ linienName }}</dd>
        <dt>Position</dt><dd>{{ station.position }} von {{ anzahlStationen }}</dd>
        <dt>Umsteigestation</dt>
        <dd>
          <span v-if="istUmsteige" class="badge" :style="{ background: 'var(--umsteige)' }">Ja – Umsteigen möglich</span>
          <span v-else class="muted">Nein</span>
        </dd>
      </dl>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed; inset: 0;
  background: rgba(20,28,38,.45);
  display: grid; place-items: center;
  padding: var(--sp-4); z-index: 50;
}
.panel { max-width: 420px; width: 100%; }
.head { display: flex; align-items: center; gap: var(--sp-3); margin-bottom: var(--sp-4); }
.head h2 { margin: 0; flex: 1; }
.dot { width: 18px; height: 18px; border-radius: 50%; flex: none; }
.details { display: grid; grid-template-columns: auto 1fr; gap: var(--sp-2) var(--sp-4); margin: 0; }
.details dt { font-weight: 600; color: var(--text-mut); }
.details dd { margin: 0; }
</style>
