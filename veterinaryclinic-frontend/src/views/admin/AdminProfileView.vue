<script setup lang="ts">
import { onMounted, reactive, ref, computed } from 'vue'
import {
  getMyProfile,
  updateMyProfile,
  type UserAccountDto,
  type UserUpdateDto
} from '../../api/services/accountService'
import { UserRole } from '../../types/auth'

const loading = ref(false)
const saving = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const form = reactive<UserUpdateDto>({
  name: '',
  email: '',
  phone: '',
  role: UserRole.Admin
})

const roleText = computed(() => {
  if (form.role === UserRole.Admin) return 'Admin'
  if (form.role === UserRole.Doctor) return 'Doctor'
  return 'Client'
})

const isSubmitDisabled = computed(() => {
  return saving.value || !form.name.trim() || !form.phone.trim()
})

async function loadProfile() {
  try {
    loading.value = true
    errorMessage.value = ''
    successMessage.value = ''

    const data: UserAccountDto = await getMyProfile()

    form.name = data.name ?? ''
    form.email = data.email ?? ''
    form.phone = data.phone ?? ''
    form.role = data.role
  } catch (error) {
    console.error('loadProfile error:', error)
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

    const updated = await updateMyProfile({
      name: form.name.trim(),
      email: form.email,
      phone: form.phone.trim(),
      role: form.role
    })

    form.name = updated.name ?? ''
    form.email = updated.email ?? ''
    form.phone = updated.phone ?? ''
    form.role = updated.role

    const rawUser = localStorage.getItem('user')
    if (rawUser) {
      const parsedUser = JSON.parse(rawUser)
      parsedUser.name = updated.name
      parsedUser.email = updated.email
      parsedUser.role = updated.role
      localStorage.setItem('user', JSON.stringify(parsedUser))
    }

    successMessage.value = 'Profilul a fost actualizat cu succes.'
  } catch (error) {
    console.error('handleSubmit error:', error)
    errorMessage.value = 'A apărut o eroare la salvarea profilului.'
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadProfile()
})
</script>

<template>
  <section class="profile-page">
    <div class="profile-card">
      <div class="page-header">
        <h1>Profilul meu</h1>
        <p>Actualizează datele personale ale contului tău de administrator.</p>
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
            autocomplete="name"
          />
        </div>

        <div class="form-group">
          <label for="email">Email</label>
          <input
            id="email"
            :value="form.email"
            type="email"
            disabled
            autocomplete="email"
          />
        </div>

        <div class="form-group">
          <label for="phone">Telefon</label>
          <input
            id="phone"
            v-model="form.phone"
            type="text"
            placeholder="Introdu numărul de telefon"
            autocomplete="tel"
          />
        </div>

        <div class="form-group">
          <label for="role">Rol</label>
          <input
            id="role"
            :value="roleText"
            type="text"
            disabled
          />
        </div>

        <div class="form-actions">
          <button type="submit" class="primary-btn" :disabled="isSubmitDisabled">
            {{ saving ? 'Se salvează...' : 'Salvează modificările' }}
          </button>
        </div>
      </form>
    </div>
  </section>
</template>

<style scoped>
.profile-page {
  max-width: 900px;
  margin: 0 auto;
  padding: 3rem 1.5rem;
}

.profile-card {
  background: #ffffff;
  border-radius: 24px;
  padding: 2rem;
  box-shadow: 0 14px 30px rgba(15, 23, 42, 0.08);
  border: 1px solid #f1e4ec;
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  margin: 0 0 0.75rem;
  font-size: 2.1rem;
  font-weight: 800;
  color: #1f2937;
}

.page-header p {
  margin: 0;
  color: #6b7280;
  font-size: 1rem;
}

.profile-form {
  display: grid;
  gap: 1.2rem;
}

.form-group {
  display: grid;
  gap: 0.55rem;
}

.form-group label {
  font-weight: 600;
  color: #374151;
}

.form-group input {
  width: 100%;
  border: 1px solid #d1d5db;
  border-radius: 14px;
  padding: 0.95rem 1rem;
  font-size: 1rem;
  color: #111827;
  background: #ffffff;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.form-group input:focus {
  outline: none;
  border-color: #8b145f;
  box-shadow: 0 0 0 4px rgba(139, 20, 95, 0.12);
}

.form-group input:disabled {
  background: #f3f4f6;
  color: #6b7280;
  cursor: not-allowed;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 0.25rem;
}

.primary-btn {
  border: none;
  border-radius: 14px;
  background: linear-gradient(135deg, #8b145f, #b11f76);
  color: white;
  padding: 0.95rem 1.3rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 10px 24px rgba(139, 20, 95, 0.22);
  transition: transform 0.2s ease, box-shadow 0.2s ease, opacity 0.2s ease;
}

.primary-btn:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 12px 28px rgba(139, 20, 95, 0.26);
}

.primary-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  box-shadow: none;
}

.info-box {
  border-radius: 16px;
  padding: 1rem 1.1rem;
  font-size: 0.96rem;
}

.info-box.error {
  background: #fee2e2;
  color: #b91c1c;
  border: 1px solid #fecaca;
}

.info-box.success {
  background: #dcfce7;
  color: #166534;
  border: 1px solid #bbf7d0;
}

@media (max-width: 768px) {
  .profile-page {
    padding: 2rem 1rem;
  }

  .profile-card {
    padding: 1.4rem;
    border-radius: 20px;
  }

  .page-header h1 {
    font-size: 1.8rem;
  }

  .form-actions {
    justify-content: stretch;
  }

  .primary-btn {
    width: 100%;
  }
}
</style>