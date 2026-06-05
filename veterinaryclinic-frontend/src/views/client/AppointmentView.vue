<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getAllPets, type PetResponseDto } from '../../api/services/petService'
import { getAllDoctors, type DoctorResponseDto } from '../../api/services/doctorService'
import { createAppointment, type AppointmentCreateDto } from '../../api/services/appointmentService'
import { getClientProfile } from '../../api/services/clientService'

const router = useRouter()

const pets = ref<PetResponseDto[]>([])
const doctors = ref<DoctorResponseDto[]>([])
const loading = ref(false)
const saving = ref(false)
const error = ref('')
const success = ref(false)
const clientEmail = ref('')

const form = reactive<AppointmentCreateDto>({
  date: '',
  type: 0,
  notes: '',
  clientId: 0,
  doctorId: 0,
  petId: 0
})

const selectedPetName = ref('')
const selectedDoctorName = ref('')

async function loadFormData() {
  try {
    loading.value = true
    error.value = ''
    const [petsData, doctorsData, clientData] = await Promise.all([
      getAllPets(),
      getAllDoctors(),
      getClientProfile()
    ])
    pets.value = petsData
    doctors.value = doctorsData
    form.clientId = clientData.id ?? 0
    clientEmail.value = clientData.email ?? ''
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut încărca datele pentru formular.'
  } finally {
    loading.value = false
  }
}

function onPetChange() {
  const pet = pets.value.find(p => p.id === form.petId)
  selectedPetName.value = pet?.name ?? ''
}

function onDoctorChange() {
  const doctor = doctors.value.find(d => d.id === form.doctorId)
  selectedDoctorName.value = doctor?.name ?? ''
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleString('ro-RO', { dateStyle: 'long', timeStyle: 'short' })
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
    success.value = true
    window.scrollTo({ top: 0, behavior: 'smooth' })
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut salva programarea. Încearcă din nou.'
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

      <!-- SUCCES -->
      <div v-if="success" class="success-screen">
        <div class="success-icon">✅</div>
        <h2>Programare creată cu succes!</h2>
        <p class="success-sub">
          Un email de confirmare a fost trimis la
          <strong>{{ clientEmail }}</strong>.
          Verifică inbox-ul (și folderul Spam dacă nu apare).
        </p>

        <div class="success-summary">
          <div class="summary-row">
            <span class="summary-icon">🐾</span>
            <div>
              <div class="summary-label">Pacient</div>
              <div class="summary-value">{{ selectedPetName }}</div>
            </div>
          </div>
          <div class="summary-row">
            <span class="summary-icon">🩺</span>
            <div>
              <div class="summary-label">Doctor</div>
              <div class="summary-value">{{ selectedDoctorName }}</div>
            </div>
          </div>
          <div class="summary-row">
            <span class="summary-icon">📅</span>
            <div>
              <div class="summary-label">Data</div>
              <div class="summary-value">{{ formatDate(form.date) }}</div>
            </div>
          </div>
        </div>

        <div class="success-actions">
          <button class="primary-btn" @click="router.push('/my-appointments')">
            Vezi programările mele
          </button>
          <button class="secondary-btn" @click="success = false; form.date = ''; form.petId = 0; form.doctorId = 0; form.notes = ''">
            Adaugă altă programare
          </button>
        </div>
      </div>

      <!-- FORMULAR -->
      <template v-else>
        <div class="page-header">
          <h1>Programează o vizită</h1>
          <p>Completează formularul pentru a crea o nouă programare.</p>
        </div>

        <div v-if="loading" class="info-box">
          Se încarcă datele formularului...
        </div>

        <form v-else class="appointment-form" @submit.prevent="submitForm">
          <div v-if="error" class="info-box error">{{ error }}</div>

          <div class="form-group">
            <label for="petId">Animal *</label>
            <select id="petId" v-model.number="form.petId" @change="onPetChange">
              <option :value="0">Selectează animalul</option>
              <option v-for="pet in pets" :key="pet.id" :value="pet.id">
                {{ pet.name }} — {{ pet.species }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="doctorId">Doctor *</label>
            <select id="doctorId" v-model.number="form.doctorId" @change="onDoctorChange">
              <option :value="0">Selectează doctorul</option>
              <option v-for="doctor in doctors" :key="doctor.id" :value="doctor.id">
                {{ doctor.name }} — {{ doctor.specialization }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="date">Data și ora *</label>
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

          <!-- Info email -->
          <div class="email-notice">
            <span class="notice-icon">📧</span>
            <span>
              După trimitere vei primi un email de confirmare la
              <strong>{{ clientEmail || 'adresa ta' }}</strong>.
            </span>
          </div>

          <div class="form-actions">
            <button type="button" class="secondary-btn" @click="router.push('/dashboard')">
              Anulează
            </button>
            <button type="submit" class="primary-btn" :disabled="saving">
              {{ saving ? 'Se trimite...' : '📅 Trimite programarea' }}
            </button>
          </div>
        </form>
      </template>

    </div>
  </div>
</template>

<style scoped>
.page-wrapper { max-width: 900px; margin: 0 auto; padding: 3rem 2rem; }
.form-card { background: white; border-radius: 24px; padding: 2rem; box-shadow: 0 14px 32px rgba(0,0,0,0.08); }

/* Header */
.page-header { text-align: center; margin-bottom: 2rem; }
.page-header h1 { font-size: 2.1rem; font-weight: 800; color: #1f2937; margin-bottom: 0.75rem; }
.page-header p { color: #6b7280; }

/* Form */
.appointment-form { display: grid; gap: 1.25rem; }
.form-group { display: grid; gap: 0.55rem; }
.form-group label { font-weight: 600; color: #374151; }
.form-group input, .form-group select, .form-group textarea {
  border: 1px solid #d1d5db; border-radius: 14px; padding: 0.9rem 1rem;
  font-size: 1rem; color: #1f2937; background: #fff; resize: vertical;
}
.form-group input:focus, .form-group select:focus, .form-group textarea:focus {
  outline: none; border-color: #ec4899;
  box-shadow: 0 0 0 4px rgba(236,72,153,0.12);
}

/* Email notice */
.email-notice {
  display: flex; align-items: flex-start; gap: 10px;
  background: #f0fdf4; border: 1px solid #bbf7d0;
  border-radius: 14px; padding: 12px 16px;
  font-size: 0.9rem; color: #166534;
}
.notice-icon { font-size: 1.1rem; flex-shrink: 0; margin-top: 1px; }

/* Buttons */
.form-actions { display: flex; justify-content: flex-end; gap: 1rem; flex-wrap: wrap; margin-top: 0.5rem; }
.primary-btn, .secondary-btn { border: none; border-radius: 14px; padding: 0.9rem 1.2rem; font-size: 1rem; cursor: pointer; font-weight: 600; transition: all 0.2s; }
.primary-btn { background: linear-gradient(135deg, #ec4899, #be185d); color: white; box-shadow: 0 6px 16px rgba(236,72,153,0.25); }
.primary-btn:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(236,72,153,0.3); }
.primary-btn:disabled { opacity: 0.7; cursor: not-allowed; transform: none; }
.secondary-btn { background: #f3f4f6; color: #1f2937; }
.secondary-btn:hover { background: #e5e7eb; }

/* Info boxes */
.info-box { border-radius: 16px; padding: 1rem 1.2rem; background: #f9fafb; color: #374151; }
.info-box.error { background: #fee2e2; color: #b91c1c; }

/* SUCCESS SCREEN */
.success-screen { display: flex; flex-direction: column; align-items: center; text-align: center; padding: 1rem 0; gap: 16px; }
.success-icon { font-size: 4rem; animation: popIn 0.5s ease; }
@keyframes popIn { 0%{transform:scale(0.5);opacity:0} 70%{transform:scale(1.1)} 100%{transform:scale(1);opacity:1} }

.success-screen h2 { font-size: 1.7rem; font-weight: 800; color: #1f2937; margin: 0; }
.success-sub { color: #6b7280; font-size: 0.95rem; max-width: 44ch; line-height: 1.6; margin: 0; }
.success-sub strong { color: #be185d; }

.success-summary {
  width: 100%; background: #f9fafb; border-radius: 18px;
  padding: 18px 20px; display: grid; gap: 14px; margin: 8px 0;
  border: 1px solid #f3f4f6; text-align: left;
}
.summary-row { display: flex; align-items: center; gap: 14px; }
.summary-icon { font-size: 1.4rem; flex-shrink: 0; }
.summary-label { font-size: 0.75rem; font-weight: 700; color: #9ca3af; text-transform: uppercase; letter-spacing: 0.05em; }
.summary-value { font-size: 1rem; font-weight: 700; color: #1f2937; margin-top: 2px; }

.success-actions { display: flex; gap: 12px; flex-wrap: wrap; justify-content: center; width: 100%; }
.success-actions .primary-btn { flex: 1; min-width: 200px; }
.success-actions .secondary-btn { flex: 1; min-width: 200px; }

@media (max-width: 768px) {
  .page-wrapper { padding: 2rem 1rem; }
  .form-card { padding: 1.5rem; }
  .page-header h1 { font-size: 1.8rem; }
  .form-actions { flex-direction: column-reverse; }
  .primary-btn, .secondary-btn { width: 100%; }
}
</style>