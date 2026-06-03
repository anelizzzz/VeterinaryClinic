<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getDoctorPatients, type DoctorPatientDto } from '../../api/services/doctorService'

const router = useRouter()

const loading = ref(false)
const errorMessage = ref('')
const patients = ref<DoctorPatientDto[]>([])

async function loadPatients() {
  try {
    loading.value = true
    errorMessage.value = ''
    patients.value = await getDoctorPatients()
  } catch (error) {
    console.error('loadPatients error:', error)
    errorMessage.value = 'Nu am putut încărca pacienții.'
  } finally {
    loading.value = false
  }
}

function calculateAgeLabel(age: number): string {
  return age === 1 ? '1 an' : `${age} ani`
}

function goToAddLabResult(petId: number) {
  router.push({
    name: 'doctor-lab-result-create',
    params: { petId }
  })
}

function goToViewLabResults(petId: number) {
  router.push({
    name: 'doctor-lab-results',
    params: { petId }
  })
}

function goToAddMedicalRecord(petId: number) {
  router.push({
    name: 'doctor-medical-record-create',
    params: { petId }
  })
}

function goToViewMedicalRecords(petId: number) {
  router.push({
    name: 'doctor-medical-records',
    params: { petId }
  })
}
function goToPatientDetails(petId: number) {
  router.push({
    name: 'doctor-patient-details',
    params: { petId }
  })
}
onMounted(() => {
  loadPatients()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Pacienții mei</h1>
        <p>Vizualizează animalele tale paciente și accesează rapid fișele și rezultatele de laborator.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă pacienții...
      </div>

      <div v-else-if="errorMessage" class="info-box error">
        {{ errorMessage }}
      </div>

      <div v-else-if="patients.length === 0" class="info-box">
        Nu există pacienți disponibili.
      </div>

      <div v-else class="patients-list">
        <div
          v-for="patient in patients"
          :key="patient.id"
          class="patient-row"
        >
          <div class="patient-info">
            <h2>{{ patient.name }}</h2>
            <p>{{ patient.species }} • {{ patient.breed }}</p>
            <small>{{ patient.ownerName }} • {{ calculateAgeLabel(patient.age) }}</small>
          </div>

          <div class="actions-grid">
            <button class="primary-btn" @click="goToAddLabResult(patient.id)">
              Adaugă rezultat laborator
            </button>

            <button class="secondary-btn" @click="goToViewLabResults(patient.id)">
              Vezi rezultat laborator
            </button>

            <button class="primary-btn" @click="goToAddMedicalRecord(patient.id)">
              Adaugă fișă
            </button>

            <button class="secondary-btn" @click="goToViewMedicalRecords(patient.id)">
              Vezi fișă
            </button>
            <button class="secondary-btn" @click="goToPatientDetails(patient.id)">
            Vezi detalii pacient
            </button>
          </div>
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

.patients-list {
  display: grid;
  gap: 1.25rem;
}

.patient-row {
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 1.2rem;
  background: #f9fafb;
  display: grid;
  gap: 1rem;
}

.patient-info h2 {
  margin-bottom: 0.35rem;
  font-size: 1.15rem;
  color: #1f2937;
}

.patient-info p {
  color: #4b5563;
  margin-bottom: 0.25rem;
}

.patient-info small {
  color: #6b7280;
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 0.75rem;
}

.primary-btn,
.secondary-btn {
  border: none;
  border-radius: 14px;
  padding: 0.9rem 1rem;
  font-size: 0.95rem;
  cursor: pointer;
  transition: 0.2s ease;
}

.primary-btn {
  background: #760f5c;
  color: white;
}

.primary-btn:hover {
  background: #5f0c49;
}

.secondary-btn {
  background: #ffffff;
  color: #760f5c;
  border: 1px solid #d8b4cf;
}

.secondary-btn:hover {
  background: #faf5f8;
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

  .actions-grid {
    grid-template-columns: 1fr;
  }
}
</style>