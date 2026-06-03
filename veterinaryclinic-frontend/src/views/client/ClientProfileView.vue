<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  getClientProfile,
  updateClientProfile
} from '../../api/services/clientService'

interface ClientUpdateDto {
  name: string
  phone: string
  address: string
}

const router = useRouter()

const loading = ref(false)
const saving = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const email = ref('')

const form = reactive<ClientUpdateDto>({
  name: '',
  phone: '',
  address: ''
})

async function loadProfile() {
  try {
    loading.value = true
    errorMessage.value = ''
    successMessage.value = ''

    const data = await getClientProfile()

    form.name = data.name ?? ''
    form.phone = data.phone ?? ''
    form.address = data.address ?? ''
    email.value = data.email ?? ''
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Nu am putut încărca profilul.'
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  try {
    errorMessage.value = ''
    successMessage.value = ''

    if (!form.name.trim()) {
      errorMessage.value = 'Numele este obligatoriu.'
      return
    }

    if (!form.phone.trim()) {
      errorMessage.value = 'Telefonul este obligatoriu.'
      return
    }

    saving.value = true

    await updateClientProfile({
      name: form.name.trim(),
      phone: form.phone.trim(),
      address: form.address.trim()
    })

    successMessage.value = 'Profilul a fost actualizat cu succes.'
  } catch (error) {
    console.error(error)
    errorMessage.value = 'A apărut o eroare la salvarea profilului.'
  } finally {
    saving.value = false
  }
}

function goToCreatePet() {
  router.push({ name: 'client-create-pet' })
}

onMounted(() => {
  loadProfile()
})
</script>

<template>
  <div class="page-wrapper">
    <div class="profile-card">
      <div class="page-header">
        <h1>Profilul meu</h1>
        <p>Actualizează datele personale ale contului tău.</p>
      </div>

      <div v-if="loading" class="info-box">
        Se încarcă datele profilului...
      </div>

      <form v-else class="profile-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="info-box success">
          {{ successMessage }}
        </div>

        <div class="form-group">
          <label for="name">Nume complet</label>
          <input
            id="name"
            v-model="form.name"
            type="text"
            placeholder="Introdu numele complet"
          />
        </div>

        <div class="form-group">
          <label for="email">Email</label>
          <input
            id="email"
            :value="email"
            type="email"
            disabled
          />
        </div>

        <div class="form-group">
          <label for="phone">Telefon</label>
          <input
            id="phone"
            v-model="form.phone"
            type="text"
            placeholder="Introdu numărul de telefon"
          />
        </div>

        <div class="form-group">
          <label for="address">Adresă</label>
          <textarea
            id="address"
            v-model="form.address"
            rows="4"
            placeholder="Introdu adresa"
          />
        </div>

        <div class="actions-row">
          <button type="button" class="secondary-btn" @click="goToCreatePet">
            Adaugă animal
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
.form-group textarea {
  border: 1px solid #d1d5db;
  border-radius: 14px;
  padding: 0.9rem 1rem;
  font-size: 1rem;
  color: #1f2937;
  background: #fff;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #ec4899;
  box-shadow: 0 0 0 4px rgba(236, 72, 153, 0.12);
}

.form-group input:disabled {
  background: #f3f4f6;
  color: #6b7280;
  cursor: not-allowed;
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
  background: #f3e8ff;
  color: #7c3aed;
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

  .profile-card {
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