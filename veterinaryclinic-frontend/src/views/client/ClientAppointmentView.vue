<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import {
  getAllAppointments,
  type AppointmentResponseDto
} from '../../api/services/appointmentService'

const appointments = ref<AppointmentResponseDto[]>([])
const loading = ref(false)
const error = ref('')

const hasAppointments = computed(() => appointments.value.length > 0)

function formatDateTime(dateString: string) {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleString('ro-RO')
}

function getStatusText(status: number) {
  switch (status) {
    case 0:
      return 'În așteptare'
    case 1:
      return 'Confirmată'
    case 2:
      return 'Finalizată'
    case 3:
      return 'Anulată'
    default:
      return 'Necunoscut'
  }
}

function getTypeText(type: number) {
  switch (type) {
    case 0:
      return 'Consult'
    case 1:
      return 'Control'
    case 2:
      return 'Vaccinare'
    case 3:
      return 'Urgență'
    default:
      return 'Alt tip'
  }
}

async function loadAppointments() {
  try {
    loading.value = true
    error.value = ''
    appointments.value = await getAllAppointments()
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut încărca programările.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadAppointments()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="page-header">
      <h1>Programările mele</h1>
      <p>Vezi programările active și istoricul vizitelor tale.</p>
    </div>

    <div v-if="loading" class="info-box">
      Se încarcă programările...
    </div>

    <div v-else-if="error" class="info-box error">
      {{ error }}
    </div>

    <div v-else-if="!hasAppointments" class="info-box">
      Nu ai programări momentan.
    </div>

    <div v-else class="appointment-list">
      <article
        v-for="appointment in appointments"
        :key="appointment.id"
        class="appointment-card"
      >
        <div class="card-top">
          <div>
            <h2>{{ appointment.petName }}</h2>
            <p class="appointment-type">{{ getTypeText(appointment.type) }}</p>
          </div>

          <span class="status-badge">
            {{ getStatusText(appointment.status) }}
          </span>
        </div>

        <div class="appointment-details">
          <p><strong>Doctor:</strong> {{ appointment.doctorName }}</p>
          <p><strong>Client:</strong> {{ appointment.clientName }}</p>
          <p><strong>Data:</strong> {{ formatDateTime(appointment.date) }}</p>
          <p><strong>Observații:</strong> {{ appointment.notes || 'Fără observații' }}</p>
        </div>
      </article>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper {
  max-width: 1100px;
  margin: 0 auto;
  padding: 3rem 2rem;
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.2rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.page-header p {
  color: #6b7280;
  font-size: 1rem;
}

.info-box {
  max-width: 700px;
  margin: 0 auto;
  background: white;
  border-radius: 18px;
  padding: 1.5rem;
  text-align: center;
  color: #374151;
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.06);
}

.info-box.error {
  color: #b91c1c;
}

.appointment-list {
  display: grid;
  gap: 1.5rem;
}

.appointment-card {
  background: white;
  border-radius: 22px;
  padding: 1.75rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
}

.card-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1.2rem;
  flex-wrap: wrap;
}

.card-top h2 {
  font-size: 1.25rem;
  color: #1f2937;
  margin-bottom: 0.3rem;
}

.appointment-type {
  color: #6b7280;
}

.status-badge {
  display: inline-block;
  padding: 0.45rem 0.8rem;
  border-radius: 999px;
  background: #ede9fe;
  color: #6d28d9;
  font-size: 0.9rem;
  font-weight: 600;
}

.appointment-details {
  display: grid;
  gap: 0.65rem;
}

.appointment-details p {
  color: #4b5563;
  line-height: 1.5;
}

@media (max-width: 768px) {
  .page-wrapper {
    padding: 2rem 1rem;
  }

  .page-header h1 {
    font-size: 1.9rem;
  }
}
</style>