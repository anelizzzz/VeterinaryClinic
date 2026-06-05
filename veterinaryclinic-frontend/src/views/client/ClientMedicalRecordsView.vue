<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getMedicalRecordsByPetId, type MedicalRecordResponseDto } from '../../api/services/medicalRecordService'
import api from '../../api/axios'

const route = useRoute()

const loading = ref(false)
const error = ref('')
const records = ref<MedicalRecordResponseDto[]>([])
const downloadingId = ref<number | null>(null)

const petId = computed(() => Number(route.params.petId))

function formatDate(dateString: string | null) {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('ro-RO')
}

async function downloadPdf(record: MedicalRecordResponseDto) {
  try {
    downloadingId.value = record.id
    const response = await api.get(`/MedicalRecords/${record.id}/pdf`, {
      responseType: 'blob'
    })
    const url = URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }))
    const link = document.createElement('a')
    link.href = url
    link.download = `fisa-medicala-${record.petName}-${formatDate(record.date)}.pdf`
    link.click()
    URL.revokeObjectURL(url)
  } catch (err) {
    console.error(err)
    alert('Nu am putut descărca PDF-ul.')
  } finally {
    downloadingId.value = null
  }
}

async function loadData() {
  try {
    loading.value = true
    error.value = ''
    if (!petId.value || Number.isNaN(petId.value)) {
      error.value = 'ID-ul pacientului este invalid.'
      return
    }
    records.value = await getMedicalRecordsByPetId(petId.value)
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut încărca fișele medicale.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Fișe medicale</h1>
        <p>Vezi istoricul fișelor medicale ale animalului selectat.</p>
      </div>

      <div v-if="loading" class="info-box">Se încarcă fișele medicale...</div>
      <div v-else-if="error" class="info-box error">{{ error }}</div>
      <div v-else-if="records.length === 0" class="info-box">Nu există fișe medicale pentru acest animal.</div>

      <div v-else class="records-list">
        <div v-for="record in records" :key="record.id" class="record-card">
          <div class="record-top">
            <h2>{{ record.petName }}</h2>
            <span class="date-badge">{{ formatDate(record.date) }}</span>
          </div>

          <div class="record-details">
            <p><strong>Doctor:</strong> {{ record.doctorName }}</p>
            <p><strong>Diagnostic:</strong> {{ record.diagnosis }}</p>
            <p><strong>Tratament:</strong> {{ record.treatment }}</p>
            <p><strong>Observații:</strong> {{ record.observations }}</p>
          </div>

          <div class="record-actions">
            <button
              class="pdf-btn"
              :disabled="downloadingId === record.id"
              @click="downloadPdf(record)"
            >
              {{ downloadingId === record.id ? 'Se generează...' : '📄 Descarcă PDF' }}
            </button>
          </div>
        </div>
      </div>
    </div>
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

.page-header p { color: #6b7280; }

.records-list { display: grid; gap: 1rem; }

.record-card {
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 1.2rem;
  background: #f9fafb;
  display: grid;
  gap: 1rem;
}

.record-top {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  align-items: center;
}

.record-top h2 { margin: 0; font-size: 1.15rem; color: #1f2937; }

.date-badge {
  display: inline-block;
  padding: 0.4rem 0.75rem;
  border-radius: 999px;
  background: #fce7f3;
  color: #be185d;
  font-size: 0.85rem;
  font-weight: 700;
}

.record-details { display: grid; gap: 0.5rem; color: #374151; }

.record-actions { display: flex; justify-content: flex-end; }

.pdf-btn {
  border: none;
  border-radius: 12px;
  padding: 0.7rem 1.1rem;
  font-size: 0.95rem;
  cursor: pointer;
  background: linear-gradient(135deg, #be185d, #9d174d);
  color: white;
  font-weight: 700;
  transition: all 0.2s ease;
  box-shadow: 0 6px 14px rgba(190, 24, 93, 0.2);
}

.pdf-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(190, 24, 93, 0.28);
}

.pdf-btn:disabled {
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

.info-box.error { background: #fee2e2; color: #b91c1c; }

@media (max-width: 768px) {
  .page-wrapper { padding: 2rem 1rem; }
  .profile-card { padding: 1.5rem; }
  .page-header h1 { font-size: 1.8rem; }
  .record-top { flex-direction: column; align-items: flex-start; }
  .record-actions { justify-content: stretch; }
  .pdf-btn { width: 100%; }
}
</style>