<template>
  <div class="page-wrapper">
    <div class="page-header">
      <h1>Animale pentru adopție</h1>
      <p>Administrează animalele disponibile în secțiunea de adopții.</p>
    </div>

    <div v-if="loading" class="info-box">
      Se încarcă animalele...
    </div>

    <div v-else-if="errorMessage" class="info-box error">
      {{ errorMessage }}
    </div>

    <div v-else-if="animals.length === 0" class="info-box">
      Nu există animale disponibile pentru adopție.
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
            <span class="animal-status">
              {{ formatAnimalStatus(animal.status) }}
            </span>
          </div>

          <p><strong>Specie:</strong> {{ animal.species }}</p>
          <p><strong>Rasă:</strong> {{ animal.breed }}</p>
          <p><strong>Vârstă:</strong> {{ animal.age }} ani</p>
          <p><strong>Vaccinuri:</strong> {{ animal.vaccines }}</p>
          <p><strong>Adăugat la:</strong> {{ formatDate(animal.addedDate) }}</p>
          <p class="description">{{ animal.description }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import {
  getAllAdoptionAnimals,
  type AdoptionAnimalResponseDto
} from '../../api/services/adoptionService'

const loading = ref(false)
const errorMessage = ref('')
const animals = ref<AdoptionAnimalResponseDto[]>([])

async function loadAnimals() {
  try {
    loading.value = true
    errorMessage.value = ''
    animals.value = await getAllAdoptionAnimals()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Nu am putut încărca animalele pentru adopție.'
  } finally {
    loading.value = false
  }
}

function formatDate(value: string): string {
  return new Date(value).toLocaleString('ro-RO')
}

function formatAnimalStatus(status: number): string {
  if (status === 0) return 'Disponibil'
  if (status === 1) return 'Adoptat'
  if (status === 2) return 'Indisponibil'
  return `Status ${status}`
}

onMounted(() => {
  loadAnimals()
})
</script>

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
  margin-top: 1rem;
  color: #4b5563;
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

@media (max-width: 768px) {
  .page-wrapper {
    padding: 2rem 1rem;
  }
}
</style>