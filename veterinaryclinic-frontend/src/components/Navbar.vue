<template>
  <nav class="navbar">
    <div class="container nav-content">
      <div class="brand-group">
        <router-link to="/" class="brand">
          <span class="brand-icon">
            <svg viewBox="0 0 64 64" aria-hidden="true">
              <circle cx="20" cy="18" r="6" />
              <circle cx="42" cy="18" r="6" />
              <circle cx="14" cy="34" r="5" />
              <circle cx="48" cy="34" r="5" />
              <path d="M32 26c-8 0-16 6-16 14 0 7 5 12 12 12 4 0 6-2 8-4 2 2 4 4 8 4 7 0 12-5 12-12 0-8-8-14-16-14Z" />
              <path class="cross-v" d="M32 31v10" />
              <path class="cross-h" d="M27 36h10" />
            </svg>
          </span>
          <span class="brand-text">
            <span class="brand-title">VetCare</span>
            <span class="brand-subtitle">veterinary clinic</span>
          </span>
        </router-link>
        <router-link to="/adoptii" class="adopt-btn">Adoptă un animal!</router-link>
      </div>

      <div class="nav-links">
        <router-link :to="authStore.isAuthenticated ? '/dashboard' : '/'" class="nav-link">Home</router-link>
        <router-link to="/despre-noi" class="nav-link">Despre noi</router-link>
        <router-link to="/contact" class="nav-link">Contact</router-link>

        <template v-if="!authStore.isAuthenticated">
          <router-link to="/login" class="nav-link">Login</router-link>
          <router-link to="/register" class="btn-register">Register</router-link>
        </template>

        <template v-else>
          <div class="account-box">
            <div class="account-dropdown" ref="dropdownRef">
              <button class="account-trigger" @click="toggleDropdown" type="button">
                <div class="avatar">{{ userInitial }}</div>
                <span class="account-name profile-tooltip" data-tooltip="Contul meu">
                  {{ authStore.user?.name || 'Contul meu' }}
                </span>
                <span class="dropdown-arrow" :class="{ open: isDropdownOpen }">▾</span>
              </button>

              <div v-if="isDropdownOpen" class="dropdown-menu">
                <router-link :to="profileRoute" class="dropdown-item" @click="closeDropdown">
                  👤 Profilul meu
                </router-link>

                <router-link
                  v-if="authStore.user?.role === UserRole.Admin"
                  to="/admin/account-requests"
                  class="dropdown-item requests-item"
                  @click="closeDropdown"
                >
                  🔔 Cereri cont
                  <span v-if="pendingCount > 0" class="badge">{{ pendingCount }}</span>
                </router-link>

                <div class="dropdown-divider" />

                <button class="dropdown-item logout-item" type="button" @click="handleLogout">
                  🚪 Logout
                </button>
              </div>
            </div>
          </div>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed, onMounted, onBeforeUnmount, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { UserRole } from '../types/auth'
import api from '../api/axios'

const authStore = useAuthStore()
const router = useRouter()

const isDropdownOpen = ref(false)
const dropdownRef = ref<HTMLElement | null>(null)
const pendingCount = ref(0)

const userInitial = computed(() => authStore.user?.name?.charAt(0).toUpperCase() || 'U')

const profileRoute = computed(() => {
  const role = authStore.user?.role
  if (role === UserRole.Doctor) return '/doctor/profile'
  if (role === UserRole.Client) return '/client/profile'
  if (role === UserRole.Admin) return '/admin/profile'
  return '/profile'
})

async function loadPendingCount() {
  if (authStore.user?.role !== UserRole.Admin) return
  try {
    const res = await api.get('/AccountApproval/pending')
    pendingCount.value = res.data.length
  } catch {
    pendingCount.value = 0
  }
}

const toggleDropdown = () => { isDropdownOpen.value = !isDropdownOpen.value }
const closeDropdown = () => { isDropdownOpen.value = false }

const handleClickOutside = (event: MouseEvent) => {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    closeDropdown()
  }
}

const handleLogout = () => {
  closeDropdown()
  authStore.logout()
  router.push('/login')
}

onMounted(() => {
  authStore.loadUser()
  document.addEventListener('click', handleClickOutside)
  loadPendingCount()
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})

watch(() => authStore.user, () => { loadPendingCount() })
</script>

<style scoped>
.navbar {
  background: rgba(255,255,255,0.92);
  backdrop-filter: blur(10px);
  border-bottom: 1px solid #f4cada;
  position: sticky;
  top: 0;
  z-index: 1000;
}
.container { width: min(1120px, calc(100% - 32px)); margin: 0 auto; }
.nav-content { min-height: 82px; display: flex; align-items: center; justify-content: space-between; gap: 20px; }
.brand-group { display: flex; align-items: center; gap: 14px; }
.brand { display: inline-flex; align-items: center; gap: 14px; text-decoration: none; }
.brand-icon { width: 52px; height: 52px; display: grid; place-items: center; border-radius: 16px; background: linear-gradient(135deg,#ffe0eb,#ffd1e2); box-shadow: 0 10px 24px rgba(233,30,99,0.14); }
.brand-icon svg { width: 32px; height: 32px; fill: #760f5c; }
.brand-text { display: flex; flex-direction: column; }
.brand-title { font-size: 30px; font-weight: 800; color: #d81b60; }
.brand-subtitle { font-size: 12px; text-transform: uppercase; letter-spacing: 1.8px; color: #9d4667; }
.adopt-btn { text-decoration: none; background: linear-gradient(135deg,#0f766e,#14b8a6); color: white; font-weight: 800; padding: 11px 16px; border-radius: 14px; box-shadow: 0 10px 24px rgba(15,118,110,0.18); white-space: nowrap; transition: transform 0.2s, box-shadow 0.2s; }
.adopt-btn:hover { transform: translateY(-1px); box-shadow: 0 12px 28px rgba(15,118,110,0.24); }
.nav-links { display: flex; align-items: center; gap: 12px; }
.nav-link { text-decoration: none; color: #4a2b38; font-weight: 600; padding: 10px 14px; border-radius: 12px; }
.nav-link:hover { background: #fff1f6; color: #d81b60; }
.btn-register { text-decoration: none; background: linear-gradient(135deg,#e91e63,#d81b60); color: white; font-weight: 700; padding: 12px 18px; border-radius: 14px; }
.account-box { display: flex; align-items: center; gap: 12px; }
.account-dropdown { position: relative; }
.account-trigger { display: flex; align-items: center; gap: 10px; color: #4a2b38; font-weight: 600; padding: 8px 10px; border-radius: 14px; background: transparent; border: none; cursor: pointer; }
.account-trigger:hover { background: #fff1f6; }
.avatar { width: 40px; height: 40px; border-radius: 50%; background: linear-gradient(135deg,#e91e63,#f06292); color: white; display: grid; place-items: center; font-weight: 700; box-shadow: 0 10px 20px rgba(233,30,99,0.18); }
.account-name { font-size: 14px; position: relative; }
.profile-tooltip::after { content: attr(data-tooltip); position: absolute; left: 50%; bottom: -38px; transform: translateX(-50%); background: #4a2b38; color: white; font-size: 12px; font-weight: 600; padding: 7px 10px; border-radius: 10px; white-space: nowrap; opacity: 0; visibility: hidden; pointer-events: none; transition: opacity 0.2s, transform 0.2s; box-shadow: 0 10px 24px rgba(0,0,0,0.16); z-index: 1200; }
.account-trigger:hover .profile-tooltip::after { opacity: 1; visibility: visible; transform: translateX(-50%) translateY(4px); }
.dropdown-arrow { font-size: 12px; color: #9d4667; transition: transform 0.2s; }
.dropdown-arrow.open { transform: rotate(180deg); }
.dropdown-menu { position: absolute; top: calc(100% + 10px); right: 0; min-width: 220px; background: white; border: 1px solid #f4cada; border-radius: 16px; box-shadow: 0 18px 38px rgba(0,0,0,0.12); padding: 8px; display: flex; flex-direction: column; z-index: 1200; }
.dropdown-item { text-decoration: none; background: transparent; border: none; color: #4a2b38; text-align: left; padding: 12px 14px; border-radius: 12px; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 8px; }
.dropdown-item:hover { background: #fff1f6; color: #d81b60; }
.requests-item { position: relative; }
.badge { margin-left: auto; background: #be185d; color: white; border-radius: 999px; padding: 2px 8px; font-size: 0.75rem; font-weight: 800; }
.dropdown-divider { border: none; border-top: 1px solid #f3f4f6; margin: 4px 8px; }
.logout-item { width: 100%; }

@media (max-width: 768px) {
  .brand-title { font-size: 24px; }
  .brand-subtitle { display: none; }
  .nav-content { flex-wrap: wrap; padding: 12px 0; }
  .brand-group { width: 100%; justify-content: space-between; }
  .nav-links { width: 100%; flex-wrap: wrap; }
  .adopt-btn { width: 100%; text-align: center; }
  .account-name { display: none; }
  .profile-tooltip::after { display: none; }
}
</style>