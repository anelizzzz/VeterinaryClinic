<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getLabResultsByPetId, type LabResultResponseDto, TestType } from '../../api/services/labResultService'
import { generateAiDiagnosis, type AIDiagnosisResponseDto } from '../../api/services/aiDiagnosisService'

const route = useRoute()

const loading = ref(false)
const error = ref('')
const results = ref<LabResultResponseDto[]>([])

const aiLoading = ref(false)
const aiError = ref('')
const aiResult = ref<AIDiagnosisResponseDto | null>(null)
const showAiModal = ref(false)

const petId = computed(() => Number(route.params.petId))

function formatDate(dateString: string | null) {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('ro-RO')
}

function getTestTypeText(testType: number) {
  switch (testType) {
    case TestType.BloodTest:
      return 'Analiză sânge'
    case TestType.UrineTest:
      return 'Analiză urină'
    case TestType.XRay:
      return 'Radiografie'
    case TestType.Ultrasound:
      return 'Ecografie'
    default:
      return 'Necunoscut'
  }
}

function closeAiModal() {
  showAiModal.value = false
}

async function handleGenerateAiDiagnosis(result: LabResultResponseDto) {
  try {
    aiLoading.value = true
    aiError.value = ''
    aiResult.value = null
    showAiModal.value = true

    aiResult.value = await generateAiDiagnosis({
      petName: result.petName ?? '',
      species: '',
      breed: '',
      age: 0,
      diagnosis: result.interpretation || 'Rezultat de laborator',
      treatment: '',
      observations: '',
      labResults: [result.keyValues || result.interpretation || '']
    })
  } catch (err) {
    console.error(err)
    aiError.value = 'Nu am putut genera diagnosticul AI.'
  } finally {
    aiLoading.value = false
  }
}

async function loadResults() {
  try {
    loading.value = true
    error.value = ''

    if (!petId.value || Number.isNaN(petId.value)) {
      error.value = 'ID-ul pacientului este invalid.'
      return
    }

    results.value = await getLabResultsByPetId(petId.value)
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut încărca rezultatele laborator.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadResults()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Rezultate laborator</h1>
        <p>Vezi istoricul rezultatelor de laborator ale animalului selectat.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă rezultatele...
      </div>

      <div v-else-if="error" class="info-box error">
        {{ error }}
      </div>

      <div v-else-if="results.length === 0" class="info-box">
        Nu există rezultate de laborator pentru acest animal.
      </div>

      <div v-else class="results-list">
        <div v-for="result in results" :key="result.id" class="result-card">
          <div class="result-top">
            <h2>{{ getTestTypeText(result.testType) }}</h2>
            <span class="date-badge">{{ formatDate(result.date) }}</span>
          </div>

          <div class="result-details">
            <p><strong>Pacient:</strong> {{ result.petName }}</p>
            <p><strong>Interpretare:</strong> {{ result.interpretation }}</p>
            <p><strong>Valori cheie:</strong> {{ result.keyValues }}</p>
          </div>

          <a
            v-if="result.filePath"
            :href="result.filePath"
            target="_blank"
            rel="noopener noreferrer"
            class="file-link"
          >
            Deschide fișierul rezultatului
          </a>

          <div class="result-actions">
            <button
              class="primary-btn"
              :disabled="aiLoading"
              @click="handleGenerateAiDiagnosis(result)"
            >
              {{ aiLoading ? 'Se detectează...' : 'Detectează un diagnostic' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <Teleport to="body">
      <div v-if="showAiModal" class="modal-backdrop" @click.self="closeAiModal">
        <div class="modal-card">
          <div class="modal-header">
            <h3>Diagnostic aproximativ AI</h3>
            <button class="close-btn" @click="closeAiModal">×</button>
          </div>

          <div class="modal-body">
            <div v-if="aiLoading" class="info-box">
              Se generează diagnosticul...
            </div>

            <div v-else-if="aiError" class="info-box error">
              {{ aiError }}
            </div>

            <div v-else-if="aiResult" class="ai-result">
              <p><strong>Cauze posibile:</strong> {{ aiResult.possibleCauses.join(', ') }}</p>
              <p><strong>Urgență:</strong> {{ aiResult.urgencyLevel }}</p>
              <p><strong>Pași recomandați:</strong> {{ aiResult.recommendedNextSteps }}</p>
              <p><strong>Încredere:</strong> {{ aiResult.confidence }}%</p>
              <small>{{ aiResult.disclaimer }}</small>
            </div>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.page-wrapper {
  max-width: 1100px;
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
  gap: 1rem;
}

.result-card {
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 1.2rem;
  background: #f9fafb;
  display: grid;
  gap: 1rem;
}

.result-top {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  align-items: center;
}

.result-top h2 {
  margin: 0;
  font-size: 1.15rem;
  color: #1f2937;
}

.date-badge {
  display: inline-block;
  padding: 0.4rem 0.75rem;
  border-radius: 999px;
  background: #e0f2fe;
  color: #0369a1;
  font-size: 0.85rem;
  font-weight: 700;
}

.result-details {
  display: grid;
  gap: 0.5rem;
  color: #374151;
}

.file-link {
  color: #760f5c;
  font-weight: 700;
  text-decoration: none;
}

.file-link:hover {
  text-decoration: underline;
}

.result-actions {
  display: flex;
  justify-content: flex-end;
}

.primary-btn {
  border: none;
  border-radius: 14px;
  padding: 0.9rem 1.2rem;
  font-size: 1rem;
  cursor: pointer;
  background: linear-gradient(135deg, #7c3aed, #5b21b6);
  color: white;
  font-weight: 700;
  transition: all 0.2s ease;
  box-shadow: 0 8px 18px rgba(91, 33, 182, 0.18);
}

.primary-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 24px rgba(91, 33, 182, 0.25);
}

.primary-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
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

.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
  z-index: 1000;
}

.modal-card {
  width: min(720px, 100%);
  background: white;
  border-radius: 20px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
  overflow: hidden;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.25rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.2rem;
  color: #1f2937;
}

.close-btn {
  border: none;
  background: transparent;
  font-size: 1.6rem;
  cursor: pointer;
  line-height: 1;
  color: #6b7280;
}

.modal-body {
  padding: 1.25rem;
}

.ai-result {
  display: grid;
  gap: 0.75rem;
  color: #374151;
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

  .result-top {
    flex-direction: column;
    align-items: flex-start;
  }

  .result-actions {
    justify-content: stretch;
  }

  .primary-btn {
    width: 100%;
  }
}
</style>