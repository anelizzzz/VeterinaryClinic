<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  getPetById,
  updatePet,
  type PetResponseDto,
  type PetUpdateDto
} from '../../api/services/petService'

const route = useRoute()
const router = useRouter()

const petId = Number(route.params.id)

const loading = ref(false)
const saving = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const imageUrl = ref('')

const form = reactive<PetUpdateDto>({
  name: '',
  species: '',
  breed: '',
  birthdate: '',
  vaccines: '',
  status: 0,
  imageUrl: ''
})

function formatDateForInput(dateString: string | null) {
  if (!dateString) return ''
  return new Date(dateString).toISOString().split('T')[0]
}

async function loadPet() {
  try {
    loading.value = true
    errorMessage.value = ''
    successMessage.value = ''

    const data: PetResponseDto = await getPetById(petId)

    form.name = data.name ?? ''
    form.species = data.species ?? ''
    form.breed = data.breed ?? ''
    form.birthdate = formatDateForInput(data.birthdate)
    form.vaccines = data.vaccines ?? ''
    form.status = data.status
    form.imageUrl = data.imageUrl ?? ''

    imageUrl.value = data.imageUrl ?? ''
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Nu am putut încărca datele animalului.'
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  try {
    errorMessage.value = ''
    successMessage.value = ''

    if (!form.name.trim()) {
      errorMessage.value = 'Numele animalului este obligatoriu.'
      return
    }

    if (!form.species.trim()) {
      errorMessage.value = 'Specia este obligatorie.'
      return
    }

    if (!form.breed.trim()) {
      errorMessage.value = 'Rasa este obligatorie.'
      return
    }

    saving.value = true

    await updatePet(petId, {
      name: form.name.trim(),
      species: form.species.trim(),
      breed: form.breed.trim(),
      birthdate: form.birthdate || null,
      vaccines: form.vaccines.trim(),
      status: form.status,
      imageUrl: form.imageUrl.trim()
    })

    imageUrl.value = form.imageUrl
    successMessage.value = 'Informațiile animalului au fost actualizate cu succes.'
  } catch (error) {
    console.error(error)
    errorMessage.value = 'A apărut o eroare la salvarea modificărilor.'
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push('/my-pets')
}

onMounted(() => {
  if (!petId || Number.isNaN(petId)) {
    errorMessage.value = 'ID-ul animalului este invalid.'
    return
  }

  loadPet()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Editează animalul</h1>
        <p>Actualizează informațiile despre animalul tău.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă datele animalului...
      </div>

      <form v-else class="profile-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="info-box success">
          {{ successMessage }}
        </div>

        <div v-if="imageUrl" class="image-preview-wrapper">
          <img :src="imageUrl" alt="Imagine animal" class="image-preview" />
        </div>

        <div class="form-group">
          <label for="name">Nume</label>
          <input
            id="name"
            v-model="form.name"
            type="text"
            placeholder="Introdu numele animalului"
          />
        </div>

        <div class="form-group">
          <label for="species">Specie</label>
          <input
            id="species"
            v-model="form.species"
            type="text"
            placeholder="Ex: câine, pisică"
          />
        </div>

        <div class="form-group">
          <label for="breed">Rasă</label>
          <input
            id="breed"
            v-model="form.breed"
            type="text"
            placeholder="Introdu rasa"
          />
        </div>

        <div class="form-group">
          <label for="birthdate">Data nașterii</label>
          <input
            id="birthdate"
            v-model="form.birthdate"
            type="date"
          />
        </div>

        <div class="form-group">
          <label for="vaccines">Vaccinuri</label>
          <textarea
            id="vaccines"
            v-model="form.vaccines"
            rows="4"
            placeholder="Introdu vaccinurile sau observațiile medicale"
          />
        </div>

        <div class="form-group">
          <label for="imageUrl">Imagine URL</label>
          <input
            id="imageUrl"
            v-model="form.imageUrl"
            type="text"
            placeholder="https://exemplu.com/imagine.jpg"
          />
        </div>

        <div class="form-group">
          <label for="status">Status</label>
          <select id="status" v-model.number="form.status">
            <option :value="0">Activ</option>
            <option :value="1">În tratament</option>
            <option :value="2">Externat</option>
          </select>
        </div>

        <div class="form-actions">
          <button type="button" class="secondary-btn" @click="goBack">
            Înapoi
          </button>
          <button type="submit" class="primary-btn" :disabled="saving">
            {{ saving ? 'Se salvează...' : 'Salvează modificările' }}
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
.form-group textarea,
.form-group select {
  border: 1px solid #d1d5db;
  border-radius: 14px;
  padding: 0.9rem 1rem;
  font-size: 1rem;
  color: #1f2937;
  background: #fff;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
  border-color: #ec4899;
  box-shadow: 0 0 0 4px rgba(236, 72, 153, 0.12);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 0.5rem;
  flex-wrap: wrap;
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

.secondary-btn {
  background: #f3f4f6;
  color: #1f2937;
}

.primary-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
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

.image-preview-wrapper {
  display: flex;
  justify-content: center;
}

.image-preview {
  width: 180px;
  height: 180px;
  object-fit: cover;
  border-radius: 18px;
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.08);
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
}
</style>