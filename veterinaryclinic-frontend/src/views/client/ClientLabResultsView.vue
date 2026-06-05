<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getLabResultsByPetId, type LabResultResponseDto, TestType } from '../../api/services/labResultService'
import { generateAiDiagnosis, type AIDiagnosisResponseDto } from '../../api/services/aiDiagnosisService'
import api from '../../api/axios'

const route = useRoute()
const loading = ref(false)
const error = ref('')
const results = ref<LabResultResponseDto[]>([])
const aiLoading = ref(false)
const aiError = ref('')
const aiResult = ref<AIDiagnosisResponseDto | null>(null)
const showAiModal = ref(false)
const downloadingId = ref<number | null>(null)
const petId = computed(() => Number(route.params.petId))

function formatDate(dateString: string | null) {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('ro-RO')
}

function getTestTypeText(testType: number) {
  switch (testType) {
    case TestType.BloodTest: return 'Analiză sânge'
    case TestType.UrineTest: return 'Analiză urină'
    case TestType.XRay: return 'Radiografie'
    case TestType.Ultrasound: return 'Ecografie'
    default: return 'Necunoscut'
  }
}

function formatConfidence(value: number): string {
  const pct = value <= 1 ? Math.round(value * 100) : Math.round(value)
  return `${pct}%`
}

function urgencyClass(level: string): string {
  const l = level?.toLowerCase() ?? ''
  if (l.includes('critic')) return 'urgency-critical'
  if (l.includes('ridicat')) return 'urgency-high'
  if (l.includes('mediu')) return 'urgency-medium'
  return 'urgency-low'
}

function closeAiModal() { showAiModal.value = false }

async function downloadPdf(result: LabResultResponseDto) {
  try {
    downloadingId.value = result.id
    const response = await api.get(`/LabResults/${result.id}/pdf`, { responseType: 'blob' })
    const url = URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }))
    const link = document.createElement('a')
    link.href = url
    link.download = `rezultat-laborator-${result.petName}-${formatDate(result.date)}.pdf`
    link.click()
    URL.revokeObjectURL(url)
  } catch (err) {
    console.error(err)
    alert('Nu am putut descărca PDF-ul.')
  } finally {
    downloadingId.value = null
  }
}

async function handleGenerateAiDiagnosis(result: LabResultResponseDto) {
  try {
    aiLoading.value = true
    aiError.value = ''
    aiResult.value = null
    showAiModal.value = true

    // Construim context complet pentru AI
    const labResults: string[] = []
    if (result.keyValues && result.keyValues !== '{}') labResults.push(`Valori cheie: ${result.keyValues}`)
    if (result.interpretation) labResults.push(`Interpretare medic: ${result.interpretation}`)
    labResults.push(`Tip test: ${getTestTypeText(result.testType)}`)
    labResults.push(`Data analizei: ${formatDate(result.date)}`)
    if (result.vaccines) labResults.push(`Vaccinuri: ${result.vaccines}`)

    aiResult.value = await generateAiDiagnosis({
      petName: result.petName || 'Necunoscut',
      species: result.species || '',
      breed: result.breed || '',
      age: result.ageYears || 0,
      diagnosis: result.interpretation || '',
      treatment: '',
      observations: '',
      labResults
    })
  } catch (err) {
    console.error(err)
    aiError.value = 'Nu am putut genera diagnosticul AI. Verifică conexiunea și încearcă din nou.'
  } finally {
    aiLoading.value = false
  }
}

async function loadResults() {
  try {
    loading.value = true
    error.value = ''
    if (!petId.value || Number.isNaN(petId.value)) { error.value = 'ID-ul pacientului este invalid.'; return }
    results.value = await getLabResultsByPetId(petId.value)
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut încărca rezultatele laborator.'
  } finally {
    loading.value = false
  }
}

onMounted(() => { loadResults() })
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Rezultate laborator</h1>
        <p>Vezi istoricul rezultatelor de laborator ale animalului selectat.</p>
      </div>
      <div v-if="loading" class="info-box">Se încarcă rezultatele...</div>
      <div v-else-if="error" class="info-box error">{{ error }}</div>
      <div v-else-if="results.length === 0" class="info-box">Nu există rezultate de laborator pentru acest animal.</div>
      <div v-else class="results-list">
        <div v-for="result in results" :key="result.id" class="result-card">
          <div class="result-top">
            <h2>{{ getTestTypeText(result.testType) }}</h2>
            <span class="date-badge">{{ formatDate(result.date) }}</span>
          </div>

          <!-- Date animal -->
          <div class="pet-info-row">
            <span class="pet-chip">🐾 {{ result.petName }}</span>
            <span v-if="result.species" class="pet-chip">{{ result.species }}</span>
            <span v-if="result.breed" class="pet-chip">{{ result.breed }}</span>
            <span v-if="result.ageYears" class="pet-chip">{{ result.ageYears }} ani</span>
          </div>

          <div class="result-details">
            <p><strong>Interpretare:</strong> {{ result.interpretation }}</p>
            <p><strong>Valori cheie:</strong> {{ result.keyValues }}</p>
          </div>

          <a v-if="result.filePath" :href="result.filePath" target="_blank" rel="noopener noreferrer" class="file-link">
            Deschide fișierul rezultatului
          </a>

          <div class="result-actions">
            <button class="pdf-btn" :disabled="downloadingId === result.id" @click="downloadPdf(result)">
              {{ downloadingId === result.id ? 'Se generează...' : '📄 Descarcă PDF' }}
            </button>
            <button class="primary-btn" :disabled="aiLoading" @click="handleGenerateAiDiagnosis(result)">
              {{ aiLoading ? 'Se analizează...' : '🤖 Analiză AI' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <Teleport to="body">
      <div v-if="showAiModal" class="modal-backdrop" @click.self="closeAiModal">
        <div class="modal-card">
          <div class="modal-header">
            <h3>🤖 Diagnostic aproximativ AI</h3>
            <button class="close-btn" @click="closeAiModal">×</button>
          </div>
          <div class="modal-body">
            <div v-if="aiLoading" class="info-box loading-box">
              <span class="spinner" />Se analizează rezultatele de laborator...
            </div>
            <div v-else-if="aiError" class="info-box error">{{ aiError }}</div>
            <div v-else-if="aiResult" class="ai-result">
              <div class="ai-section">
                <h4>Cauze posibile</h4>
                <ul class="causes-list">
                  <li v-for="cause in aiResult.possibleCauses" :key="cause">{{ cause }}</li>
                </ul>
              </div>
              <div class="ai-section">
                <h4>Nivel urgență</h4>
                <span :class="['urgency-badge', urgencyClass(aiResult.urgencyLevel)]">{{ aiResult.urgencyLevel }}</span>
              </div>
              <div class="ai-section">
                <h4>Pași recomandați</h4>
                <p>{{ aiResult.recommendedNextSteps }}</p>
              </div>
              <div class="ai-section confidence-row">
                <h4>Grad de încredere</h4>
                <div class="confidence-bar-wrapper">
                  <div class="confidence-bar" :style="{ width: formatConfidence(aiResult.confidence) }" />
                </div>
                <span class="confidence-pct">{{ formatConfidence(aiResult.confidence) }}</span>
              </div>
              <div class="disclaimer-box">
                <small>⚠️ {{ aiResult.disclaimer }}</small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.page-wrapper { max-width: 1100px; margin: 0 auto; padding: 3rem 2rem; }
.profile-card { background: white; border-radius: 24px; padding: 2rem; box-shadow: 0 14px 32px rgba(0,0,0,0.08); }
.page-header { text-align: center; margin-bottom: 2rem; }
.page-header h1 { font-size: 2.1rem; font-weight: 800; color: #1f2937; margin-bottom: 0.75rem; }
.page-header p { color: #6b7280; }
.results-list { display: grid; gap: 1rem; }
.result-card { border: 1px solid #e5e7eb; border-radius: 18px; padding: 1.2rem; background: #f9fafb; display: grid; gap: 1rem; }
.result-top { display: flex; justify-content: space-between; gap: 1rem; align-items: center; }
.result-top h2 { margin: 0; font-size: 1.15rem; color: #1f2937; }
.date-badge { display: inline-block; padding: 0.4rem 0.75rem; border-radius: 999px; background: #e0f2fe; color: #0369a1; font-size: 0.85rem; font-weight: 700; }

.pet-info-row { display: flex; flex-wrap: wrap; gap: 0.5rem; }
.pet-chip { display: inline-block; padding: 0.25rem 0.7rem; border-radius: 999px; background: #f3e8ff; color: #6d28d9; font-size: 0.82rem; font-weight: 600; }

.result-details { display: grid; gap: 0.5rem; color: #374151; }
.file-link { color: #760f5c; font-weight: 700; text-decoration: none; }
.file-link:hover { text-decoration: underline; }
.result-actions { display: flex; justify-content: flex-end; gap: 0.75rem; }

.pdf-btn { border: none; border-radius: 12px; padding: 0.9rem 1.2rem; font-size: 1rem; cursor: pointer; background: linear-gradient(135deg, #be185d, #9d174d); color: white; font-weight: 700; transition: all 0.2s ease; box-shadow: 0 8px 18px rgba(190,24,93,0.18); }
.pdf-btn:hover { transform: translateY(-2px); box-shadow: 0 12px 24px rgba(190,24,93,0.28); }
.pdf-btn:disabled { opacity: 0.7; cursor: not-allowed; transform: none; box-shadow: none; }

.primary-btn { border: none; border-radius: 14px; padding: 0.9rem 1.2rem; font-size: 1rem; cursor: pointer; background: linear-gradient(135deg, #7c3aed, #5b21b6); color: white; font-weight: 700; transition: all 0.2s ease; box-shadow: 0 8px 18px rgba(91,33,182,0.18); }
.primary-btn:hover { transform: translateY(-2px); box-shadow: 0 12px 24px rgba(91,33,182,0.25); }
.primary-btn:disabled { opacity: 0.7; cursor: not-allowed; transform: none; box-shadow: none; }

.info-box { border-radius: 16px; padding: 1rem 1.2rem; background: #f9fafb; color: #374151; }
.info-box.error { background: #fee2e2; color: #b91c1c; }
.loading-box { display: flex; align-items: center; gap: 0.75rem; }
.spinner { width: 20px; height: 20px; border: 3px solid #e5e7eb; border-top-color: #7c3aed; border-radius: 50%; animation: spin 0.8s linear infinite; flex-shrink: 0; }
@keyframes spin { to { transform: rotate(360deg); } }

.modal-backdrop { position: fixed; inset: 0; background: rgba(0,0,0,0.45); display: flex; align-items: center; justify-content: center; padding: 1rem; z-index: 1000; }
.modal-card { width: min(720px, 100%); background: white; border-radius: 20px; box-shadow: 0 20px 60px rgba(0,0,0,0.2); overflow: hidden; max-height: 90vh; display: flex; flex-direction: column; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 1rem 1.25rem; border-bottom: 1px solid #e5e7eb; flex-shrink: 0; }
.modal-header h3 { margin: 0; font-size: 1.2rem; color: #1f2937; }
.close-btn { border: none; background: transparent; font-size: 1.6rem; cursor: pointer; line-height: 1; color: #6b7280; }
.modal-body { padding: 1.25rem; overflow-y: auto; }
.ai-result { display: grid; gap: 1.25rem; color: #374151; }
.ai-section h4 { margin: 0 0 0.5rem; font-size: 0.85rem; text-transform: uppercase; letter-spacing: 0.05em; color: #6b7280; }
.causes-list { margin: 0; padding-left: 1.25rem; display: grid; gap: 0.3rem; }
.urgency-badge { display: inline-block; padding: 0.35rem 0.9rem; border-radius: 999px; font-weight: 700; font-size: 0.9rem; }
.urgency-low { background: #dcfce7; color: #166534; }
.urgency-medium { background: #fef9c3; color: #854d0e; }
.urgency-high { background: #ffedd5; color: #9a3412; }
.urgency-critical { background: #fee2e2; color: #991b1b; }
.confidence-row { display: grid; gap: 0.5rem; }
.confidence-bar-wrapper { background: #e5e7eb; border-radius: 999px; height: 10px; overflow: hidden; }
.confidence-bar { height: 100%; background: linear-gradient(90deg, #7c3aed, #5b21b6); border-radius: 999px; transition: width 0.6s ease; }
.confidence-pct { font-weight: 700; color: #5b21b6; font-size: 0.95rem; }
.disclaimer-box { background: #fef3c7; border-radius: 12px; padding: 0.75rem 1rem; color: #92400e; font-size: 0.85rem; line-height: 1.5; }

@media (max-width: 768px) {
  .page-wrapper { padding: 2rem 1rem; }
  .profile-card { padding: 1.5rem; }
  .page-header h1 { font-size: 1.8rem; }
  .result-top { flex-direction: column; align-items: flex-start; }
  .result-actions { flex-direction: column; }
  .pdf-btn, .primary-btn { width: 100%; }
}
</style>