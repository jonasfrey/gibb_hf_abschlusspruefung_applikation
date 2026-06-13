import { createRouter, createWebHistory } from 'vue-router'
import StreckennetzView from '../views/StreckennetzView.vue'
import FahrtView from '../views/FahrtView.vue'
import AdminView from '../views/AdminView.vue'

const routes = [
  { path: '/', name: 'streckennetz', component: StreckennetzView },
  { path: '/fahrt', name: 'fahrt', component: FahrtView },
  { path: '/admin', name: 'admin', component: AdminView },
]

export default createRouter({
  history: createWebHistory(),
  routes,
})
