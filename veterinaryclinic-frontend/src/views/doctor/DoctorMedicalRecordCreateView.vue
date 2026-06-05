<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getDoctorProfile } from '../../api/services/doctorService'
import { createMedicalRecord, type MedicalRecordCreateDto } from '../../api/services/medicalRecordService'
import api from '../../api/axios'

interface DiagnosisDto {
  id: number
  name: string
  description: string
  treatment: string
  observations: string
  species: string
}

const route = useRoute()
const router = useRouter()

const loadingDoctor = ref(false)
const loadingDiagnoses = ref(false)
const saving = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const diagnoses = ref<DiagnosisDto[]>([])
const selectedDiagnosisId = ref<number | null>(null)
const searchQuery = ref('')
const showDropdown = ref(false)

const petId = computed(() => Number(route.params.petId))

const form = reactive<MedicalRecordCreateDto>({
  petId: petId.value,
  doctorId: 0,
  diagnosis: '',
  treatment: '',
  observations: ''
})

const filteredDiagnoses = computed(() => {
  const q = searchQuery.value.toLowerCase()
  return diagnoses.value.filter(d =>
    d.name.toLowerCase().includes(q) ||
    d.description.toLowerCase().includes(q)
  )
})

async function loadDoctorProfile() {
  try {
    loadingDoctor.value = true
    const doctor = await getDoctorProfile()
    form.doctorId = doctor.id
  } catch {
    errorMessage.value = 'Nu am putut încărca datele doctorului.'
  } finally {
    loadingDoctor.value = false
  }
}

async function loadDiagnoses() {
  try {
    loadingDiagnoses.value = true
    const response = await api.get<DiagnosisDto[]>('/Diagnosis')
    diagnoses.value = response.data
  } catch {
    errorMessage.value = 'Nu am putut încărca lista de diagnostice.'
  } finally {
    loadingDiagnoses.value = false
  }
}

function hideDropdown() {
  window.setTimeout(() => { showDropdown.value = false }, 150)
}

function selectDiagnosis(d: DiagnosisDto) {
  selectedDiagnosisId.value = d.id
  searchQuery.value = d.name
  form.diagnosis = d.name
  form.treatment = d.treatment
  form.observations = d.observations
  showDropdown.value = false
}

function clearDiagnosis() {
  selectedDiagnosisId.value = null
  searchQuery.value = ''
  form.diagnosis = ''
  form.treatment = ''
  form.observations = ''
}

async function handleSubmit() {
  try {
    errorMessage.value = ''
    successMessage.value = ''

    if (!petId.value || Number.isNaN(petId.value)) {
      errorMessage.value = 'ID-ul pacientului este invalid.'
      return
    }
    if (!form.doctorId) {
      errorMessage.value = 'ID-ul doctorului nu a fost găsit.'
      return
    }
    if (!form.diagnosis.trim()) {
      errorMessage.value = 'Diagnosticul este obligatoriu.'
      return
    }
    if (!form.treatment.trim()) {
      errorMessage.value = 'Tratamentul este obligatoriu.'
      return
    }

    saving.value = true
    form.petId = petId.value

    await createMedicalRecord({
      petId: form.petId,
      doctorId: form.doctorId,
      diagnosis: form.diagnosis.trim(),
      treatment: form.treatment.trim(),
      observations: form.observations.trim()
    })

    successMessage.value = 'Fișa medicală a fost adăugată cu succes.'
    setTimeout(() => {
      router.push({ name: 'doctor-medical-records', params: { petId: form.petId } })
    }, 1000)
  } catch {
    errorMessage.value = 'A apărut o eroare la salvarea fișei medicale.'
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push({ name: 'doctor-patients' })
}

onMounted(() => {
  loadDoctorProfile()
  loadDiagnoses()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Adaugă fișă medicală</h1>
        <p>Selectează un diagnostic predefinit sau completează manual câmpurile.</p>
      </div>

      <div v-if="loadingDoctor || loadingDiagnoses" class="info-box">
        Se încarcă datele necesare...
      </div>

      <form v-else class="profile-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">{{ errorMessage }}</div>
        <div v-if="successMessage" class="info-box success">{{ successMessage }}</div>

        <!-- ID pacient -->
        <div class="form-group">
          <label>ID pacient</label>
          <input :value="petId" type="text" disabled />
        </div>

        <!-- Selector diagnostic predefinit -->
        <div class="diagnosis-selector">
          <label>Selectează diagnostic predefinit</label>
          <p class="hint">Alege un diagnostic din baza de date — câmpurile se vor completa automat.</p>

          <div class="search-wrapper">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Caută după nume sau descriere..."
              class="search-input"
              @focus="showDropdown = true"
              @blur="hideDropdown"
            />
            <button v-if="selectedDiagnosisId" type="button" class="clear-btn" @click="clearDiagnosis">×</button>
          </div>

          <div v-if="showDropdown && filteredDiagnoses.length > 0" class="dropdown">
            <div
              v-for="d in filteredDiagnoses"
              :key="d.id"
              class="dropdown-item"
              @mousedown="selectDiagnosis(d)"
            >
              <div class="diag-name">{{ d.name }}</div>
              <div class="diag-meta">
                <span class="diag-desc">{{ d.description }}</span>
                <span v-if="d.species" class="species-chip">{{ d.species }}</span>
              </div>
            </div>
          </div>

          <div v-if="showDropdown && filteredDiagnoses.length === 0" class="dropdown">
            <div class="dropdown-empty">Niciun diagnostic găsit</div>
          </div>
        </div>

        <div class="divider">
          <span>sau completează manual</span>
        </div>

        <!-- Câmpuri formular -->
        <div class="form-group">
          <label for="diagnosis">Diagnostic *</label>
          <input
            id="diagnosis"
            v-model="form.diagnosis"
            type="text"
            placeholder="Introduce diagnosticul"
          />
        </div>

        <div class="form-group">
          <label for="treatment">Tratament *</label>
          <textarea
            id="treatment"
            v-model="form.treatment"
            rows="5"
            placeholder="Introduce tratamentul recomandat"
          />
        </div>

        <div class="form-group">
          <label for="observations">Observații</label>
          <textarea
            id="observations"
            v-model="form.observations"
            rows="4"
            placeholder="Introduce observațiile medicale"
          />
        </div>

        <div class="form-actions">
          <button type="button" class="secondary-btn" @click="goBack">Înapoi</button>
          <button type="submit" class="primary-btn" :disabled="saving">
            {{ saving ? 'Se salvează...' : 'Salvează fișa' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper { max-width: 900px; margin: 0 auto; padding: 3rem 2rem; }
.profile-card { background: white; border-radius: 24px; padding: 2rem; box-shadow: 0 14px 32px rgba(0,0,0,0.08); }
.page-header { text-align: center; margin-bottom: 2rem; }
.page-header h1 { font-size: 2.1rem; font-weight: 800; color: #1f2937; margin-bottom: 0.75rem; }
.page-header p { color: #6b7280; }
.profile-form { display: grid; gap: 1.25rem; }
.form-group { display: grid; gap: 0.55rem; }
.form-group label { font-weight: 600; color: #374151; }
.form-group input, .form-group textarea {
  border: 1px solid #d1d5db; border-radius: 14px; padding: 0.9rem 1rem;
  font-size: 1rem; color: #1f2937; background: #fff; resize: vertical;
}
.form-group input:focus, .form-group textarea:focus {
  outline: none; border-color: #760f5c;
  box-shadow: 0 0 0 4px rgba(118,15,92,0.12);
}
.form-group input:disabled { background: #f3f4f6; color: #6b7280; cursor: not-allowed; }

/* Diagnosis selector */
.diagnosis-selector {
  background: #fdf4ff;
  border: 1px solid #e9d5ff;
  border-radius: 18px;
  padding: 1.25rem;
  display: grid;
  gap: 0.6rem;
  position: relative;
}
.diagnosis-selector label { font-weight: 700; color: #6d28d9; font-size: 1rem; }
.hint { margin: 0; font-size: 0.85rem; color: #7c3aed; }

.search-wrapper { position: relative; }
.search-input {
  width: 100%; border: 1.5px solid #c4b5fd; border-radius: 12px;
  padding: 0.85rem 2.5rem 0.85rem 1rem; font-size: 1rem;
  color: #1f2937; background: white; box-sizing: border-box;
}
.search-input:focus { outline: none; border-color: #7c3aed; box-shadow: 0 0 0 3px rgba(124,58,237,0.15); }
.clear-btn {
  position: absolute; right: 0.75rem; top: 50%; transform: translateY(-50%);
  background: none; border: none; font-size: 1.3rem; color: #9ca3af;
  cursor: pointer; line-height: 1;
}
.clear-btn:hover { color: #374151; }

.dropdown {
  position: absolute; left: 1.25rem; right: 1.25rem; top: calc(100% - 0.5rem);
  background: white; border: 1px solid #e5e7eb; border-radius: 14px;
  box-shadow: 0 8px 24px rgba(0,0,0,0.12); z-index: 50;
  max-height: 280px; overflow-y: auto;
}
.dropdown-item {
  padding: 0.85rem 1rem; cursor: pointer; border-bottom: 1px solid #f3f4f6;
  transition: background 0.15s;
}
.dropdown-item:last-child { border-bottom: none; }
.dropdown-item:hover { background: #f5f3ff; }
.diag-name { font-weight: 600; color: #1f2937; font-size: 0.95rem; }
.diag-meta { display: flex; align-items: center; gap: 0.5rem; margin-top: 0.2rem; flex-wrap: wrap; }
.diag-desc { font-size: 0.82rem; color: #6b7280; }
.species-chip {
  display: inline-block; padding: 0.1rem 0.55rem; border-radius: 999px;
  background: #e0f2fe; color: #0369a1; font-size: 0.75rem; font-weight: 600;
}
.dropdown-empty { padding: 1rem; text-align: center; color: #9ca3af; font-size: 0.9rem; }

/* Divider */
.divider {
  display: flex; align-items: center; gap: 1rem;
  color: #9ca3af; font-size: 0.85rem;
}
.divider::before, .divider::after {
  content: ''; flex: 1; border-top: 1px dashed #e5e7eb;
}

.form-actions { display: flex; justify-content: flex-end; gap: 0.75rem; margin-top: 0.5rem; }
.primary-btn, .secondary-btn {
  border: none; border-radius: 14px; padding: 0.9rem 1.2rem;
  font-size: 1rem; cursor: pointer; font-weight: 600;
}
.primary-btn { background: #760f5c; color: white; }
.primary-btn:hover { background: #5f0c49; }
.primary-btn:disabled { opacity: 0.7; cursor: not-allowed; }
.secondary-btn { background: #e5e7eb; color: #1f2937; }
.secondary-btn:hover { background: #d1d5db; }

.info-box { border-radius: 16px; padding: 1rem 1.2rem; background: #f9fafb; color: #374151; }
.info-box.error { background: #fee2e2; color: #b91c1c; }
.info-box.success { background: #dcfce7; color: #166534; }

@media (max-width: 768px) {
  .page-wrapper { padding: 2rem 1rem; }
  .profile-card { padding: 1.5rem; }
  .page-header h1 { font-size: 1.8rem; }
  .form-actions { flex-direction: column; }
  .primary-btn, .secondary-btn { width: 100%; }
}
</style>