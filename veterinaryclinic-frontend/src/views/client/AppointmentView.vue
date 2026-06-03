<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getAllPets, type PetResponseDto } from '../../api/services/petService'
import { getAllDoctors, type DoctorResponseDto } from '../../api/services/doctorService'
import {
  createAppointment,
  type AppointmentCreateDto
} from '../../api/services/appointmentService'

const router = useRouter()

const pets = ref<PetResponseDto[]>([])
const doctors = ref<DoctorResponseDto[]>([])
const loading = ref(false)
const saving = ref(false)
const error = ref('')

const form = reactive<AppointmentCreateDto>({
  date: '',
  type: 0,
  notes: '',
  clientId: 0,
  doctorId: 0,
  petId: 0
})

async function loadFormData() {
  try {
    loading.value = true
    error.value = ''
    pets.value = await getAllPets()
    doctors.value = await getAllDoctors()
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut încărca datele pentru formular.'
  } finally {
    loading.value = false
  }
}

async function submitForm() {
  if (!form.petId || !form.doctorId || !form.date) {
    error.value = 'Completează toate câmpurile obligatorii.'
    return
  }

  try {
    saving.value = true
    error.value = ''

    await createAppointment(form)

    alert('Programarea a fost creată cu succes.')
    router.push('/my-appointments')
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut salva programarea.'
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadFormData()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="form-card">
      <div class="page-header">
        <h1>Programează o vizită</h1>
        <p>Completează formularul pentru a crea o nouă programare.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă datele formularului...
      </div>

      <form v-else class="appointment-form" @submit.prevent="submitForm">
        <div v-if="error" class="info-box error">
          {{ error }}
        </div>

        <div class="form-group">
          <label for="petId">Animal</label>
          <select id="petId" v-model.number="form.petId">
            <option :value="0">Selectează animalul</option>
            <option v-for="pet in pets" :key="pet.id" :value="pet.id">
              {{ pet.name }} - {{ pet.species }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="doctorId">Doctor</label>
          <select id="doctorId" v-model.number="form.doctorId">
            <option :value="0">Selectează doctorul</option>
            <option v-for="doctor in doctors" :key="doctor.id" :value="doctor.id">
              {{ doctor.name }} - {{ doctor.specialization }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="date">Data și ora</label>
          <input id="date" v-model="form.date" type="datetime-local" />
        </div>

        <div class="form-group">
          <label for="type">Tip programare</label>
          <select id="type" v-model.number="form.type">
            <option :value="0">Consult</option>
            <option :value="1">Control</option>
            <option :value="2">Vaccinare</option>
            <option :value="3">Urgență</option>
          </select>
        </div>

        <div class="form-group">
          <label for="notes">Observații</label>
          <textarea
            id="notes"
            v-model="form.notes"
            rows="4"
            placeholder="Scrie detalii suplimentare despre problemă"
          />
        </div>

        <div class="form-actions">
          <button type="button" class="secondary-btn" @click="router.push('/dashboard')">
            Anulează
          </button>
          <button type="submit" class="primary-btn" :disabled="saving">
            {{ saving ? 'Se salvează...' : 'Trimite programarea' }}
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

.appointment-form {
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
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #ec4899;
  box-shadow: 0 0 0 4px rgba(236, 72, 153, 0.12);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  flex-wrap: wrap;
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
  background: #ec4899;
  color: white;
}

.primary-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.secondary-btn {
  background: #f3f4f6;
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
}
</style>