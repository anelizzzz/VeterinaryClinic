<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  getDoctorPatientDetails,
  type DoctorPatientDetailsDto
} from '../../api/services/doctorService'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const errorMessage = ref('')
const patient = ref<DoctorPatientDetailsDto | null>(null)

const petId = computed(() => Number(route.params.petId))

async function loadPatientDetails() {
  try {
    loading.value = true
    errorMessage.value = ''
    patient.value = await getDoctorPatientDetails(petId.value)
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Nu am putut încărca detaliile pacientului.'
  } finally {
    loading.value = false
  }
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('ro-RO')
}

function goBack() {
  router.push({ name: 'doctor-patients' })
}

onMounted(() => {
  loadPatientDetails()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Detalii pacient</h1>
        <p>Vizualizează informațiile principale ale pacientului.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă detaliile pacientului...
      </div>

      <div v-else-if="errorMessage" class="info-box error">
        {{ errorMessage }}
      </div>

      <div v-else-if="patient" class="details-grid">
        <div class="image-box" v-if="patient.imageUrl">
          <img :src="patient.imageUrl" alt="Imagine pacient" class="pet-image" />
        </div>

        <div class="detail-item">
          <strong>ID:</strong> {{ patient.id }}
        </div>
        <div class="detail-item">
          <strong>Nume:</strong> {{ patient.name }}
        </div>
        <div class="detail-item">
          <strong>Specie:</strong> {{ patient.species }}
        </div>
        <div class="detail-item">
          <strong>Rasă:</strong> {{ patient.breed }}
        </div>
        <div class="detail-item">
          <strong>Data nașterii:</strong> {{ formatDate(patient.birthdate) }}
        </div>
        <div class="detail-item">
          <strong>Status:</strong> {{ patient.status }}
        </div>
        <div class="detail-item">
          <strong>Vaccinuri:</strong> {{ patient.vaccines || 'Nespecificate' }}
        </div>
        <div class="detail-item">
          <strong>Client ID:</strong> {{ patient.clientId }}
        </div>
        <div class="detail-item">
          <strong>Stăpân:</strong> {{ patient.ownerName }}
        </div>

        <div class="form-actions">
          <button class="primary-btn" @click="goBack">
            Înapoi la pacienți
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper {
  max-width: 900px;
  margin: 0 auto;
  padding: 3rem 2rem;
}

.profile-card {
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

.details-grid {
  display: grid;
  gap: 1rem;
}

.image-box {
  display: flex;
  justify-content: center;
  margin-bottom: 1rem;
}

.pet-image {
  max-width: 220px;
  border-radius: 18px;
  object-fit: cover;
}

.detail-item {
  background: #f9fafb;
  border-radius: 14px;
  padding: 1rem 1.1rem;
  color: #374151;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 0.5rem;
}

.primary-btn {
  border: none;
  border-radius: 14px;
  padding: 0.9rem 1.2rem;
  font-size: 1rem;
  cursor: pointer;
  background: #760f5c;
  color: white;
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

@media (max-width: 768px) {
  .page-wrapper {
    padding: 2rem 1rem;
  }

  .profile-card {
    padding: 1.5rem;
  }

  .page-header h1 {
    font-size: 1.8rem;
  }
}
</style>