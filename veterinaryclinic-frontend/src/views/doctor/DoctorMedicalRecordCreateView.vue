<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getDoctorProfile } from '../../api/services/doctorService'
import {
  createMedicalRecord,
  type MedicalRecordCreateDto
} from '../../api/services/medicalRecordService'

const route = useRoute()
const router = useRouter()

const loadingDoctor = ref(false)
const saving = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const doctorId = ref<number | null>(null)

const petId = computed(() => Number(route.params.petId))

const form = reactive<MedicalRecordCreateDto>({
  petId: petId.value,
  doctorId: 0,
  diagnosis: '',
  treatment: '',
  observations: ''
})

async function loadDoctorProfile() {
  try {
    loadingDoctor.value = true
    const doctor = await getDoctorProfile()
    doctorId.value = doctor.id
    form.doctorId = doctor.id
  } catch (error) {
    console.error('loadDoctorProfile error:', error)
    errorMessage.value = 'Nu am putut încărca datele doctorului.'
  } finally {
    loadingDoctor.value = false
  }
}

async function handleSubmit() {
  try {
    errorMessage.value = ''
    successMessage.value = ''

    if (!petId.value || Number.isNaN(petId.value)) {
      errorMessage.value = 'ID-ul pacientului este invalid.'
      return
    }

    if (!doctorId.value) {
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
    form.doctorId = doctorId.value

    await createMedicalRecord({
      petId: form.petId,
      doctorId: form.doctorId,
      diagnosis: form.diagnosis.trim(),
      treatment: form.treatment.trim(),
      observations: form.observations.trim()
    })

    successMessage.value = 'Fișa medicală a fost adăugată cu succes.'

    setTimeout(() => {
      router.push({
        name: 'doctor-medical-records',
        params: { petId: form.petId }
      })
    }, 1000)
  } catch (error) {
    console.error('handleSubmit error:', error)
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
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Adaugă fișă medicală</h1>
        <p>Completează datele fișei pentru pacientul selectat.</p>
      </div>

      <div v-if="loadingDoctor" class="info-box">
        Se încarcă datele necesare...
      </div>

      <form v-else class="profile-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="info-box success">
          {{ successMessage }}
        </div>

        <div class="form-group">
          <label for="petId">ID pacient</label>
          <input id="petId" :value="petId" type="text" disabled />
        </div>

        <div class="form-group">
          <label for="diagnosis">Diagnostic</label>
          <input
            id="diagnosis"
            v-model="form.diagnosis"
            type="text"
            placeholder="Introduce diagnosticul"
          />
        </div>

        <div class="form-group">
          <label for="treatment">Tratament</label>
          <textarea
            id="treatment"
            v-model="form.treatment"
            rows="4"
            placeholder="Introduce tratamentul recomandat"
          />
        </div>

        <div class="form-group">
          <label for="observations">Observații</label>
          <textarea
            id="observations"
            v-model="form.observations"
            rows="5"
            placeholder="Introduce observațiile medicale"
          />
        </div>

        <div class="form-actions">
          <button type="button" class="secondary-btn" @click="goBack">
            Înapoi
          </button>

          <button type="submit" class="primary-btn" :disabled="saving">
            {{ saving ? 'Se salvează...' : 'Salvează fișa' }}
          </button>
        </div>
      </form>
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

.profile-form {
  display: grid;
  gap: 1.25rem;
}

.form-group {
  display: grid;
  gap: 0.55rem;
}

.form-group label {
  font-weight: 600;
  color: #374151;
}

.form-group input,
.form-group textarea {
  border: 1px solid #d1d5db;
  border-radius: 14px;
  padding: 0.9rem 1rem;
  font-size: 1rem;
  color: #1f2937;
  background: #fff;
  resize: vertical;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #760f5c;
  box-shadow: 0 0 0 4px rgba(118, 15, 92, 0.12);
}

.form-group input:disabled {
  background: #f3f4f6;
  color: #6b7280;
  cursor: not-allowed;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  margin-top: 0.5rem;
}

.primary-btn,
.secondary-btn {
  border: none;
  border-radius: 14px;
  padding: 0.9rem 1.2rem;
  font-size: 1rem;
  cursor: pointer;
}

.primary-btn {
  background: #760f5c;
  color: white;
}

.primary-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.secondary-btn {
  background: #e5e7eb;
  color: #1f2937;
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

.info-box.success {
  background: #dcfce7;
  color: #166534;
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

  .form-actions {
    flex-direction: column;
  }

  .primary-btn,
  .secondary-btn {
    width: 100%;
  }
}
</style>