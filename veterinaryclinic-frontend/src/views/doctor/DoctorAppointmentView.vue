<template>
  <section class="page doctor-appointments">
    <header class="page-header">
      <div>
        <span class="eyebrow">Doctor Dashboard</span>
        <h1>Programările mele</h1>
        <p>Vezi consultațiile de azi, viitoare și istoricul lor.</p>
      </div>

      <div class="header-actions">
        <input v-model="search" type="text" placeholder="Caută după pacient..." />
        <select v-model="statusFilter">
  <option value="">Toate statusurile</option>
  <option :value="0">În așteptare</option>
  <option :value="1">Confirmate</option>
  <option :value="2">Anulate</option>
  <option :value="3">Finalizate</option>
</select>

<select v-model="typeFilter">
  <option value="">Toate tipurile</option>
  <option :value="0">Control</option>
  <option :value="1">Vaccinare</option>
  <option :value="2">Intervenție</option>
  <option :value="3">Urgență</option>
  <option :value="4">Follow-up</option>
</select>
      </div>
    </header>

    <div class="cards">
      <article class="stat-card">
        <span>Azi</span>
        <strong>{{ todayCount }}</strong>
        <p>Programări în ziua curentă</p>
      </article>

      <article class="stat-card">
        <span>Viitoare</span>
        <strong>{{ upcomingCount }}</strong>
        <p>Consultații programate</p>
      </article>

      <article class="stat-card">
        <span>Finalizate</span>
        <strong>{{ completedCount }}</strong>
        <p>Vizite încheiate cu succes</p>
      </article>
    </div>

    <div class="table-card">
      <div class="table-head">
        <h2>Lista programărilor</h2>
        <span>{{ filteredAppointments.length }} rezultate</span>
      </div>

      <div v-if="loading" class="state-box">Se încarcă programările...</div>
      <div v-else-if="error" class="state-box error">{{ error }}</div>

      <div v-else class="table-scroll">
        <table>
          <thead>
            <tr>
              <th>Pacient</th>
              <th>Proprietar</th>
              <th>Data</th>
              <th>Tip</th>
              <th>Status</th>
              <th>Note</th>
              <th>Acțiuni</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="appointment in filteredAppointments" :key="appointment.id">
              <td>
                <div class="cell-title">{{ appointment.petName }}</div>
              </td>
              <td>{{ appointment.clientName }}</td>
              <td>{{ formatDate(appointment.date) }}</td>
              <td>
                <span class="chip">{{ getTypeLabel(appointment.type) }}</span>
              </td>
              <td>
                <span class="status-badge" :class="getStatusClass(appointment.status)">
                  {{ getStatusLabel(appointment.status) }}
                </span>
              </td>
              <td class="notes-cell">{{ appointment.notes }}</td>
              <td>
                <div class="row-actions">
                  <button class="action-btn secondary" @click="goToDetails(appointment.id)">
                    Detalii
                   </button>
                  <button class="action-btn" @click="editAppointment(appointment.id)">
                    Editează
                  </button>
               </div>
                </td>
            </tr>

            <tr v-if="filteredAppointments.length === 0">
              <td colspan="7" class="empty-row">
                Nu există programări pentru filtrele selectate.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </section>
</template>

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

onMounted(() => {
  loadAppointments()
})

const filteredAppointments = computed(() => {
  const term = search.value.toLowerCase().trim()

  return appointments.value.filter((appointment) => {
    const matchesSearch =
      !term ||
      appointment.petName.toLowerCase().includes(term) ||
      appointment.clientName.toLowerCase().includes(term) ||
      appointment.notes.toLowerCase().includes(term)

    const matchesStatus = !statusFilter.value || String(appointment.status) === statusFilter.value
    const matchesType = !typeFilter.value || String(appointment.type) === typeFilter.value

    return matchesSearch && matchesStatus && matchesType
  })
})

const todayCount = computed(() => {
  const today = new Date().toDateString()
  return appointments.value.filter((a) => new Date(a.date).toDateString() === today).length
})

const upcomingCount = computed(() => {
  const now = new Date()
  return appointments.value.filter(
    (a) => new Date(a.date) > now && String(a.status) !== 'Completed' && String(a.status) !== '3'
  ).length
})

const completedCount = computed(() => {
  return appointments.value.filter((a) => String(a.status) === 'Completed' || String(a.status) === '3').length
})

function formatDate(value: string) {
  return new Date(value).toLocaleString('ro-RO', {
    dateStyle: 'medium',
    timeStyle: 'short'
  })
}
function goToDetails(id: number) {
  router.push(`/doctor/appointments/${id}`)
}

function editAppointment(id: number) {
  router.push(`/doctor/appointments/edit/${id}`)
}
function getStatusLabel(status: string | number) {
  const map: Record<string, string> = {
    Pending: 'În așteptare',
    Confirmed: 'Confirmată',
    Cancelled: 'Anulată',
    Completed: 'Finalizată',
    '0': 'În așteptare',
    '1': 'Confirmată',
    '2': 'Anulată',
    '3': 'Finalizată'
  }
  return map[String(status)] ?? String(status)
}

function getTypeLabel(type: string | number) {
  const map: Record<string, string> = {
    Checkup: 'Control',
    Vaccination: 'Vaccinare',
    Surgery: 'Intervenție',
    Emergency: 'Urgență',
    FollowUp: 'Follow-up',
    '0': 'Control',
    '1': 'Vaccinare',
    '2': 'Intervenție',
    '3': 'Urgență',
    '4': 'Follow-up'
  }
  return map[String(type)] ?? String(type)
}

function getStatusClass(status: string | number) {
  const map: Record<string, string> = {
    Pending: 'pending',
    Confirmed: 'confirmed',
    Cancelled: 'cancelled',
    Completed: 'completed',
    '0': 'pending',
    '1': 'confirmed',
    '2': 'cancelled',
    '3': 'completed'
  }
  return map[String(status)] ?? ''
}
</script>

<style scoped>
.page {
  padding: 32px 24px;
  max-width: 1280px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 24px;
  flex-wrap: wrap;
  margin-bottom: 24px;
}

.eyebrow {
  display: inline-block;
  font-size: 12px;
  letter-spacing: 0.14em;
  text-transform: uppercase;
  color: #ec4899;
  font-weight: 700;
  margin-bottom: 10px;
}

.page-header h1 {
  font-size: clamp(2rem, 1.4rem + 1.5vw, 2.8rem);
  line-height: 1.1;
  margin-bottom: 8px;
  color: #1f2937;
}

.page-header p {
  color: #6b7280;
  max-width: 60ch;
}

.header-actions {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  align-items: center;
}

.header-actions input,
.header-actions select {
  min-width: 220px;
  border: 1px solid #e5e7eb;
  border-radius: 14px;
  padding: 12px 14px;
  background: #fff;
  color: #111827;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
}

.header-actions input:focus,
.header-actions select:focus {
  outline: none;
  border-color: #ec4899;
  box-shadow: 0 0 0 4px rgba(236, 72, 153, 0.12);
}

.cards {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.stat-card {
  background: linear-gradient(180deg, #ffffff 0%, #fff7fb 100%);
  border: 1px solid #f3d7e6;
  border-radius: 20px;
  padding: 20px;
  box-shadow: 0 10px 30px rgba(236, 72, 153, 0.08);
}

.stat-card span {
  display: block;
  color: #9f1239;
  font-size: 13px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  margin-bottom: 10px;
}

.stat-card strong {
  display: block;
  font-size: 2rem;
  line-height: 1;
  color: #111827;
  margin-bottom: 8px;
}

.stat-card p {
  color: #6b7280;
  font-size: 14px;
}

.table-card {
  background: #fff;
  border-radius: 24px;
  padding: 20px;
  box-shadow: 0 16px 40px rgba(15, 23, 42, 0.08);
  border: 1px solid #f3f4f6;
}

.table-head {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
  margin-bottom: 18px;
}

.table-head h2 {
  font-size: 1.1rem;
  color: #111827;
}

.table-head span {
  font-size: 14px;
  color: #6b7280;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  padding: 8px 12px;
  border-radius: 999px;
}

.table-scroll {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  text-align: left;
  padding: 16px 14px;
  border-bottom: 1px solid #eef2f7;
  vertical-align: top;
}

th {
  color: #374151;
  font-size: 13px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

td {
  color: #4b5563;
}

.cell-title {
  font-weight: 700;
  color: #111827;
}

.notes-cell {
  max-width: 280px;
  white-space: normal;
}

.chip {
  display: inline-flex;
  align-items: center;
  padding: 6px 10px;
  border-radius: 999px;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  font-size: 13px;
  color: #374151;
}

.status-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 999px;
  padding: 7px 11px;
  font-size: 12px;
  font-weight: 700;
  white-space: nowrap;
}

.status-badge.pending {
  background: #fef3c7;
  color: #92400e;
}

.status-badge.confirmed {
  background: #dbeafe;
  color: #1d4ed8;
}

.status-badge.cancelled {
  background: #fee2e2;
  color: #b91c1c;
}

.status-badge.completed {
  background: #dcfce7;
  color: #166534;
}

.action-btn {
  border: none;
  border-radius: 12px;
  padding: 10px 14px;
  background: #ec4899;
  color: white;
  font-weight: 700;
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease, background 0.2s ease;
}

.action-btn:hover {
  background: #db2777;
  transform: translateY(-1px);
  box-shadow: 0 10px 20px rgba(236, 72, 153, 0.2);
}

.empty-row {
  text-align: center;
  padding: 28px 16px;
  color: #6b7280;
}

.state-box {
  padding: 24px;
  border-radius: 16px;
  background: #f9fafb;
  color: #374151;
  text-align: center;
  border: 1px dashed #d1d5db;
}

.state-box.error {
  background: #fef2f2;
  color: #b91c1c;
  border-color: #fecaca;
}

@media (max-width: 1024px) {
  .cards {
    grid-template-columns: 1fr;
  }

  .header-actions {
    width: 100%;
  }

  .header-actions input,
  .header-actions select {
    min-width: 0;
    width: 100%;
  }
}

@media (max-width: 768px) {
  .page {
    padding: 20px 14px;
  }

  .table-card {
    padding: 16px;
    border-radius: 18px;
  }

  th,
  td {
    padding: 12px 10px;
  }

  .table-head {
    flex-direction: column;
    align-items: flex-start;
  }

.row-actions {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.action-btn.secondary {
  background: #f3f4f6;
  color: #1f2937;
}

.action-btn.secondary:hover {
  background: #e5e7eb;
  box-shadow: none;
  transform: translateY(-1px);
}
}
</style>