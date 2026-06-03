<script setup lang="ts">
import { computed, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  createLabResult,
  TestType,
  type LabResultCreateDto
} from '../../api/services/labResultService'

const route = useRoute()
const router = useRouter()

const saving = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const petId = computed(() => Number(route.params.petId))

const form = reactive<LabResultCreateDto>({
  testType: TestType.BloodTest,
  filePath: '',
  keyValues: '{}',
  interpretation: '',
  petId: petId.value
})

function testTypeLabel(value: number): string {
  if (value === TestType.BloodTest) return 'Analiză sânge'
  if (value === TestType.UrineTest) return 'Analiză urină'
  if (value === TestType.XRay) return 'Radiografie'
  if (value === TestType.Ultrasound) return 'Ecografie'
  return 'Necunoscut'
}

async function handleSubmit() {
  try {
    errorMessage.value = ''
    successMessage.value = ''

    if (!petId.value || Number.isNaN(petId.value)) {
      errorMessage.value = 'ID-ul pacientului este invalid.'
      return
    }

    if (!form.interpretation.trim()) {
      errorMessage.value = 'Interpretarea este obligatorie.'
      return
    }

    saving.value = true
    form.petId = petId.value

    await createLabResult({
    testType: form.testType,
    filePath: form.filePath.trim(),
    keyValues: form.keyValues.trim() || '{}',
    interpretation: form.interpretation.trim(),
    petId: form.petId
    })

    successMessage.value = 'Rezultatul de laborator a fost adăugat cu succes.'

    setTimeout(() => {
      router.push({ name: 'doctor-patients' })
    }, 1000)
  } catch (error) {
    console.error('handleSubmit error:', error)
    errorMessage.value = 'A apărut o eroare la salvarea rezultatului.'
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push({ name: 'doctor-patients' })
}
</script>

<template>
  <div class="page-wrapper">
    <div class="form-card">
      <div class="page-header">
        <h1>Adaugă rezultat laborator</h1>
        <p>Completează informațiile rezultatului pentru pacientul selectat.</p>
      </div>

      <form class="lab-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="info-box success">
          {{ successMessage }}
        </div>

        <div class="form-group">
          <label for="petId">ID pacient</label>
          <input
            id="petId"
            :value="petId"
            type="text"
            disabled
          />
        </div>

        <div class="form-group">
          <label for="testType">Tip analiză</label>
          <select id="testType" v-model="form.testType">
            <option :value="TestType.BloodTest">
              {{ testTypeLabel(TestType.BloodTest) }}
            </option>
            <option :value="TestType.UrineTest">
              {{ testTypeLabel(TestType.UrineTest) }}
            </option>
            <option :value="TestType.XRay">
              {{ testTypeLabel(TestType.XRay) }}
            </option>
            <option :value="TestType.Ultrasound">
              {{ testTypeLabel(TestType.Ultrasound) }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="filePath">Cale fișier</label>
          <input
            id="filePath"
            v-model="form.filePath"
            type="text"
            placeholder="Ex: /uploads/lab-results/rezultat.pdf"
          />
        </div>

        <div class="form-group">
          <label for="keyValues">Valori cheie (JSON)</label>
          <textarea
            id="keyValues"
            v-model="form.keyValues"
            rows="6"
            placeholder='Ex: {"Hemoglobina":"13.2","Leucocite":"7.8"}'
          />
        </div>

        <div class="form-group">
          <label for="interpretation">Interpretare</label>
          <textarea
            id="interpretation"
            v-model="form.interpretation"
            rows="5"
            placeholder="Scrie interpretarea rezultatului"
          />
        </div>

        <div class="form-actions">
          <button
            type="button"
            class="secondary-btn"
            @click="goBack"
          >
            Înapoi
          </button>

          <button
            type="submit"
            class="primary-btn"
            :disabled="saving"
          >
            {{ saving ? 'Se salvează...' : 'Salvează rezultatul' }}
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

.form-card {
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

.lab-form {
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
.form-group select,
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
.form-group select:focus,
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
  transition: all 0.2s ease;
}

.primary-btn {
  background: #760f5c;
  color: white;
}

.primary-btn:hover {
  background: #5f0c49;
}

.primary-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.secondary-btn {
  background: #e5e7eb;
  color: #1f2937;
}

.secondary-btn:hover {
  background: #d1d5db;
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

  .form-card {
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