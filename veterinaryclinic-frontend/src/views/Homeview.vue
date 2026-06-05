<script setup lang="ts">
import { useAuthStore } from '../stores/auth'
import { onMounted, ref, computed } from 'vue'
import { getAllAppointments, getDoctorAppointments } from '../api/services/appointmentService'
import { getAllPets } from '../api/services/petService'
import { Bar, Pie, Doughnut } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale, LinearScale, BarElement,
  ArcElement, Tooltip, Legend, Title
} from 'chart.js'

ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Tooltip, Legend, Title)

const authStore = useAuthStore()

// ── ADMIN ────────────────────────────────────────────────
const allAppointments = ref<any[]>([])
const allPets = ref<any[]>([])
const loadingAdmin = ref(false)

const adminStats = computed(() => ({
  total:   allAppointments.value.length,
  today:   allAppointments.value.filter(a => new Date(a.date).toDateString() === new Date().toDateString()).length,
  pending: allAppointments.value.filter(a => String(a.status) === '0').length,
  pets:    allPets.value.length
}))

const monthlyChartData = computed(() => {
  const labels = ['Ian','Feb','Mar','Apr','Mai','Iun','Iul','Aug','Sep','Oct','Nov','Dec']
  const counts = Array(12).fill(0)
  allAppointments.value.forEach(a => { counts[new Date(a.date).getMonth()]++ })
  return {
    labels,
    datasets: [{
      label: 'Programări',
      data: counts,
      backgroundColor: '#be185d',
      borderRadius: 8,
      borderSkipped: false
    }]
  }
})

const typeChartData = computed(() => {
  const typeMap: Record<number, string> = { 0:'Consult', 1:'Control', 2:'Vaccinare', 3:'Urgență', 4:'Follow-up' }
  const counts: Record<string, number> = {}
  allAppointments.value.forEach(a => {
    const l = typeMap[a.type] ?? 'Altele'
    counts[l] = (counts[l] || 0) + 1
  })
  return {
    labels: Object.keys(counts),
    datasets: [{ data: Object.values(counts), backgroundColor: COLORS, borderWidth: 2, borderColor: '#fff' }]
  }
})

const speciesChartData = computed(() => {
  const counts: Record<string, number> = {}
  allPets.value.forEach(p => {
    const s = p.species || 'Necunoscut'
    counts[s] = (counts[s] || 0) + 1
  })
  return {
    labels: Object.keys(counts),
    datasets: [{ data: Object.values(counts), backgroundColor: COLORS, borderWidth: 2, borderColor: '#fff' }]
  }
})

// ── DOCTOR ───────────────────────────────────────────────
const doctorAppointments = ref<any[]>([])
const loadingDoctor = ref(false)

const doctorStats = computed(() => ({
  total:     doctorAppointments.value.length,
  today:     doctorAppointments.value.filter(a => new Date(a.date).toDateString() === new Date().toDateString()).length,
  upcoming:  doctorAppointments.value.filter(a => new Date(a.date) > new Date() && String(a.status) !== '2').length,
  completed: doctorAppointments.value.filter(a => String(a.status) === '3').length,
}))

const weeklyChartData = computed(() => {
  const days = ['Lun','Mar','Mie','Joi','Vin','Sâm','Dum']
  const counts = Array(7).fill(0)
  const now = new Date()
  const weekStart = new Date(now)
  weekStart.setDate(now.getDate() - ((now.getDay() + 6) % 7))
  weekStart.setHours(0, 0, 0, 0)
  doctorAppointments.value.forEach(a => {
    const diff = Math.floor((new Date(a.date).getTime() - weekStart.getTime()) / 86400000)
    if (diff >= 0 && diff < 7) counts[diff]++
  })
  return {
    labels: days,
    datasets: [{
      label: 'Programări',
      data: counts,
      backgroundColor: '#7c3aed',
      borderRadius: 8,
      borderSkipped: false
    }]
  }
})

const statusChartData = computed(() => {
  const map: Record<string, string> = { '0':'În așteptare', '1':'Confirmată', '2':'Anulată', '3':'Finalizată' }
  const counts: Record<string, number> = {}
  doctorAppointments.value.forEach(a => {
    const l = map[String(a.status)] ?? 'Altele'
    counts[l] = (counts[l] || 0) + 1
  })
  return {
    labels: Object.keys(counts),
    datasets: [{ data: Object.values(counts), backgroundColor: COLORS, borderWidth: 2, borderColor: '#fff' }]
  }
})

const doctorTypeChartData = computed(() => {
  const typeMap: Record<number, string> = { 0:'Consult', 1:'Control', 2:'Vaccinare', 3:'Urgență', 4:'Follow-up' }
  const counts: Record<string, number> = {}
  doctorAppointments.value.forEach(a => {
    const l = typeMap[a.type] ?? 'Altele'
    counts[l] = (counts[l] || 0) + 1
  })
  return {
    labels: Object.keys(counts),
    datasets: [{ data: Object.values(counts), backgroundColor: COLORS, borderWidth: 2, borderColor: '#fff' }]
  }
})

// ── SHARED ───────────────────────────────────────────────
const COLORS = ['#be185d','#ec4899','#f9a8d4','#7c3aed','#a78bfa','#10b981','#f59e0b','#3b82f6']

const barOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } },
  scales: {
    y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: '#f3f4f6' } },
    x: { grid: { display: false } }
  }
}

const pieOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { position: 'bottom' as const, labels: { padding: 14, font: { size: 12 } } } }
}

async function loadAdminData() {
  try {
    loadingAdmin.value = true
    const [appts, pets] = await Promise.all([getAllAppointments(), getAllPets()])
    allAppointments.value = appts
    allPets.value = pets
  } catch (e) { console.error(e) }
  finally { loadingAdmin.value = false }
}

async function loadDoctorData() {
  try {
    loadingDoctor.value = true
    doctorAppointments.value = await getDoctorAppointments()
  } catch (e) { console.error(e) }
  finally { loadingDoctor.value = false }
}

onMounted(() => {
  if (authStore.isAdmin) loadAdminData()
  if (authStore.isDoctor) loadDoctorData()
})
</script>

<template>
  <div class="role-dashboard">

    <!-- ═══════ ADMIN ═══════ -->
    <section v-if="authStore.isAdmin" class="dashboard-section">
      <header class="dashboard-header">
        <h1>Panou Admin</h1>
        <p>Bun venit, {{ authStore.user?.name }}! Iată o privire de ansamblu asupra clinicii.</p>
      </header>

      <div v-if="loadingAdmin" class="loading-box">Se încarcă statisticile...</div>

      <template v-else>
        <div class="stats-row">
          <div class="stat-card" style="border-left-color:#be185d">
            <div class="stat-icon">📅</div>
            <div><strong>{{ adminStats.total }}</strong><span>Total programări</span></div>
          </div>
          <div class="stat-card" style="border-left-color:#f59e0b">
            <div class="stat-icon">🕐</div>
            <div><strong>{{ adminStats.today }}</strong><span>Azi</span></div>
          </div>
          <div class="stat-card" style="border-left-color:#7c3aed">
            <div class="stat-icon">⏳</div>
            <div><strong>{{ adminStats.pending }}</strong><span>În așteptare</span></div>
          </div>
          <div class="stat-card" style="border-left-color:#10b981">
            <div class="stat-icon">🐾</div>
            <div><strong>{{ adminStats.pets }}</strong><span>Animale</span></div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-card wide">
            <h3 class="chart-title">📊 Programări pe luni</h3>
            <div class="chart-wrap tall">
              <Bar :data="monthlyChartData" :options="barOptions" />
            </div>
          </div>
          <div class="chart-card">
            <h3 class="chart-title">🩺 Tipuri de consultații</h3>
            <div class="chart-wrap">
              <Pie :data="typeChartData" :options="pieOptions" />
            </div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-card">
            <h3 class="chart-title">🐕 Animale după specie</h3>
            <div class="chart-wrap">
              <Doughnut :data="speciesChartData" :options="pieOptions" />
            </div>
          </div>
          <div class="chart-card quick-links">
            <h3 class="chart-title">🔗 Acces rapid</h3>
            <div class="links-grid">
              <router-link to="/admin/pets" class="quick-link">🐾 Pacienți</router-link>
              <router-link to="/admin/accounts" class="quick-link">👤 Conturi</router-link>
              <router-link to="/admin/appointments" class="quick-link">📅 Programări</router-link>
              <router-link to="/admin/adoptions" class="quick-link">💛 Adopții</router-link>
              <router-link to="/admin/adoption-requests" class="quick-link">📩 Cereri</router-link>
              <router-link to="/admin/profile" class="quick-link">⚙️ Profil</router-link>
            </div>
          </div>
        </div>
      </template>
    </section>

    <!-- ═══════ DOCTOR ═══════ -->
    <section v-else-if="authStore.isDoctor" class="dashboard-section">
      <header class="dashboard-header">
        <h1>Panou Doctor</h1>
        <p>Bun venit, Dr. {{ authStore.user?.name }}! Iată activitatea ta curentă.</p>
      </header>

      <div v-if="loadingDoctor" class="loading-box">Se încarcă statisticile...</div>

      <template v-else>
        <div class="stats-row">
          <div class="stat-card" style="border-left-color:#be185d">
            <div class="stat-icon">📋</div>
            <div><strong>{{ doctorStats.total }}</strong><span>Total programări</span></div>
          </div>
          <div class="stat-card" style="border-left-color:#f59e0b">
            <div class="stat-icon">📅</div>
            <div><strong>{{ doctorStats.today }}</strong><span>Azi</span></div>
          </div>
          <div class="stat-card" style="border-left-color:#3b82f6">
            <div class="stat-icon">⏳</div>
            <div><strong>{{ doctorStats.upcoming }}</strong><span>Viitoare</span></div>
          </div>
          <div class="stat-card" style="border-left-color:#10b981">
            <div class="stat-icon">✅</div>
            <div><strong>{{ doctorStats.completed }}</strong><span>Finalizate</span></div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-card wide">
            <h3 class="chart-title">📅 Programări săptămâna curentă</h3>
            <div class="chart-wrap tall">
              <Bar :data="weeklyChartData" :options="barOptions" />
            </div>
          </div>
          <div class="chart-card">
            <h3 class="chart-title">🔵 Status programări</h3>
            <div class="chart-wrap">
              <Pie :data="statusChartData" :options="pieOptions" />
            </div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-card">
            <h3 class="chart-title">🩺 Tipuri de consultații</h3>
            <div class="chart-wrap">
              <Doughnut :data="doctorTypeChartData" :options="pieOptions" />
            </div>
          </div>
          <div class="chart-card quick-links">
            <h3 class="chart-title">🔗 Acces rapid</h3>
            <div class="links-grid">
              <router-link to="/doctor/appointments" class="quick-link">📋 Programări</router-link>
              <router-link to="/doctor/patients" class="quick-link">🐶 Pacienți</router-link>
              <router-link to="/doctor/profile" class="quick-link">👨‍⚕️ Profil</router-link>
            </div>
          </div>
        </div>
      </template>
    </section>

    <!-- ═══════ CLIENT ═══════ -->
    <section v-else class="dashboard-section">
      <header class="dashboard-header">
        <h1>Bun venit, {{ authStore.user?.name }}!</h1>
        <p>De aici poți gestiona animalele tale și programările făcute.</p>
      </header>
      <div class="card-grid">
        <router-link to="/my-pets" class="dashboard-card">
          <span class="card-icon">🐕</span>
          <h3>Animalele mele</h3>
          <p>Vezi animalele înregistrate în contul tău.</p>
        </router-link>
        <router-link to="/my-appointments" class="dashboard-card">
          <span class="card-icon">🗓️</span>
          <h3>Programările mele</h3>
          <p>Verifică programările active și istoricul vizitelor.</p>
        </router-link>
        <router-link to="/appointments/create" class="dashboard-card">
          <span class="card-icon">➕</span>
          <h3>Programează o vizită</h3>
          <p>Fă rapid o nouă programare la un doctor.</p>
        </router-link>
        <router-link to="/client/profile" class="dashboard-card">
          <span class="card-icon">👤</span>
          <h3>Profilul meu</h3>
          <p>Actualizează datele personale ale contului.</p>
        </router-link>
        <router-link to="/client/adoption-requests" class="dashboard-card">
          <span class="card-icon">📨</span>
          <h3>Cererile mele de adopție</h3>
          <p>Vezi cererile trimise și statusul lor actual.</p>
        </router-link>
      </div>
    </section>

  </div>
</template>

<style scoped>
.role-dashboard { padding: 2.5rem 2rem; max-width: 1200px; margin: 0 auto; }
.dashboard-section { display: flex; flex-direction: column; gap: 1.5rem; }
.dashboard-header { text-align: center; margin-bottom: 0.5rem; }
.dashboard-header h1 { font-size: 2.2rem; font-weight: 800; color: #1f2937; margin-bottom: 0.6rem; }
.dashboard-header p { font-size: 1rem; color: #6b7280; max-width: 600px; margin: 0 auto; }

.loading-box { text-align: center; padding: 3rem; color: #6b7280; background: white; border-radius: 18px; font-size: 1rem; }

.stats-row { display: grid; grid-template-columns: repeat(4, 1fr); gap: 14px; }
.stat-card { background: white; border-radius: 18px; padding: 18px 20px; display: flex; align-items: center; gap: 14px; box-shadow: 0 4px 14px rgba(0,0,0,0.06); border-left: 4px solid #e5e7eb; }
.stat-icon { font-size: 1.7rem; }
.stat-card strong { display: block; font-size: 1.9rem; font-weight: 900; color: #1f2937; line-height: 1; }
.stat-card span { font-size: 0.78rem; color: #6b7280; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; }

.charts-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.chart-card { background: white; border-radius: 22px; padding: 22px 20px; box-shadow: 0 4px 14px rgba(0,0,0,0.06); border: 1px solid #f3f4f6; }
.chart-card.wide { grid-column: 1 / -1; }
.chart-title { font-size: 0.95rem; font-weight: 700; color: #374151; margin: 0 0 16px; }

.chart-wrap { height: 240px; position: relative; }
.chart-wrap.tall { height: 260px; }

.quick-links { display: flex; flex-direction: column; }
.links-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-top: 4px; }
.quick-link { text-decoration: none; background: #f9fafb; border: 1px solid #f3f4f6; border-radius: 14px; padding: 14px 16px; font-size: 0.88rem; font-weight: 600; color: #374151; transition: all 0.2s; text-align: center; }
.quick-link:hover { background: #fdf2f8; border-color: #fbcfe8; color: #be185d; transform: translateY(-2px); }

.card-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 1.25rem; }
.dashboard-card { background: white; border-radius: 22px; padding: 1.8rem; text-decoration: none; color: #1f2937; box-shadow: 0 8px 24px rgba(0,0,0,0.07); transition: all 0.25s; display: block; }
.dashboard-card:hover { transform: translateY(-6px); box-shadow: 0 16px 40px rgba(233,30,99,0.14); }
.card-icon { font-size: 2.1rem; display: block; margin-bottom: 1rem; }
.dashboard-card h3 { font-size: 1.1rem; font-weight: 700; margin-bottom: 0.5rem; }
.dashboard-card p { font-size: 0.9rem; color: #6b7280; line-height: 1.6; }

@media (max-width: 1024px) {
  .charts-grid { grid-template-columns: 1fr; }
  .chart-card.wide { grid-column: 1; }
}
@media (max-width: 768px) {
  .role-dashboard { padding: 1.5rem 1rem; }
  .stats-row { grid-template-columns: repeat(2, 1fr); }
  .dashboard-header h1 { font-size: 1.8rem; }
}
</style>