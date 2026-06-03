<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  getLabResultsByPetId,
  TestType,
  type LabResultResponseDto
} from '../../api/services/labResultService'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const errorMessage = ref('')
const results = ref<LabResultResponseDto[]>([])

const petId = computed(() => Number(route.params.petId))

function testTypeLabel(value: number): string {
  if (value === TestType.BloodTest) return 'Analiză sânge'
  if (value === TestType.UrineTest) return 'Analiză urină'
  if (value === TestType.XRay) return 'Radiografie'
  if (value === TestType.Ultrasound) return 'Ecografie'
  return 'Necunoscut'
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('ro-RO')
}

async function loadLabResults() {
  try {
    loading.value = true
    errorMessage.value = ''

    if (!petId.value || Number.isNaN(petId.value)) {
      errorMessage.value = 'ID-ul pacientului este invalid.'
      return
    }

    results.value = await getLabResultsByPetId(petId.value)
  } catch (error) {
    console.error('loadLabResults error:', error)
    errorMessage.value = 'Nu am putut încărca rezultatele de laborator.'
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push({ name: 'doctor-patients' })
}

onMounted(() => {
  loadLabResults()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Rezultate laborator</h1>
        <p>Vizualizează rezultatele de laborator pentru pacientul selectat.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă rezultatele de laborator...
      </div>

      <div v-else-if="errorMessage" class="info-box error">
        {{ errorMessage }}
      </div>

      <div v-else-if="results.length === 0" class="info-box">
        Nu există rezultate de laborator pentru acest pacient.
      </div>

      <div v-else class="results-list">
        <div
          v-for="result in results"
          :key="result.id"
          class="result-row"
        >
          <div class="result-info">
            <h2>{{ testTypeLabel(result.testType) }}</h2>
            <p><strong>Pacient:</strong> {{ result.petName }}</p>
            <p><strong>Data:</strong> {{ formatDate(result.date) }}</p>
            <p><strong>Interpretare:</strong> {{ result.interpretation }}</p>
            <p><strong>Valori cheie:</strong> {{ result.keyValues }}</p>

            <a
              v-if="result.filePath"
              :href="result.filePath"
              target="_blank"
              rel="noopener noreferrer"
              class="file-link"
            >
              Deschide fișierul rezultatului
            </a>
          </div>
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

.results-list {
  display: grid;
  gap: 1.25rem;
}

.result-row {
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 1.2rem;
  background: #f9fafb;
}

.result-info h2 {
  margin-bottom: 0.75rem;
  font-size: 1.15rem;
  color: #1f2937;
}

.result-info p {
  margin-bottom: 0.5rem;
  color: #374151;
}

.file-link {
  display: inline-block;
  margin-top: 0.5rem;
  color: #760f5c;
  font-weight: 600;
  text-decoration: none;
}

.file-link:hover {
  text-decoration: underline;
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