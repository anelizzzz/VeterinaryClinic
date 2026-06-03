<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  getMedicalRecordsByPetId,
  type MedicalRecordResponseDto
} from '../../api/services/medicalRecordService'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const errorMessage = ref('')
const records = ref<MedicalRecordResponseDto[]>([])

const petId = computed(() => Number(route.params.petId))

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('ro-RO')
}

async function loadMedicalRecords() {
  try {
    loading.value = true
    errorMessage.value = ''

    if (!petId.value || Number.isNaN(petId.value)) {
      errorMessage.value = 'ID-ul pacientului este invalid.'
      return
    }

    records.value = await getMedicalRecordsByPetId(petId.value)
  } catch (error) {
    console.error('loadMedicalRecords error:', error)
    errorMessage.value = 'Nu am putut încărca fișele medicale.'
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push({ name: 'doctor-patients' })
}

onMounted(() => {
  loadMedicalRecords()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Fișe medicale</h1>
        <p>Vizualizează istoricul fișelor medicale pentru pacientul selectat.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă fișele medicale...
      </div>

      <div v-else-if="errorMessage" class="info-box error">
        {{ errorMessage }}
      </div>

      <div v-else-if="records.length === 0" class="info-box">
        Nu există fișe medicale pentru acest pacient.
      </div>

      <div v-else class="records-list">
        <div
          v-for="record in records"
          :key="record.id"
          class="record-row"
        >
          <h2>{{ record.petName }}</h2>
          <p><strong>Data:</strong> {{ formatDate(record.date) }}</p>
          <p><strong>Doctor:</strong> {{ record.doctorName }}</p>
          <p><strong>Diagnostic:</strong> {{ record.diagnosis }}</p>
          <p><strong>Tratament:</strong> {{ record.treatment }}</p>
          <p><strong>Observații:</strong> {{ record.observations }}</p>
        </div>
      </div>

      <div class="form-actions">
        <button class="primary-btn" @click="goBack">
          Înapoi la pacienți
        </button>
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

.records-list {
  display: grid;
  gap: 1.25rem;
}

.record-row {
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 1.2rem;
  background: #f9fafb;
}

.record-row h2 {
  margin-bottom: 0.75rem;
  font-size: 1.15rem;
  color: #1f2937;
}

.record-row p {
  margin-bottom: 0.5rem;
  color: #374151;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 1.5rem;
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