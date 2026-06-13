<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="app">
    <header class="topbar">
      <div class="brand">
        <span class="logo">Ⓜ</span>
        <span>U-Bahn</span>
      </div>
      <nav class="nav">
        <RouterLink to="/">Streckennetz</RouterLink>
        <RouterLink to="/fahrt">Fahrt abfragen</RouterLink>
        <RouterLink to="/admin" class="admin-link">Admin</RouterLink>
      </nav>
    </header>

    <main class="content">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--sp-4);
  padding: var(--sp-3) var(--sp-5);
  background: var(--surface);
  border-bottom: 1px solid var(--border);
  position: sticky;
  top: 0;
  z-index: 10;
  flex-wrap: wrap;
}
.brand {
  display: flex;
  align-items: center;
  gap: var(--sp-2);
  font-weight: 700;
  font-size: 1.15rem;
}
.logo {
  background: var(--primary);
  color: #fff;
  width: 32px;
  height: 32px;
  display: grid;
  place-items: center;
  border-radius: 50%;
  font-size: 1.1rem;
}
.nav { display: flex; gap: var(--sp-2); flex-wrap: wrap; }
.nav a {
  text-decoration: none;
  color: var(--text);
  padding: var(--sp-2) var(--sp-3);
  border-radius: var(--radius);
  font-weight: 600;
}
.nav a:hover { background: #eef1f5; }
.nav a.router-link-exact-active { background: var(--primary); color: #fff; }
.admin-link { border: 1px dashed var(--border); }
.admin-link.router-link-exact-active { border-color: transparent; }

.content {
  max-width: 1100px;
  margin: 0 auto;
  padding: var(--sp-5);
}
</style>
