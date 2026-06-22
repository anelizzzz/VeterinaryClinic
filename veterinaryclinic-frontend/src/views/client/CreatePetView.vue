<script setup lang="ts">
import { reactive, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { createPet } from '../../api/services/petService'
import { getClientProfile } from '../../api/services/clientService'

interface PetCreateForm {
  name: string
  species: string
  breed: string
  birthdate: string
  vaccines: string
  imageUrl: string
}

const router = useRouter()

const saving = ref(false)
const clientId = ref<number | null>(null)
const successMessage = ref('')
const errorMessage = ref('')

const form = reactive<PetCreateForm>({
  name: '',
  species: '',
  breed: '',
  birthdate: '',
  vaccines: '',
  imageUrl: ''
})

async function loadClientId() {
  try {
    const profile = await getClientProfile()
    clientId.value = profile.id
  } catch (err) {
    console.error('Could not load client profile', err)
    errorMessage.value = 'Nu am putut încărca profilul de client.'
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

    await createPet({
      name: form.name.trim(),
      species: form.species.trim(),
      breed: form.breed.trim(),
      birthdate: form.birthdate || null,
      vaccines: form.vaccines.trim(),
      imageUrl: form.imageUrl.trim(),
      clientId: clientId.value ?? undefined
    })

    successMessage.value = 'Animalul a fost adăugat cu succes.'

    setTimeout(() => {
      router.push({ name: 'client-profile' })
    }, 800)
  } catch (error) {
    console.error(error)
    errorMessage.value = 'A apărut o eroare la adăugarea animalului.'
  } finally {
    saving.value = false
  }
}

onMounted(() => { loadClientId() })

function goBack() {
  router.push({ name: 'client-profile' })
}
</script>

<template>
  <div class="page-wrapper">
    <div class="pet-card">
      <div class="page-header">
        <h1>Adaugă animal</h1>
        <p>Creează un nou animal asociat contului tău de client.</p>
      </div>

      <form class="pet-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="info-box success">
          {{ successMessage }}
        </div>

        <div class="form-group">
          <label for="name">Nume</label>
          <input
            id="name"
            v-model="form.name"
            type="text"
            placeholder="Numele animalului"
          />
        </div>

        <div class="form-group">
          <label for="species">Specie</label>
          <input
            id="species"
            v-model="form.species"
            type="text"
            placeholder="Ex: Câine, Pisică"
          />
        </div>

        <div class="form-group">
          <label for="breed">Rasă</label>
          <input
            id="breed"
            v-model="form.breed"
            type="text"
            placeholder="Rasa animalului"
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
            placeholder="Introdu vaccinurile sau istoricul medical"
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

        <div class="actions-row">
          <button type="button" class="secondary-btn" @click="goBack">
            Înapoi
          </button>

          <button type="submit" class="primary-btn" :disabled="saving">
            {{ saving ? 'Se salvează...' : 'Salvează animalul' }}
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

.pet-card {
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

.pet-form {
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

.actions-row {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
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

.secondary-btn {
  background: #f3f4f6;
  color: #374151;
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

@media (max-width: 768px) {
  .page-wrapper {
    padding: 2rem 1rem;
  }

  .pet-card {
    padding: 1.5rem;
  }

  .page-header h1 {
    font-size: 1.8rem;
  }

  .actions-row {
    flex-direction: column-reverse;
  }

  .primary-btn,
  .secondary-btn {
    width: 100%;
  }
}
</style>