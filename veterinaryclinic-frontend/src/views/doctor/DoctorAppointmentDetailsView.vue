<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getAppointmentDetails, type AppointmentDetailsDto } from '../../api/services/appointmentService'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const error = ref('')
const details = ref<AppointmentDetailsDto | null>(null)

const appointmentId = Number(route.params.id)

function formatDate(value: string) {
  return new Date(value).toLocaleString('ro-RO', {
    dateStyle: 'medium',
    timeStyle: 'short'
  })
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

async function loadDetails() {
  try {
    loading.value = true
    error.value = ''
    details.value = await getAppointmentDetails(appointmentId)
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-au putut încărca detaliile programării.'
  } finally {
    loading.value = false
  }
}

function goToEdit() {
  router.push(`/doctor/appointments/edit/${appointmentId}`)
}

function goToMedicalRecord() {
  router.push(`/doctor/appointments/${appointmentId}/medical-record`)
}

function goToLabResults() {
  router.push(`/doctor/appointments/${appointmentId}/lab-results`)
}

function goBack() {
  router.push('/doctor/appointments')
}

onMounted(() => {
  console.log('route id:', route.params.id)
  loadDetails()
})
</script>

<template>
  <section class="page">
    <header class="page-header">
      <div>
        <span class="eyebrow">Programare</span>
        <h1>Detalii programare</h1>
        <p>Vezi informațiile despre client, animal și consultație.</p>
      </div>

      <div class="header-actions">
        <button class="secondary-btn" @click="goBack">Înapoi</button>
        <button class="primary-btn" @click="goToEdit">Editează programarea</button>
      </div>
    </header>

    <div v-if="loading" class="state-box">
      Se încarcă detaliile programării...
    </div>

    <div v-else-if="error" class="state-box error">
      {{ error }}
    </div>

    <template v-else-if="details">
      <div class="details-grid">
        <article class="details-card">
          <h2>Detalii client</h2>
          <div class="detail-list">
            <p><strong>Nume:</strong> {{ details.client.name }}</p>
            <p><strong>Email:</strong> {{ details.client.email }}</p>
            <p><strong>Telefon:</strong> {{ details.client.phone }}</p>
            <p><strong>Adresă:</strong> {{ details.client.address || '-' }}</p>
          </div>
        </article>

        <article class="details-card">
          <h2>Detalii animal</h2>
          <div v-if="details.pet.imageUrl" class="pet-image-wrap">
            <img :src="details.pet.imageUrl" :alt="details.pet.name" class="pet-image" />
          </div>

          <div class="detail-list">
            <p><strong>Nume:</strong> {{ details.pet.name }}</p>
            <p><strong>Specie:</strong> {{ details.pet.species }}</p>
            <p><strong>Rasă:</strong> {{ details.pet.breed }}</p>
            <p><strong>Data nașterii:</strong> {{ details.pet.birthdate ? formatDate(details.pet.birthdate) : '-' }}</p>
            <p><strong>Vaccinuri:</strong> {{ details.pet.vaccines || '-' }}</p>
          </div>
        </article>

        <article class="details-card full-width">
          <h2>Detalii programare</h2>
          <div class="detail-grid">
            <p><strong>Data:</strong> {{ formatDate(details.date) }}</p>
            <p><strong>Status:</strong> {{ getStatusLabel(details.status) }}</p>
            <p><strong>Tip:</strong> {{ getTypeLabel(details.type) }}</p>
            <p><strong>Note:</strong> {{ details.notes || '-' }}</p>
          </div>
        </article>
      </div>

      <div class="actions-bar">
        <button class="primary-btn" @click="goToMedicalRecord">
          Adaugă fișă medicală
        </button>
        <button class="secondary-btn" @click="goToLabResults">
          Adaugă rezultate analize
        </button>
      </div>
    </template>
  </section>
</template>

<style scoped>
.page {
  padding: 32px 24px;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 20px;
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
  font-size: clamp(2rem, 1.4rem + 1.5vw, 2.6rem);
  margin-bottom: 8px;
  color: #1f2937;
}

.page-header p {
  color: #6b7280;
}

.header-actions {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.details-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 20px;
}

.details-card {
  background: #fff;
  border-radius: 24px;
  padding: 22px;
  box-shadow: 0 16px 40px rgba(15, 23, 42, 0.08);
  border: 1px solid #f3f4f6;
}

.details-card h2 {
  margin-bottom: 16px;
  color: #111827;
  font-size: 1.15rem;
}

.full-width {
  grid-column: 1 / -1;
}

.detail-list,
.detail-grid {
  display: grid;
  gap: 12px;
}

.detail-list p,
.detail-grid p {
  color: #4b5563;
  line-height: 1.5;
}

.pet-image-wrap {
  margin-bottom: 18px;
}

.pet-image {
  width: 140px;
  height: 140px;
  object-fit: cover;
  border-radius: 18px;
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.08);
}

.actions-bar {
  margin-top: 24px;
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.primary-btn,
.secondary-btn {
  border: none;
  border-radius: 14px;
  padding: 12px 16px;
  font-weight: 700;
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

@media (max-width: 768px) {
  .page {
    padding: 20px 14px;
  }

  .details-grid {
    grid-template-columns: 1fr;
  }
}
</style>