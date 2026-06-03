<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { createAdoptionRequest, getAllAdoptionAnimals, type AdoptionAnimalResponseDto } from '../../api/services/adoptionService'

const authStore = useAuthStore()

const loading = ref(false)
const sendingId = ref<number | null>(null)
const errorMessage = ref('')
const successMessage = ref('')
const animals = ref<AdoptionAnimalResponseDto[]>([])

async function loadAnimals() {
  try {
    loading.value = true
    errorMessage.value = ''
    animals.value = await getAllAdoptionAnimals()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Nu am putut încărca animalele disponibile pentru adopție.'
  } finally {
    loading.value = false
  }
}

async function adopt(animalId: number) {
  try {
    errorMessage.value = ''
    successMessage.value = ''

    const clientId = authStore.user?.id
    if (!clientId) {
      errorMessage.value = 'Nu s-a putut identifica utilizatorul autentificat.'
      return
    }

    const motivation = window.prompt('Scrie pe scurt motivul pentru care vrei să adopți acest animal:')

    if (!motivation || !motivation.trim()) {
      errorMessage.value = 'Motivația este obligatorie.'
      return
    }

    sendingId.value = animalId

    await createAdoptionRequest({
      adoptionAnimalId: animalId,
      motivation: motivation.trim()
    })

    successMessage.value = 'Cererea de adopție a fost trimisă cu succes.'
  } catch (error) {
    console.error(error)
    errorMessage.value = 'A apărut o eroare la trimiterea cererii.'
  } finally {
    sendingId.value = null
  }
}

onMounted(() => {
  authStore.loadUser()
  loadAnimals()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="page-header">
      <h1>Adoptă un animal</h1>
      <p>Animale disponibile pentru adopție în cadrul VetCare.</p>
    </div>

    <div v-if="loading" class="info-box">
      Se încarcă animalele...
    </div>

    <div v-if="errorMessage" class="info-box error">
      {{ errorMessage }}
    </div>

    <div v-if="successMessage" class="info-box success">
      {{ successMessage }}
    </div>

    <div v-if="!loading && animals.length === 0" class="info-box">
      Momentan nu există animale disponibile pentru adopție.
    </div>

    <div v-else class="animals-grid">
      <div
        v-for="animal in animals"
        :key="animal.id"
        class="animal-card"
      >
        <img
          :src="animal.imageUrl"
          :alt="animal.name"
          class="animal-image"
        />

        <div class="animal-body">
          <div class="animal-top">
            <h2>{{ animal.name }}</h2>
            <span class="animal-status">{{ animal.status }}</span>
          </div>

          <p><strong>Specie:</strong> {{ animal.species }}</p>
          <p><strong>Rasă:</strong> {{ animal.breed }}</p>
          <p><strong>Vârstă:</strong> {{ animal.age }} ani</p>
          <p><strong>Vaccinuri:</strong> {{ animal.vaccines }}</p>
          <p class="description">{{ animal.description }}</p>

          <button
            class="adopt-btn"
            :disabled="sendingId === animal.id"
            @click="adopt(animal.id)"
          >
            {{ sendingId === animal.id ? 'Se trimite...' : 'Trimite cerere de adopție' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper {
  max-width: 1200px;
  margin: 0 auto;
  padding: 3rem 2rem;
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.4rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.page-header p {
  color: #6b7280;
}

.animals-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
}

.animal-card {
  background: white;
  border-radius: 24px;
  overflow: hidden;
  box-shadow: 0 14px 32px rgba(0, 0, 0, 0.08);
}

.animal-image {
  width: 100%;
  height: 220px;
  object-fit: cover;
}

.animal-body {
  padding: 1.25rem;
}

.animal-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.animal-top h2 {
  font-size: 1.35rem;
  font-weight: 800;
  color: #1f2937;
}

.animal-status {
  background: #fce7f3;
  color: #be185d;
  border-radius: 999px;
  padding: 0.35rem 0.8rem;
  font-size: 0.9rem;
  font-weight: 700;
}

.description {
  margin: 1rem 0;
  color: #4b5563;
}

.adopt-btn {
  width: 100%;
  border: none;
  border-radius: 14px;
  padding: 0.95rem 1rem;
  background: #ec4899;
  color: white;
  font-weight: 700;
  cursor: pointer;
}

.adopt-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.info-box {
  margin-bottom: 1rem;
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
</style>