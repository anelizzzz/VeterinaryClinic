<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  getAllPets,
  deletePet,
  uploadPetImage,
  type PetResponseDto
} from '../../api/services/petService'

const router = useRouter()

const pets = ref<PetResponseDto[]>([])
const loading = ref(false)
const error = ref('')
const successMessage = ref('')

const openDropdownId = ref<number | null>(null)
const uploadingPetId = ref<number | null>(null)
const fileInputs = ref<Record<number, HTMLInputElement | null>>({})

const hasPets = computed(() => pets.value.length > 0)
const API_URL = import.meta.env.VITE_API_BASE_URL

function getPetImage(imageUrl?: string | null) {
  if (!imageUrl) return ''

  if (imageUrl.startsWith('http')) {
    return imageUrl
  }

  return `${API_URL}${imageUrl}`
}
function formatDate(dateString?: string | null) {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('ro-RO')
}

function getStatusText(status: number) {
  switch (status) {
    case 0:
      return 'Activ'
    case 1:
      return 'În tratament'
    case 2:
      return 'Externat'
    default:
      return 'Necunoscut'
  }
}

function toggleDropdown(petId: number) {
  openDropdownId.value = openDropdownId.value === petId ? null : petId
}

function closeDropdown() {
  openDropdownId.value = null
}

function handleClickOutside() {
  closeDropdown()
}

function setFileInputRef(el: HTMLInputElement | null, petId: number) {
  fileInputs.value[petId] = el
}

function editPet(petId: number) {
  closeDropdown()
  router.push(`/pets/edit/${petId}`)
}

async function removePet(petId: number) {
  closeDropdown()
  successMessage.value = ''
  error.value = ''

  const confirmed = window.confirm('Sigur vrei să ștergi acest animal?')
  if (!confirmed) return

  try {
    await deletePet(petId)
    pets.value = pets.value.filter((pet) => pet.id !== petId)
    successMessage.value = 'Animalul a fost șters cu succes.'
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut șterge animalul.'
  }
}

async function triggerChangePhoto(petId: number) {
  closeDropdown()
  await nextTick()
  fileInputs.value[petId]?.click()
}
function goToMedicalRecords(petId: number) {
  router.push({ name: 'client-medical-records', params: { petId } })
}

function goToLabResults(petId: number) {
  router.push({ name: 'client-lab-results', params: { petId } })
}
async function handlePhotoSelected(event: Event, petId: number) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]

  if (!file) return

  if (!file.type.startsWith('image/')) {
    error.value = 'Te rog selectează un fișier imagine valid.'
    successMessage.value = ''
    input.value = ''
    return
  }

  try {
    uploadingPetId.value = petId
    error.value = ''
    successMessage.value = ''

    await uploadPetImage(petId, file)
    await loadPets()

    successMessage.value = 'Poza animalului a fost actualizată cu succes.'
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut actualiza poza animalului.'
  } finally {
    uploadingPetId.value = null
    input.value = ''
  }
}

async function loadPets() {
  try {
    loading.value = true
    error.value = ''
    pets.value = await getAllPets()
  } catch (err) {
    console.error(err)
    error.value = 'Nu am putut încărca animalele.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadPets()
  document.addEventListener('click', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<template>
  <div class="page-wrapper">
    <div class="page-header">
      <h1>Animalele mele</h1>
      <p>Vezi toate animalele asociate contului tău.</p>
      <button class="add-pet-btn" @click="router.push({ name: 'client-create-pet' })">
        + Adaugă animal
      </button>
    </div>

    <div v-if="loading" class="info-box">
      Se încarcă animalele...
    </div>

    <div v-else>
      <div v-if="error" class="info-box error">
        {{ error }}
      </div>

      <div v-if="successMessage" class="info-box success">
        {{ successMessage }}
      </div>

      <div v-if="!hasPets" class="info-box empty-box">
        <div class="empty-icon">🐾</div>
        <p>Nu ai animale înregistrate momentan.</p>
        <button class="add-pet-btn" @click="router.push({ name: 'client-create-pet' })">
          + Adaugă primul tău animal
        </button>
      </div>

      <div v-else class="pet-grid">
        <article
          v-for="pet in pets"
          :key="pet.id"
          class="pet-card"
        >
          <input
            :ref="(el) => setFileInputRef(el as HTMLInputElement | null, pet.id)"
            type="file"
            accept="image/*"
            class="hidden-file-input"
            @change="handlePhotoSelected($event, pet.id)"
          />

          <div class="card-actions" @click.stop>
            <button class="actions-btn" @click.stop="toggleDropdown(pet.id)">
              ⋮
            </button>

            <div
              v-if="openDropdownId === pet.id"
              class="actions-dropdown"
            >
              <button class="dropdown-item" @click="editPet(pet.id)">
                Editează
              </button>

              <button class="dropdown-item" @click="triggerChangePhoto(pet.id)">
                {{ uploadingPetId === pet.id ? 'Se încarcă...' : 'Schimbă poza' }}
              </button>

              <button class="dropdown-item delete" @click="removePet(pet.id)">
                Șterge
              </button>
            </div>
          </div>

          <img
            v-if="pet.imageUrl"
            :src="getPetImage(pet.imageUrl)"
            :alt="pet.name"
            class="pet-image"
          />

          <div v-else class="pet-image placeholder">
            Fără poză
          </div>

          <div class="pet-content">
            <h2>{{ pet.name }}</h2>
            <span class="status-badge">{{ getStatusText(pet.status) }}</span>

            <div class="pet-details">
              <p><strong>Specie:</strong> {{ pet.species }}</p>
              <p><strong>Rasă:</strong> {{ pet.breed }}</p>
              <p><strong>Data nașterii:</strong> {{ formatDate(pet.birthdate) }}</p>
              <p><strong>Vaccinuri:</strong> {{ pet.vaccines || 'Nespecificate' }}</p>
            </div>
          </div>
          <div class="pet-actions">
          <button class="action-btn primary" @click="goToMedicalRecords(pet.id)">
            Vezi fișă medicală
          </button>

          <button class="action-btn secondary" @click="goToLabResults(pet.id)">
          Vezi rezultate laborator
          </button>
          </div>
        </article>
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
  font-size: 2.2rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.page-header p {
  font-size: 1rem;
  color: #6b7280;
}

.info-box {
  max-width: 700px;
  margin: 0 auto 1rem auto;
  background: white;
  border-radius: 18px;
  padding: 1.5rem;
  text-align: center;
  color: #374151;
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.06);
}

.info-box.error {
  background: #fee2e2;
  color: #b91c1c;
}

.info-box.success {
  background: #dcfce7;
  color: #166534;
}

.pet-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
}

.pet-card {
  position: relative;
  background: white;
  border-radius: 22px;
  overflow: visible;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
}

.card-actions {
  position: absolute;
  top: 14px;
  right: 14px;
  z-index: 10;
}

.actions-btn {
  width: 40px;
  height: 40px;
  border: none;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  color: #374151;
  font-size: 22px;
  font-weight: 700;
  cursor: pointer;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
}

.actions-dropdown {
  position: absolute;
  top: 48px;
  right: 0;
  min-width: 180px;
  background: white;
  border-radius: 14px;
  padding: 0.5rem;
  box-shadow: 0 16px 32px rgba(0, 0, 0, 0.14);
  border: 1px solid #f3f4f6;
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.dropdown-item {
  border: none;
  background: transparent;
  text-align: left;
  padding: 0.75rem 0.9rem;
  border-radius: 10px;
  cursor: pointer;
  font-weight: 600;
  color: #374151;
}

.dropdown-item:hover {
  background: #f9fafb;
}

.dropdown-item.delete {
  color: #b91c1c;
}

.dropdown-item.delete:hover {
  background: #fee2e2;
}

.hidden-file-input {
  display: none;
}

.pet-image {
  width: 100%;
  height: 220px;
  object-fit: cover;
  border-top-left-radius: 22px;
  border-top-right-radius: 22px;
}

.pet-image.placeholder {
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f3f4f6;
  color: #6b7280;
  font-weight: 600;
  height: 220px;
  border-top-left-radius: 22px;
  border-top-right-radius: 22px;
}

.pet-content {
  padding: 1.5rem;
}

.pet-content h2 {
  font-size: 1.3rem;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.status-badge {
  display: inline-block;
  margin-bottom: 1rem;
  padding: 0.45rem 0.8rem;
  border-radius: 999px;
  background: #fce7f3;
  color: #be185d;
  font-size: 0.9rem;
  font-weight: 600;
}

.pet-details {
  display: grid;
  gap: 0.65rem;
}

.pet-details p {
  color: #4b5563;
  line-height: 1.5;
}

@media (max-width: 768px) {
  .page-wrapper {
    padding: 2rem 1rem;
  }

  .page-header h1 {
    font-size: 1.9rem;
  }
}
.pet-actions {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
  margin-top: 1rem;
}

.action-btn {
  border: none;
  border-radius: 14px;
  padding: 0.85rem 1.1rem;
  font-size: 0.95rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 8px 18px rgba(0, 0, 0, 0.08);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 190px;
}

.action-btn.primary {
  background: linear-gradient(135deg, #7c3aed, #5b21b6);
  color: white;
}

.action-btn.primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 24px rgba(91, 33, 182, 0.25);
}

.action-btn.primary:active {
  transform: translateY(0);
}

.action-btn.secondary {
  background: linear-gradient(135deg, #0ea5e9, #0284c7);
  color: white;
}

.action-btn.secondary:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 24px rgba(2, 132, 199, 0.25);
}

.action-btn.secondary:active {
  transform: translateY(0);
}

.action-btn:focus-visible {
  outline: 3px solid rgba(124, 58, 237, 0.35);
  outline-offset: 3px;
}

.action-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.add-pet-btn {
  border: none; border-radius: 14px; padding: 12px 24px;
  background: linear-gradient(135deg, #be185d, #9d174d);
  color: white; font-weight: 700; font-size: 1rem;
  cursor: pointer; transition: all 0.2s;
  box-shadow: 0 6px 16px rgba(190,24,93,0.25);
  margin-top: 12px;
}
.add-pet-btn:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(190,24,93,0.3); }

.empty-box { display: flex; flex-direction: column; align-items: center; gap: 12px; }
.empty-icon { font-size: 3rem; }

@media (max-width: 768px) {
  .pet-actions {
    flex-direction: column;
  }

  .action-btn {
    width: 100%;
    min-width: unset;
  }
}
</style>