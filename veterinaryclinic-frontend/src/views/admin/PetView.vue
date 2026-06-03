<template>
  <section class="admin-page">
    <div class="page-header">
      <h1>Pacienți</h1>
      <p>Gestionează animalele înregistrate în platformă.</p>
    </div>

    <p v-if="loading" class="state-message">Se încarcă...</p>
    <p v-else-if="error" class="error-message">{{ error }}</p>

    <div v-else class="table-card">
      <table>
        <thead>
          <tr>
            <th>Nume</th>
            <th>Specie</th>
            <th>Rasă</th>
            <th>Owner</th>
            <th>Acțiuni</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="pet in pets" :key="pet.id">
            <td>{{ pet.name }}</td>
            <td>{{ pet.species }}</td>
            <td>{{ pet.breed }}</td>
            <td>{{ pet.clientName }}</td>
            <td>
              <div class="row-actions">
                <button class="action-btn delete" @click="removePet(pet.id)">
                  Șterge
                </button>
              </div>
            </td>
          </tr>

          <tr v-if="pets.length === 0">
            <td colspan="5" class="empty-row">
              Nu există pacienți disponibili.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllPets, deletePet, type PetResponseDto } from '../../api/services/petService'

const pets = ref<PetResponseDto[]>([])
const loading = ref(false)
const error = ref('')

const loadPets = async (): Promise<void> => {
  try {
    loading.value = true
    error.value = ''
    pets.value = await getAllPets()
  } catch (err: any) {
    console.error(err)
    error.value = 'Nu s-au putut încărca pacienții.'
  } finally {
    loading.value = false
  }
}

const removePet = async (id: number): Promise<void> => {
  const confirmed = window.confirm('Sigur vrei să ștergi acest pacient?')
  if (!confirmed) return

  try {
    error.value = ''
    await deletePet(id)
    pets.value = pets.value.filter(pet => pet.id !== id)
  } catch (err: any) {
    console.error(err)
    error.value = 'Nu s-a putut șterge pacientul.'
  }
}

onMounted(() => {
  loadPets()
})
</script>

<style scoped>
.admin-page {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 1.5rem;
}

.page-header h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  color: #1f2937;
}

.page-header p {
  color: #6b7280;
}

.table-card {
  background: white;
  border-radius: 20px;
  padding: 1.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  text-align: left;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

th {
  color: #374151;
  font-weight: 700;
}

td {
  color: #4b5563;
}

.row-actions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.action-btn {
  border: none;
  border-radius: 10px;
  padding: 8px 12px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s ease;
}

.action-btn.delete {
  background: #fee2e2;
  color: #b91c1c;
}

.action-btn.delete:hover {
  background: #fecaca;
}

.state-message {
  color: #374151;
  font-weight: 500;
}

.error-message {
  color: #dc2626;
  font-weight: 600;
}

.empty-row {
  text-align: center;
  color: #6b7280;
  padding: 1.5rem;
}

@media (max-width: 768px) {
  .admin-page {
    padding: 1rem;
  }

  .table-card {
    padding: 1rem;
  }

  th,
  td {
    padding: 0.8rem;
  }
}
</style>