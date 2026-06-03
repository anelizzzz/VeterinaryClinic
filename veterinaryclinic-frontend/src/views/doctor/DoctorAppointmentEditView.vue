<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  getAppointmentById,
  updateAppointment,
  type AppointmentResponseDto,
  type AppointmentUpdateDto
} from '../../api/services/appointmentService'

const route = useRoute()
const router = useRouter()

const appointmentId = Number(route.params.id)

const loading = ref(false)
const saving = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const form = reactive<AppointmentUpdateDto>({
  date: '',
  type: 0,
  status: 0,
  notes: '',
  doctorId: 0
})

function formatDateForInput(dateString: string) {
  if (!dateString) return ''
  const date = new Date(dateString)
  const pad = (n: number) => String(n).padStart(2, '0')

  const year = date.getFullYear()
  const month = pad(date.getMonth() + 1)
  const day = pad(date.getDate())
  const hours = pad(date.getHours())
  const minutes = pad(date.getMinutes())

  return `${year}-${month}-${day}T${hours}:${minutes}`
}

async function loadAppointment() {
  try {
    loading.value = true
    errorMessage.value = ''
    successMessage.value = ''

    const data: AppointmentResponseDto = await getAppointmentById(appointmentId)

    form.date = formatDateForInput(data.date)
    form.type = Number(data.type)
    form.status = Number(data.status)
    form.notes = data.notes || ''
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Nu am putut încărca programarea.'
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  try {
    errorMessage.value = ''
    successMessage.value = ''

    if (!form.date) {
      errorMessage.value = 'Data programării este obligatorie.'
      return
    }

    saving.value = true

  await updateAppointment(appointmentId, {
  date: form.date,
  type: form.type,
  status: form.status,
  notes: form.notes,
  doctorId: form.doctorId
})

    successMessage.value = 'Programarea a fost actualizată cu succes.'
  } catch (error) {
    console.error(error)
    errorMessage.value = 'A apărut o eroare la salvare.'
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push(`/doctor/appointments/${appointmentId}`)
}

onMounted(() => {
  loadAppointment()
})
</script>

<template>
  <section class="page-wrapper">
    <div class="form-card">
      <div class="page-header">
        <h1>Editează programarea</h1>
        <p>Modifică data, tipul, statusul și notele programării.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă datele programării...
      </div>

      <form v-else class="appointment-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="info-box success">
          {{ successMessage }}
        </div>

        <div class="form-group">
          <label for="date">Data programării</label>
          <input id="date" v-model="form.date" type="datetime-local" />
        </div>

        <div class="form-group">
          <label for="type">Tip programare</label>
          <select id="type" v-model.number="form.type">
            <option :value="0">Control</option>
            <option :value="1">Vaccinare</option>
            <option :value="2">Intervenție</option>
            <option :value="3">Urgență</option>
            <option :value="4">Follow-up</option>
          </select>
        </div>

        <div class="form-group">
          <label for="status">Status</label>
          <select id="status" v-model.number="form.status">
            <option :value="0">În așteptare</option>
            <option :value="1">Confirmată</option>
            <option :value="2">Anulată</option>
            <option :value="3">Finalizată</option>
          </select>
        </div>

        <div class="form-group">
          <label for="notes">Note</label>
          <textarea
            id="notes"
            v-model="form.notes"
            rows="5"
            placeholder="Adaugă observații despre programare"
          />
        </div>

        <div class="form-actions">
          <button type="button" class="secondary-btn" @click="goBack">
            Înapoi
          </button>
          <button type="submit" class="primary-btn" :disabled="saving">
            {{ saving ? 'Se salvează...' : 'Salvează modificările' }}
          </button>
        </div>
      </form>
    </div>
  </section>
</template>

<style scoped>
.page-wrapper {
  max-width: 900px;
  margin: 0 auto;
  padding: 3rem 2rem;
}

.form-card {
  background: white;
  border-radius: 24px;
  padding: 2rem;
  box-shadow: 0 14px 32px rgba(0, 0, 0, 0.08);
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.1rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.page-header p {
  color: #6b7280;
}

.appointment-form {
  display: grid;
  gap: 1.25rem;
}

.form-group {
  display: grid;
  gap: 0.55rem;
}

.form-group label {
  font-weight: 600;
  color: #374151;
}

.form-group input,
.form-group textarea,
.form-group select {
  border: 1px solid #d1d5db;
  border-radius: 14px;
  padding: 0.9rem 1rem;
  font-size: 1rem;
  color: #1f2937;
  background: #fff;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
  border-color: #ec4899;
  box-shadow: 0 0 0 4px rgba(236, 72, 153, 0.12);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  flex-wrap: wrap;
}

.primary-btn,
.secondary-btn {
  border: none;
  border-radius: 14px;
  padding: 0.9rem 1.2rem;
  font-size: 1rem;
  cursor: pointer;
}

.primary-btn {
  background: #ec4899;
  color: white;
}

.secondary-btn {
  background: #f3f4f6;
  color: #1f2937;
}

.primary-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.info-box {
  border-radius: 16px;
  padding: 1rem 1.2rem;
  background: #f9fafb;
  color: #374151;
}

.info-box.error {
  background: #fee2e2;
  color: #b91c1c;
}

.info-box.success {
  background: #dcfce7;
  color: #166534;
}

@media (max-width: 768px) {
  .page-wrapper {
    padding: 2rem 1rem;
  }

  .form-card {
    padding: 1.5rem;
  }

  .page-header h1 {
    font-size: 1.8rem;
  }
}
</style>