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
  return new Date(value).toLocaleString('ro-RO', { dateStyle: 'long', timeStyle: 'short' })
}

function getStatusLabel(status: string | number) {
  const map: Record<string, string> = { '0': 'În așteptare', '1': 'Confirmată', '2': 'Anulată', '3': 'Finalizată' }
  return map[String(status)] ?? String(status)
}

function getStatusClass(status: string | number) {
  const map: Record<string, string> = { '0': 'pending', '1': 'confirmed', '2': 'cancelled', '3': 'completed' }
  return map[String(status)] ?? ''
}

function getTypeLabel(type: string | number) {
  const map: Record<string, string> = { '0': 'Control', '1': 'Vaccinare', '2': 'Intervenție', '3': 'Urgență', '4': 'Follow-up' }
  return map[String(type)] ?? String(type)
}

function getTypeIcon(type: string | number) {
  const map: Record<string, string> = { '0': '🩺', '1': '💉', '2': '🔬', '3': '🚨', '4': '📋' }
  return map[String(type)] ?? '📅'
}

function goToMedicalRecord() {
  if (details.value?.pet?.id) {
    router.push({ name: 'doctor-medical-record-create', params: { petId: details.value.pet.id } })
  }
}

function goToLabResults() {
  if (details.value?.pet?.id) {
    router.push({ name: 'doctor-lab-result-create', params: { petId: details.value.pet.id } })
  }
}

function goBack() {
  router.push('/doctor/appointments')
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

onMounted(() => { loadDetails() })
</script>

<template>
  <section class="page">
    <header class="page-header">
      <button class="back-btn" @click="goBack">← Înapoi la programări</button>
    </header>

    <div v-if="loading" class="state-box">Se încarcă detaliile programării...</div>
    <div v-else-if="error" class="state-box error">{{ error }}</div>

    <template v-else-if="details">
      <!-- Titlu + status -->
      <div class="appt-hero">
        <div class="hero-left">
          <span class="type-icon-big">{{ getTypeIcon(details.type) }}</span>
          <div>
            <h1>{{ getTypeLabel(details.type) }}</h1>
            <p class="hero-date">{{ formatDate(details.date) }}</p>
          </div>
        </div>
        <span class="status-badge-lg" :class="getStatusClass(details.status)">
          {{ getStatusLabel(details.status) }}
        </span>
      </div>

      <!-- Note -->
      <div v-if="details.notes" class="notes-box">
        <span class="notes-label">📝 Note</span>
        <p>{{ details.notes }}</p>
      </div>

      <!-- Grid info -->
      <div class="details-grid">
        <!-- Client -->
        <article class="details-card">
          <div class="card-header">
            <span class="card-icon">👤</span>
            <h2>Proprietar</h2>
          </div>
          <div class="detail-list">
            <div class="detail-row">
              <span class="detail-label">Nume</span>
              <span class="detail-value">{{ details.client.name }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Email</span>
              <span class="detail-value">{{ details.client.email }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Telefon</span>
              <span class="detail-value">{{ details.client.phone || '-' }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Adresă</span>
              <span class="detail-value">{{ details.client.address || '-' }}</span>
            </div>
          </div>
        </article>

        <!-- Animal -->
        <article class="details-card">
          <div class="card-header">
            <span class="card-icon">🐾</span>
            <h2>Animal</h2>
          </div>
          <div class="pet-image-wrap" v-if="details.pet.imageUrl">
            <img :src="details.pet.imageUrl" :alt="details.pet.name" class="pet-image" />
          </div>
          <div class="detail-list">
            <div class="detail-row">
              <span class="detail-label">Nume</span>
              <span class="detail-value font-bold">{{ details.pet.name }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Specie</span>
              <span class="detail-value">{{ details.pet.species }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Rasă</span>
              <span class="detail-value">{{ details.pet.breed }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Data nașterii</span>
              <span class="detail-value">{{ details.pet.birthdate ? formatDate(details.pet.birthdate) : '-' }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Vaccinuri</span>
              <span class="detail-value">{{ details.pet.vaccines || '-' }}</span>
            </div>
          </div>
        </article>
      </div>

      <!-- Acțiuni -->
      <div class="actions-section">
        <h3>Acțiuni rapide</h3>
        <div class="actions-grid">
          <button class="action-card" @click="goToMedicalRecord">
            <span class="action-icon">📋</span>
            <div>
              <div class="action-title">Adaugă fișă medicală</div>
              <div class="action-desc">Creează o fișă medicală pentru această consultație</div>
            </div>
            <span class="action-arrow">→</span>
          </button>
          <button class="action-card secondary" @click="goToLabResults">
            <span class="action-icon">🔬</span>
            <div>
              <div class="action-title">Adaugă rezultate analize</div>
              <div class="action-desc">Încarcă rezultatele de laborator ale animalului</div>
            </div>
            <span class="action-arrow">→</span>
          </button>
        </div>
      </div>
    </template>
  </section>
</template>

<style scoped>
.page { padding: 32px 24px; max-width: 1100px; margin: 0 auto; }

.back-btn { background: none; border: none; font-size: 0.95rem; color: #6b7280; cursor: pointer; padding: 0; font-weight: 600; margin-bottom: 24px; display: block; }
.back-btn:hover { color: #ec4899; }

/* Hero */
.appt-hero { display: flex; justify-content: space-between; align-items: center; gap: 20px; flex-wrap: wrap; margin-bottom: 20px; background: white; border-radius: 22px; padding: 24px; box-shadow: 0 4px 16px rgba(0,0,0,0.06); border: 1px solid #f3f4f6; }
.hero-left { display: flex; align-items: center; gap: 16px; }
.type-icon-big { font-size: 2.5rem; }
.appt-hero h1 { font-size: 1.6rem; font-weight: 800; color: #1f2937; margin: 0 0 4px; }
.hero-date { color: #6b7280; font-size: 0.95rem; margin: 0; }

.status-badge-lg { padding: 8px 18px; border-radius: 999px; font-weight: 700; font-size: 0.95rem; }
.status-badge-lg.pending { background: #fef3c7; color: #92400e; }
.status-badge-lg.confirmed { background: #dbeafe; color: #1d4ed8; }
.status-badge-lg.cancelled { background: #fee2e2; color: #b91c1c; }
.status-badge-lg.completed { background: #dcfce7; color: #166534; }

/* Note */
.notes-box { background: #fefce8; border: 1px solid #fde68a; border-radius: 16px; padding: 14px 18px; margin-bottom: 20px; }
.notes-label { font-weight: 700; color: #92400e; font-size: 0.85rem; display: block; margin-bottom: 4px; }
.notes-box p { margin: 0; color: #78350f; }

/* Grid */
.details-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 16px; margin-bottom: 24px; }
.details-card { background: white; border-radius: 22px; padding: 22px; box-shadow: 0 4px 16px rgba(0,0,0,0.06); border: 1px solid #f3f4f6; }
.card-header { display: flex; align-items: center; gap: 10px; margin-bottom: 18px; padding-bottom: 12px; border-bottom: 1px solid #f3f4f6; }
.card-icon { font-size: 1.3rem; }
.card-header h2 { margin: 0; font-size: 1.05rem; color: #1f2937; }

.detail-list { display: grid; gap: 10px; }
.detail-row { display: flex; justify-content: space-between; align-items: flex-start; gap: 12px; }
.detail-label { font-size: 0.85rem; color: #9ca3af; font-weight: 600; white-space: nowrap; }
.detail-value { font-size: 0.95rem; color: #1f2937; text-align: right; }
.font-bold { font-weight: 700; }

.pet-image-wrap { margin-bottom: 16px; }
.pet-image { width: 100px; height: 100px; object-fit: cover; border-radius: 16px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }

/* Acțiuni */
.actions-section h3 { font-size: 1rem; font-weight: 700; color: #374151; margin-bottom: 12px; }
.actions-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 14px; }
.action-card { background: white; border: 1px solid #f3f4f6; border-radius: 18px; padding: 18px; display: flex; align-items: center; gap: 14px; cursor: pointer; text-align: left; transition: all 0.2s; box-shadow: 0 2px 8px rgba(0,0,0,0.04); }
.action-card:hover { transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.1); border-color: #ec4899; }
.action-card.secondary:hover { border-color: #3b82f6; }
.action-icon { font-size: 1.8rem; flex-shrink: 0; }
.action-title { font-weight: 700; color: #1f2937; font-size: 0.95rem; margin-bottom: 2px; }
.action-desc { font-size: 0.8rem; color: #6b7280; }
.action-arrow { margin-left: auto; color: #d1d5db; font-size: 1.1rem; flex-shrink: 0; transition: all 0.2s; }
.action-card:hover .action-arrow { color: #ec4899; transform: translateX(4px); }
.action-card.secondary:hover .action-arrow { color: #3b82f6; }

.state-box { padding: 32px; border-radius: 18px; background: #f9fafb; color: #374151; text-align: center; border: 1px dashed #d1d5db; }
.state-box.error { background: #fef2f2; color: #b91c1c; border-color: #fecaca; }

@media (max-width: 768px) {
  .page { padding: 20px 14px; }
  .details-grid { grid-template-columns: 1fr; }
  .actions-grid { grid-template-columns: 1fr; }
  .appt-hero { flex-direction: column; align-items: flex-start; }
  .detail-row { flex-direction: column; gap: 2px; }
  .detail-value { text-align: left; }
}
</style>