<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { getDoctorAppointments, type AppointmentResponseDto } from '../../api/services/appointmentService'
import { useRouter } from 'vue-router'

const router = useRouter()
const search = ref('')
const statusFilter = ref('')
const typeFilter = ref('')
const loading = ref(false)
const error = ref('')
const appointments = ref<AppointmentResponseDto[]>([])

const loadAppointments = async () => {
  try {
    loading.value = true
    error.value = ''
    appointments.value = await getDoctorAppointments()
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-au putut încărca programările.'
  } finally {
    loading.value = false
  }
}

onMounted(() => { loadAppointments() })

const filteredAppointments = computed(() => {
  const term = search.value.toLowerCase().trim()
  return appointments.value
    .filter((a) => {
      const matchesSearch = !term ||
        a.petName.toLowerCase().includes(term) ||
        a.clientName.toLowerCase().includes(term) ||
        a.notes?.toLowerCase().includes(term)
      const matchesStatus = statusFilter.value === '' || String(a.status) === statusFilter.value
      const matchesType = typeFilter.value === '' || String(a.type) === typeFilter.value
      return matchesSearch && matchesStatus && matchesType
    })
    .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
})

const todayCount = computed(() => {
  const today = new Date().toDateString()
  return appointments.value.filter(a => new Date(a.date).toDateString() === today).length
})

const upcomingCount = computed(() => {
  const now = new Date()
  return appointments.value.filter(a => new Date(a.date) > now && String(a.status) !== '3').length
})

const completedCount = computed(() => {
  return appointments.value.filter(a => String(a.status) === '3').length
})

function formatDate(value: string) {
  return new Date(value).toLocaleString('ro-RO', { dateStyle: 'medium', timeStyle: 'short' })
}

function formatDateShort(value: string) {
  return new Date(value).toLocaleDateString('ro-RO', { day: '2-digit', month: 'short' })
}

function formatTime(value: string) {
  return new Date(value).toLocaleTimeString('ro-RO', { hour: '2-digit', minute: '2-digit' })
}

function isToday(value: string) {
  return new Date(value).toDateString() === new Date().toDateString()
}

function isPast(value: string) {
  return new Date(value) < new Date()
}

function goToDetails(id: number) {
  router.push(`/doctor/appointments/${id}`)
}

function getStatusLabel(status: string | number) {
  const map: Record<string, string> = { '0': 'În așteptare', '1': 'Confirmată', '2': 'Anulată', '3': 'Finalizată' }
  return map[String(status)] ?? String(status)
}

function getTypeLabel(type: string | number) {
  const map: Record<string, string> = { '0': 'Control', '1': 'Vaccinare', '2': 'Intervenție', '3': 'Urgență', '4': 'Follow-up' }
  return map[String(type)] ?? String(type)
}

function getTypeIcon(type: string | number) {
  const map: Record<string, string> = { '0': '🩺', '1': '💉', '2': '🔬', '3': '🚨', '4': '📋' }
  return map[String(type)] ?? '📅'
}

function getStatusClass(status: string | number) {
  const map: Record<string, string> = { '0': 'pending', '1': 'confirmed', '2': 'cancelled', '3': 'completed' }
  return map[String(status)] ?? ''
}
</script>

<template>
  <section class="page">
    <!-- Header -->
    <header class="page-header">
      <div>
        <span class="eyebrow">Doctor Dashboard</span>
        <h1>Programările mele</h1>
        <p>Vezi consultațiile de azi, viitoare și istoricul lor.</p>
      </div>
    </header>

    <!-- Stats -->
    <div class="stats-row">
      <div class="stat-card today">
        <div class="stat-icon">📅</div>
        <div class="stat-content">
          <strong>{{ todayCount }}</strong>
          <span>Azi</span>
        </div>
      </div>
      <div class="stat-card upcoming">
        <div class="stat-icon">⏳</div>
        <div class="stat-content">
          <strong>{{ upcomingCount }}</strong>
          <span>Viitoare</span>
        </div>
      </div>
      <div class="stat-card completed">
        <div class="stat-icon">✅</div>
        <div class="stat-content">
          <strong>{{ completedCount }}</strong>
          <span>Finalizate</span>
        </div>
      </div>
      <div class="stat-card total">
        <div class="stat-icon">📊</div>
        <div class="stat-content">
          <strong>{{ appointments.length }}</strong>
          <span>Total</span>
        </div>
      </div>
    </div>

    <!-- Filtre -->
    <div class="filters-bar">
      <div class="search-wrap">
        <span class="search-icon">🔍</span>
        <input v-model="search" type="text" placeholder="Caută după pacient sau proprietar..." />
      </div>
      <select v-model="statusFilter">
        <option value="">Toate statusurile</option>
        <option value="0">În așteptare</option>
        <option value="1">Confirmate</option>
        <option value="2">Anulate</option>
        <option value="3">Finalizate</option>
      </select>
      <select v-model="typeFilter">
        <option value="">Toate tipurile</option>
        <option value="0">Control</option>
        <option value="1">Vaccinare</option>
        <option value="2">Intervenție</option>
        <option value="3">Urgență</option>
        <option value="4">Follow-up</option>
      </select>
    </div>

    <!-- Stări -->
    <div v-if="loading" class="state-box">Se încarcă programările...</div>
    <div v-else-if="error" class="state-box error">{{ error }}</div>

    <!-- Lista carduri -->
    <div v-else-if="filteredAppointments.length === 0" class="state-box">
      Nu există programări pentru filtrele selectate.
    </div>

    <div v-else class="appointments-list">
      <div
        v-for="appointment in filteredAppointments"
        :key="appointment.id"
        class="appointment-card"
        :class="{ 'is-today': isToday(appointment.date), 'is-past': isPast(appointment.date) && String(appointment.status) !== '3' }"
        @click="goToDetails(appointment.id)"
      >
        <!-- Data -->
        <div class="card-date">
          <div class="date-day">{{ formatDateShort(appointment.date) }}</div>
          <div class="date-time">{{ formatTime(appointment.date) }}</div>
          <div v-if="isToday(appointment.date)" class="today-badge">Azi</div>
        </div>

        <!-- Info principal -->
        <div class="card-main">
          <div class="card-top">
            <span class="type-icon">{{ getTypeIcon(appointment.type) }}</span>
            <span class="type-label">{{ getTypeLabel(appointment.type) }}</span>
            <span class="status-badge" :class="getStatusClass(appointment.status)">
              {{ getStatusLabel(appointment.status) }}
            </span>
          </div>
          <div class="pet-name">{{ appointment.petName }}</div>
          <div class="client-name">Proprietar: {{ appointment.clientName }}</div>
          <div v-if="appointment.notes" class="notes">{{ appointment.notes }}</div>
        </div>

        <!-- Arrow -->
        <div class="card-arrow">→</div>
      </div>
    </div>

    <!-- Counter -->
    <div v-if="!loading && !error && filteredAppointments.length > 0" class="results-count">
      {{ filteredAppointments.length }} programări găsite
    </div>
  </section>
</template>

<style scoped>
.page { padding: 32px 24px; max-width: 1100px; margin: 0 auto; }

.eyebrow { display: inline-block; font-size: 12px; letter-spacing: 0.14em; text-transform: uppercase; color: #ec4899; font-weight: 700; margin-bottom: 10px; }
.page-header { margin-bottom: 28px; }
.page-header h1 { font-size: clamp(1.8rem, 1.4rem + 1.5vw, 2.6rem); color: #1f2937; margin-bottom: 6px; }
.page-header p { color: #6b7280; }

/* Stats */
.stats-row { display: grid; grid-template-columns: repeat(4, 1fr); gap: 14px; margin-bottom: 24px; }
.stat-card { background: white; border-radius: 18px; padding: 18px; display: flex; align-items: center; gap: 14px; box-shadow: 0 4px 16px rgba(0,0,0,0.06); border: 1px solid #f3f4f6; }
.stat-icon { font-size: 1.8rem; }
.stat-content strong { display: block; font-size: 1.8rem; font-weight: 800; color: #1f2937; line-height: 1; }
.stat-content span { font-size: 0.82rem; color: #6b7280; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; }
.stat-card.today { border-left: 4px solid #f59e0b; }
.stat-card.upcoming { border-left: 4px solid #3b82f6; }
.stat-card.completed { border-left: 4px solid #10b981; }
.stat-card.total { border-left: 4px solid #ec4899; }

/* Filtre */
.filters-bar { display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 20px; align-items: center; }
.search-wrap { position: relative; flex: 1; min-width: 240px; }
.search-icon { position: absolute; left: 14px; top: 50%; transform: translateY(-50%); font-size: 0.95rem; }
.search-wrap input { width: 100%; border: 1px solid #e5e7eb; border-radius: 14px; padding: 11px 14px 11px 38px; background: white; color: #111827; box-sizing: border-box; }
.search-wrap input:focus { outline: none; border-color: #ec4899; box-shadow: 0 0 0 3px rgba(236,72,153,0.12); }
.filters-bar select { border: 1px solid #e5e7eb; border-radius: 14px; padding: 11px 14px; background: white; color: #374151; cursor: pointer; }
.filters-bar select:focus { outline: none; border-color: #ec4899; }

/* Lista */
.appointments-list { display: grid; gap: 10px; }

.appointment-card {
  background: white; border-radius: 18px; border: 1px solid #f3f4f6;
  padding: 18px 20px; display: flex; align-items: center; gap: 20px;
  cursor: pointer; transition: all 0.2s ease;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
.appointment-card:hover { transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.1); border-color: #ec4899; }
.appointment-card.is-today { border-left: 4px solid #f59e0b; background: linear-gradient(135deg, #fffbeb, white); }
.appointment-card.is-past { opacity: 0.7; }

.card-date { text-align: center; min-width: 60px; }
.date-day { font-size: 0.9rem; font-weight: 700; color: #374151; }
.date-time { font-size: 1.1rem; font-weight: 800; color: #1f2937; margin-top: 2px; }
.today-badge { display: inline-block; margin-top: 4px; font-size: 0.7rem; font-weight: 700; background: #fef3c7; color: #92400e; padding: 2px 8px; border-radius: 999px; text-transform: uppercase; }

.card-main { flex: 1; }
.card-top { display: flex; align-items: center; gap: 8px; margin-bottom: 6px; flex-wrap: wrap; }
.type-icon { font-size: 1.1rem; }
.type-label { font-size: 0.82rem; font-weight: 600; color: #6b7280; background: #f9fafb; padding: 3px 10px; border-radius: 999px; border: 1px solid #e5e7eb; }
.pet-name { font-size: 1.05rem; font-weight: 700; color: #1f2937; }
.client-name { font-size: 0.88rem; color: #6b7280; margin-top: 2px; }
.notes { font-size: 0.85rem; color: #9ca3af; margin-top: 4px; font-style: italic; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 400px; }

.card-arrow { font-size: 1.2rem; color: #d1d5db; transition: color 0.2s, transform 0.2s; }
.appointment-card:hover .card-arrow { color: #ec4899; transform: translateX(4px); }

.status-badge { display: inline-flex; align-items: center; border-radius: 999px; padding: 4px 10px; font-size: 0.75rem; font-weight: 700; white-space: nowrap; }
.status-badge.pending { background: #fef3c7; color: #92400e; }
.status-badge.confirmed { background: #dbeafe; color: #1d4ed8; }
.status-badge.cancelled { background: #fee2e2; color: #b91c1c; }
.status-badge.completed { background: #dcfce7; color: #166534; }

.state-box { padding: 32px; border-radius: 18px; background: #f9fafb; color: #374151; text-align: center; border: 1px dashed #d1d5db; }
.state-box.error { background: #fef2f2; color: #b91c1c; border-color: #fecaca; }

.results-count { text-align: center; margin-top: 16px; font-size: 0.85rem; color: #9ca3af; }

@media (max-width: 768px) {
  .page { padding: 20px 14px; }
  .stats-row { grid-template-columns: repeat(2, 1fr); }
  .appointment-card { flex-wrap: wrap; }
  .card-arrow { display: none; }
  .notes { max-width: 100%; }
}
</style>