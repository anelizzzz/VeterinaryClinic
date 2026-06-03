<template>
  <section class="admin-page">
    <div class="page-header">
      <h1>Programări</h1>
      <p>Administrează programările existente din clinică.</p>
    </div>

    <p v-if="loading">Se încarcă programările...</p>
    <p v-else-if="error" class="error-message">{{ error }}</p>

    <div v-else class="table-card">
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Data</th>
            <th>Status</th>
            <th>Tip</th>
            <th>Client</th>
            <th>Doctor</th>
            <th>Animal</th>
            <th>Notițe</th>
            <th>Acțiuni</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="appointment in appointments" :key="appointment.id">
            <td>{{ appointment.id }}</td>
            <td>{{ formatDate(appointment.date) }}</td>
            <td>{{ getStatusLabel(appointment.status) }}</td>
            <td>{{ getTypeLabel(appointment.type) }}</td>
            <td>{{ appointment.clientName }}</td>
            <td>{{ appointment.doctorName }}</td>
            <td>{{ appointment.petName }}</td>
            <td>{{ appointment.notes }}</td>
            <td class="actions">
              <button class="edit-btn" @click="startEdit(appointment)">Modifică</button>
              <button class="delete-btn" @click="handleDelete(appointment.id)">Șterge</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="editingAppointment" class="edit-card">
      <h2>Modifică programarea #{{ editingAppointment.id }}</h2>

      <div class="form-grid">
        <div class="form-group">
          <label>Data</label>
          <input v-model="editForm.date" type="datetime-local" />
        </div>

        <div class="form-group">
          <label>Status</label>
          <select v-model.number="editForm.status">
            <option :value="0">Pending</option>
            <option :value="1">Confirmed</option>
            <option :value="2">Cancelled</option>
            <option :value="3">Completed</option>
          </select>
        </div>

        <div class="form-group">
          <label>Tip</label>
          <input v-model.number="editForm.type" type="number" />
        </div>

        <div class="form-group">
          <label>Doctor Id</label>
          <input v-model.number="editForm.doctorId" type="number" />
        </div>

        <div class="form-group full-width">
          <label>Notițe</label>
          <textarea v-model="editForm.notes" rows="4"></textarea>
        </div>
      </div>

      <div class="form-actions">
        <button class="save-btn" @click="handleUpdate">Salvează</button>
        <button class="cancel-btn" @click="cancelEdit">Anulează</button>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  getAllAppointments,
  updateAppointment,
  deleteAppointment,
  type AppointmentResponseDto,
  type AppointmentUpdateDto
} from '../../api/services/appointmentService'

const appointments = ref<AppointmentResponseDto[]>([])
const loading = ref(false)
const error = ref('')

const editingAppointment = ref<AppointmentResponseDto | null>(null)

const editForm = ref<AppointmentUpdateDto>({
  date: '',
  status: 0,
  type: 0,
  notes: '',
  doctorId: 0
})

const loadAppointments = async (): Promise<void> => {
  try {
    loading.value = true
    error.value = ''
    appointments.value = await getAllAppointments()
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-au putut încărca programările.'
  } finally {
    loading.value = false
  }
}

const formatDate = (date: string): string => {
  return new Date(date).toLocaleString('ro-RO')
}

const toDateTimeLocal = (date: string): string => {
  const d = new Date(date)
  const pad = (n: number) => String(n).padStart(2, '0')
  const year = d.getFullYear()
  const month = pad(d.getMonth() + 1)
  const day = pad(d.getDate())
  const hours = pad(d.getHours())
  const minutes = pad(d.getMinutes())
  return `${year}-${month}-${day}T${hours}:${minutes}`
}

const getStatusLabel = (status: number): string => {
  switch (status) {
    case 0: return 'Pending'
    case 1: return 'Confirmed'
    case 2: return 'Cancelled'
    case 3: return 'Completed'
    default: return String(status)
  }
}

const getTypeLabel = (type: number): string => {
  switch (type) {
    case 0: return 'Checkup'
    case 1: return 'Vaccination'
    case 2: return 'Surgery'
    case 3: return 'Emergency'
    case 4: return 'FollowUp'
    default: return String(type)
  }
}

const startEdit = (appointment: AppointmentResponseDto): void => {
  editingAppointment.value = appointment
  editForm.value = {
    date: toDateTimeLocal(appointment.date),
    status: appointment.status,
    type: appointment.type,
    notes: appointment.notes,
    doctorId: 0
  }
}

const cancelEdit = (): void => {
  editingAppointment.value = null
  editForm.value = {
    date: '',
    status: 0,
    type: 0,
    notes: '',
    doctorId: 0
  }
}

const handleUpdate = async (): Promise<void> => {
  if (!editingAppointment.value) return

  try {
    await updateAppointment(editingAppointment.value.id, {
      ...editForm.value,
      date: new Date(editForm.value.date).toISOString()
    })

    cancelEdit()
    await loadAppointments()
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-a putut modifica programarea.'
  }
}

const handleDelete = async (id: number): Promise<void> => {
  const confirmed = window.confirm('Sigur vrei să ștergi această programare?')
  if (!confirmed) return

  try {
    await deleteAppointment(id)
    await loadAppointments()
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-a putut șterge programarea.'
  }
}

onMounted(() => {
  loadAppointments()
})
</script>

<style scoped>
.admin-page {
  padding: 2rem;
  max-width: 1300px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 1.5rem;
}

.page-header h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  color: #1f2937;
}

.page-header p {
  color: #6b7280;
}

.table-card,
.edit-card {
  background: white;
  border-radius: 20px;
  padding: 1.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  overflow-x: auto;
  margin-bottom: 1.5rem;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  text-align: left;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
  vertical-align: top;
}

th {
  color: #374151;
  font-weight: 700;
}

td {
  color: #4b5563;
}

.actions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.edit-btn,
.delete-btn,
.save-btn,
.cancel-btn {
  border: none;
  border-radius: 10px;
  padding: 10px 14px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s ease;
}

.edit-btn,
.save-btn {
  background: #760f5c;
  color: white;
}

.edit-btn:hover,
.save-btn:hover {
  background: #5d0c49;
}

.delete-btn {
  background: #fee2e2;
  color: #b91c1c;
}

.delete-btn:hover {
  background: #fecaca;
}

.cancel-btn {
  background: #f3f4f6;
  color: #374151;
}

.cancel-btn:hover {
  background: #e5e7eb;
}

.edit-card h2 {
  margin-bottom: 1rem;
  color: #1f2937;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.4rem;
  font-weight: 600;
  color: #374151;
}

.form-group input,
.form-group textarea,
.form-group select {
  border: 1px solid #d1d5db;
  border-radius: 12px;
  padding: 0.85rem 1rem;
  font: inherit;
  background: white;
}

.full-width {
  grid-column: 1 / -1;
}

.form-actions {
  display: flex;
  gap: 12px;
  margin-top: 1.5rem;
}

.error-message {
  color: #dc2626;
  font-weight: 600;
}

@media (max-width: 768px) {
  .admin-page {
    padding: 1rem;
  }

  .form-actions {
    flex-direction: column;
  }

  .edit-btn,
  .delete-btn,
  .save-btn,
  .cancel-btn {
    width: 100%;
  }
}
</style>