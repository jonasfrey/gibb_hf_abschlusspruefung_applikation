<script setup>
import { ref, computed } from 'vue'
import { useStore } from '../api/store.js'
import { pflicht, nameLaenge } from '../api/validators.js'

const store = useStore()

const neuName = ref('')
const bearbeiteId = ref(null)
const bearbeiteName = ref('')
const fehler = ref(null)

const neuFehler = computed(() => pflicht(neuName.value) ?? nameLaenge(neuName.value))
const bearbeiteFehler = computed(() => pflicht(bearbeiteName.value) ?? nameLaenge(bearbeiteName.value))

async function anlegen() {
  if (neuFehler.value) return
  fehler.value = null
  try {
    await store.createLinie({ name: neuName.value.trim() })
    neuName.value = ''
  } catch (e) { fehler.value = e.message }
}

function startBearbeiten(linie) {
  bearbeiteId.value = linie.id
  bearbeiteName.value = linie.name
}
function abbrechen() { bearbeiteId.value = null; bearbeiteName.value = '' }

async function speichern(id) {
  if (bearbeiteFehler.value) return
  fehler.value = null
  try {
    await store.updateLinie(id, { name: bearbeiteName.value.trim() })
    abbrechen()
  } catch (e) { fehler.value = e.message }
}

async function loeschen(linie) {
  const anzahl = linie.stationen.length
  const text = anzahl > 0
    ? `Linie "${linie.name}" löschen? Die ${anzahl} zugehörigen Stationen und deren Fahrzeiten werden mitgelöscht.`
    : `Linie "${linie.name}" löschen?`
  if (!confirm(text)) return
  fehler.value = null
  try { await store.deleteLinie(linie.id) } catch (e) { fehler.value = e.message }
}
</script>

<template>
  <div>
    <h2>Linien</h2>
    <p class="muted">Beim Löschen einer Linie werden ihre Stationen und Fahrzeiten mitgelöscht (kaskadierend).</p>

    <div v-if="fehler" class="alert alert-error">{{ fehler }}</div>

    <!-- Neue Linie -->
    <div class="card neu">
      <div class="field" style="flex:1;margin:0">
        <label for="neu-linie">Neue Linie</label>
        <input
          id="neu-linie"
          v-model="neuName"
          :class="{ invalid: neuName && neuFehler }"
          placeholder="z.B. Hauptlinie"
          @keyup.enter="anlegen"
        />
        <div v-if="neuName && neuFehler" class="err">{{ neuFehler }}</div>
      </div>
      <button class="btn" :disabled="!!neuFehler" @click="anlegen">Anlegen</button>
    </div>

    <!-- Liste -->
    <div class="card">
      <table>
        <thead>
          <tr><th>Name</th><th>Stationen</th><th class="aktionen">Aktionen</th></tr>
        </thead>
        <tbody>
          <tr v-for="linie in store.state.linien" :key="linie.id">
            <td>
              <template v-if="bearbeiteId === linie.id">
                <input
                  v-model="bearbeiteName"
                  :class="{ invalid: bearbeiteFehler }"
                  @keyup.enter="speichern(linie.id)"
                />
                <div v-if="bearbeiteFehler" class="err">{{ bearbeiteFehler }}</div>
              </template>
              <template v-else>{{ linie.name }}</template>
            </td>
            <td>{{ linie.stationen.length }}</td>
            <td class="aktionen">
              <template v-if="bearbeiteId === linie.id">
                <button class="btn btn-sm" :disabled="!!bearbeiteFehler" @click="speichern(linie.id)">Speichern</button>
                <button class="btn btn-secondary btn-sm" @click="abbrechen">Abbrechen</button>
              </template>
              <template v-else>
                <button class="btn btn-secondary btn-sm" @click="startBearbeiten(linie)">Bearbeiten</button>
                <button class="btn btn-danger btn-sm" @click="loeschen(linie)">Löschen</button>
              </template>
            </td>
          </tr>
          <tr v-if="store.state.linien.length === 0">
            <td colspan="3" class="muted">Noch keine Linien.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.neu { display: flex; gap: var(--sp-3); align-items: flex-end; margin-bottom: var(--sp-4); }
.aktionen { white-space: nowrap; display: flex; gap: var(--sp-2); }
</style>
