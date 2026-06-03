<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getMedicalRecordsByPetId, type MedicalRecordResponseDto } from '../../api/services/medicalRecordService'

const route = useRoute()

const loading = ref(false)
const error = ref('')
const records = ref<MedicalRecordResponseDto[]>([])

const petId = computed(() => Number(route.params.petId))

function formatDate(dateString: string | null) {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('ro-RO')
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

      <div v-if="loading" class="info-box">
        Se încarcă fișele medicale...
      </div>

      <div v-else-if="error" class="info-box error">
        {{ error }}
      </div>

      <div v-else-if="records.length === 0" class="info-box">
        Nu există fișe medicale pentru acest animal.
      </div>

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

.page-header p {
  color: #6b7280;
}

.records-list {
  display: grid;
  gap: 1rem;
}

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

.record-top h2 {
  margin: 0;
  font-size: 1.15rem;
  color: #1f2937;
}

.date-badge {
  display: inline-block;
  padding: 0.4rem 0.75rem;
  border-radius: 999px;
  background: #fce7f3;
  color: #be185d;
  font-size: 0.85rem;
  font-weight: 700;
}

.record-details {
  display: grid;
  gap: 0.5rem;
  color: #374151;
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

  .record-top {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>