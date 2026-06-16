<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import {
  getAllAdoptionAnimals,
  createAdoptionAnimal,
  deleteAdoptionAnimal,
  type AdoptionAnimalResponseDto,
  type AdoptionAnimalCreateDto
} from '../../api/services/adoptionService'

const loading = ref(false)
const errorMessage = ref('')
const animals = ref<AdoptionAnimalResponseDto[]>([])

// Modal
const showModal = ref(false)
const saving = ref(false)
const modalError = ref('')
const form = reactive<AdoptionAnimalCreateDto>({
  name: '',
  species: '',
  breed: '',
  age: 0,
  description: '',
  vaccines: '',
  imageUrl: ''
})

function openModal() {
  form.name = ''
  form.species = ''
  form.breed = ''
  form.age = 0
  form.description = ''
  form.vaccines = ''
  form.imageUrl = ''
  modalError.value = ''
  showModal.value = true
}

function closeModal() { showModal.value = false }

async function handleCreate() {
  if (!form.name || !form.species) {
    modalError.value = 'Numele și specia sunt obligatorii.'
    return
  }
  try {
    saving.value = true
    modalError.value = ''
    const created = await createAdoptionAnimal({ ...form })
    animals.value.unshift(created)
    closeModal()
  } catch {
    modalError.value = 'Nu am putut adăuga animalul. Încearcă din nou.'
  } finally {
    saving.value = false
  }
}

async function handleDelete(id: number, name: string) {
  if (!confirm(`Ești sigur că vrei să ștergi "${name}" din lista de adopții?`)) return
  try {
    await deleteAdoptionAnimal(id)
    animals.value = animals.value.filter(a => a.id !== id)
  } catch {
    errorMessage.value = 'Nu am putut șterge animalul.'
  }
}

async function loadAnimals() {
  try {
    loading.value = true
    errorMessage.value = ''
    animals.value = await getAllAdoptionAnimals()
  } catch {
    errorMessage.value = 'Nu am putut încărca animalele pentru adopție.'
  } finally {
    loading.value = false
  }
}

function formatDate(value: string) {
  return new Date(value).toLocaleDateString('ro-RO')
}

function formatStatus(status: number) {
  if (status === 0) return 'Disponibil'
  if (status === 1) return 'Adoptat'
  if (status === 2) return 'Indisponibil'
  return `Status ${status}`
}

function statusClass(status: number) {
  if (status === 0) return 'status-available'
  if (status === 1) return 'status-adopted'
  return 'status-unavailable'
}

onMounted(() => { loadAnimals() })
</script>

<template>
  <div class="page-wrapper">

    <div class="page-header">
      <div class="header-top">
        <div>
          <h1>Animale pentru adopție</h1>
          <p>Administrează animalele disponibile în secțiunea de adopții.</p>
        </div>
        <button class="add-btn" @click="openModal">
          + Adaugă animal
        </button>
      </div>
    </div>

    <div v-if="loading" class="info-box">Se încarcă animalele...</div>
    <div v-else-if="errorMessage" class="info-box error">{{ errorMessage }}</div>

    <div v-else-if="animals.length === 0" class="empty-box">
      <div class="empty-icon">🐾</div>
      <p>Nu există animale disponibile pentru adopție.</p>
      <button class="add-btn" @click="openModal">+ Adaugă primul animal</button>
    </div>

    <div v-else class="animals-grid">
      <div v-for="animal in animals" :key="animal.id" class="animal-card">
        <div class="animal-img-wrap">
          <img
            v-if="animal.imageUrl"
            :src="animal.imageUrl"
            :alt="animal.name"
            class="animal-image"
          />
          <div v-else class="animal-img-placeholder">🐾</div>
          <span :class="['animal-status', statusClass(animal.status)]">
            {{ formatStatus(animal.status) }}
          </span>
        </div>

        <div class="animal-body">
          <div class="animal-top">
            <h2>{{ animal.name }}</h2>
            <button class="delete-btn" @click="handleDelete(animal.id, animal.name)" title="Șterge">🗑️</button>
          </div>

          <div class="animal-chips">
            <span class="chip">{{ animal.species }}</span>
            <span v-if="animal.breed" class="chip">{{ animal.breed }}</span>
            <span class="chip">{{ animal.age }} ani</span>
          </div>

          <p v-if="animal.description" class="description">{{ animal.description }}</p>

          <div class="animal-meta">
            <span v-if="animal.vaccines">💉 {{ animal.vaccines }}</span>
            <span>📅 {{ formatDate(animal.addedDate) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL ADĂUGARE -->
    <Teleport to="body">
      <div v-if="showModal" class="modal-backdrop" @click.self="closeModal">
        <div class="modal-card">
          <div class="modal-header">
            <h3>🐾 Adaugă animal pentru adopție</h3>
            <button class="close-btn" @click="closeModal">×</button>
          </div>

          <div class="modal-body">
            <div v-if="modalError" class="alert-error">{{ modalError }}</div>

            <div class="form-grid">
              <div class="form-group">
                <label>Nume *</label>
                <input v-model="form.name" type="text" placeholder="Ex: Max" />
              </div>

              <div class="form-group">
                <label>Specie *</label>
                <input v-model="form.species" type="text" placeholder="Ex: Câine, Pisică" />
              </div>

              <div class="form-group">
                <label>Rasă</label>
                <input v-model="form.breed" type="text" placeholder="Ex: Labrador" />
              </div>

              <div class="form-group">
                <label>Vârstă (ani)</label>
                <input v-model.number="form.age" type="number" min="0" max="30" />
              </div>

              <div class="form-group full">
                <label>Vaccinuri</label>
                <input v-model="form.vaccines" type="text" placeholder="Ex: Antirabic, DHPP" />
              </div>

              <div class="form-group full">
                <label>URL imagine</label>
                <input v-model="form.imageUrl" type="text" placeholder="https://..." />
              </div>

              <div class="form-group full">
                <label>Descriere</label>
                <textarea v-model="form.description" rows="3" placeholder="Scrie câteva detalii despre animal..." />
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button class="cancel-btn" @click="closeModal">Anulează</button>
            <button class="save-btn" :disabled="saving" @click="handleCreate">
              {{ saving ? 'Se salvează...' : '+ Adaugă animal' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

  </div>
</template>

<style scoped>
.page-wrapper { max-width: 1200px; margin: 0 auto; padding: 3rem 2rem; }

.page-header { margin-bottom: 2rem; }
.header-top { display: flex; justify-content: space-between; align-items: flex-start; gap: 1rem; flex-wrap: wrap; }
.header-top h1 { font-size: 2.2rem; font-weight: 800; color: #1f2937; margin-bottom: 0.5rem; }
.header-top p { color: #6b7280; }

.add-btn {
  border: none; border-radius: 14px; padding: 12px 22px;
  background: linear-gradient(135deg, #be185d, #9d174d);
  color: white; font-weight: 700; font-size: 1rem;
  cursor: pointer; transition: all 0.2s; white-space: nowrap;
  box-shadow: 0 6px 16px rgba(190,24,93,0.25);
}
.add-btn:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(190,24,93,0.3); }

.info-box { border-radius: 16px; padding: 1rem 1.2rem; background: #f9fafb; color: #374151; }
.info-box.error { background: #fee2e2; color: #b91c1c; }

.empty-box { text-align: center; padding: 4rem 2rem; display: flex; flex-direction: column; align-items: center; gap: 14px; }
.empty-icon { font-size: 3.5rem; }
.empty-box p { color: #6b7280; font-size: 1rem; }

.animals-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 1.5rem; }

.animal-card { background: white; border-radius: 24px; overflow: hidden; box-shadow: 0 8px 24px rgba(0,0,0,0.08); transition: transform 0.2s; }
.animal-card:hover { transform: translateY(-4px); }

.animal-img-wrap { position: relative; }
.animal-image { width: 100%; height: 200px; object-fit: cover; display: block; }
.animal-img-placeholder { width: 100%; height: 200px; background: #f3f4f6; display: flex; align-items: center; justify-content: center; font-size: 4rem; }
.animal-status { position: absolute; top: 10px; right: 10px; border-radius: 999px; padding: 4px 12px; font-size: 0.8rem; font-weight: 700; }
.status-available { background: #dcfce7; color: #166534; }
.status-adopted { background: #dbeafe; color: #1d4ed8; }
.status-unavailable { background: #fee2e2; color: #b91c1c; }

.animal-body { padding: 1.2rem; }
.animal-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
.animal-top h2 { font-size: 1.2rem; font-weight: 800; color: #1f2937; }
.delete-btn { border: none; background: #fee2e2; border-radius: 8px; padding: 6px 8px; cursor: pointer; font-size: 0.9rem; transition: background 0.2s; }
.delete-btn:hover { background: #fecaca; }

.animal-chips { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 10px; }
.chip { background: #f3f4f6; border-radius: 999px; padding: 3px 10px; font-size: 0.8rem; color: #374151; font-weight: 600; }

.description { color: #4b5563; font-size: 0.9rem; line-height: 1.5; margin-bottom: 10px; }
.animal-meta { display: flex; flex-direction: column; gap: 4px; font-size: 0.82rem; color: #6b7280; }

/* Modal */
.modal-backdrop { position: fixed; inset: 0; background: rgba(0,0,0,0.45); display: flex; align-items: center; justify-content: center; padding: 1rem; z-index: 1000; }
.modal-card { width: min(600px, 100%); background: white; border-radius: 24px; box-shadow: 0 20px 60px rgba(0,0,0,0.2); overflow: hidden; max-height: 90vh; display: flex; flex-direction: column; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 1.2rem 1.5rem; border-bottom: 1px solid #f3f4f6; flex-shrink: 0; }
.modal-header h3 { margin: 0; font-size: 1.2rem; font-weight: 800; color: #1f2937; }
.close-btn { border: none; background: transparent; font-size: 1.6rem; cursor: pointer; color: #6b7280; line-height: 1; }

.modal-body { padding: 1.5rem; overflow-y: auto; }
.alert-error { background: #fee2e2; color: #b91c1c; border-radius: 12px; padding: 10px 14px; margin-bottom: 14px; font-size: 0.9rem; }

.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; }
.form-group { display: grid; gap: 6px; }
.form-group.full { grid-column: 1 / -1; }
.form-group label { font-size: 0.85rem; font-weight: 700; color: #374151; }
.form-group input, .form-group textarea {
  border: 1px solid #d1d5db; border-radius: 12px;
  padding: 10px 14px; font-size: 0.95rem; color: #1f2937; resize: vertical;
}
.form-group input:focus, .form-group textarea:focus {
  outline: none; border-color: #be185d;
  box-shadow: 0 0 0 3px rgba(190,24,93,0.12);
}

.modal-footer { display: flex; justify-content: flex-end; gap: 10px; padding: 1rem 1.5rem; border-top: 1px solid #f3f4f6; flex-shrink: 0; }
.cancel-btn { border: 1px solid #d1d5db; border-radius: 12px; padding: 10px 18px; background: white; color: #374151; font-weight: 600; cursor: pointer; }
.cancel-btn:hover { background: #f3f4f6; }
.save-btn { border: none; border-radius: 12px; padding: 10px 20px; background: linear-gradient(135deg, #be185d, #9d174d); color: white; font-weight: 700; cursor: pointer; transition: all 0.2s; }
.save-btn:hover { transform: translateY(-1px); }
.save-btn:disabled { opacity: 0.7; cursor: not-allowed; transform: none; }

@media (max-width: 768px) {
  .page-wrapper { padding: 2rem 1rem; }
  .form-grid { grid-template-columns: 1fr; }
  .form-group.full { grid-column: 1; }
}
</style>