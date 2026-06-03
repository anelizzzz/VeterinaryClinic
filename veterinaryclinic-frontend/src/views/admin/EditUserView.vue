<template>
  <section class="edit-page">
    <div class="page-header">
      <div>
        <h1>Modifică utilizator</h1>
        <p>Actualizează datele contului selectat.</p>
      </div>

      <button class="back-btn" @click="goBack">
        Înapoi
      </button>
    </div>

    <div v-if="loading" class="state-box">
      Se încarcă datele utilizatorului...
    </div>

    <div v-else-if="error" class="state-box error">
      {{ error }}
    </div>

    <form v-else class="form-card" @submit.prevent="handleSubmit">
      <div v-if="successMessage" class="state-box success">
        {{ successMessage }}
      </div>

      <div class="form-grid">
        <div class="form-group">
          <label for="name">Nume</label>
          <input id="name" v-model="form.name" type="text" placeholder="Introdu numele" />
        </div>

        <div class="form-group">
          <label for="email">Email</label>
          <input id="email" v-model="form.email" type="email" placeholder="Introdu emailul" />
        </div>

        <div class="form-group">
          <label for="phone">Telefon</label>
          <input id="phone" v-model="form.phone" type="text" placeholder="Introdu telefonul" />
        </div>

        <div class="form-group">
          <label for="role">Rol</label>
          <select id="role" v-model.number="form.role">
            <option :value="0">Admin</option>
            <option :value="1">Doctor</option>
            <option :value="2">Client</option>
          </select>
        </div>
      </div>

      <div class="form-actions">
        <button type="button" class="secondary-btn" @click="goBack">
          Anulează
        </button>
        <button type="submit" class="primary-btn" :disabled="saving">
          {{ saving ? 'Se salvează...' : 'Salvează modificările' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  getUserById,
  updateUser,
  type UserAccountDto,
  type UserUpdateDto
} from '../../api/services/accountService'

const route = useRoute()
const router = useRouter()

const userId = Number(route.params.id)

const loading = ref(false)
const saving = ref(false)
const error = ref('')
const successMessage = ref('')

const form = reactive<UserUpdateDto>({
  name: '',
  email: '',
  phone: '',
  role: 2
})

async function loadUser() {
  try {
    loading.value = true
    error.value = ''

    if (!userId || Number.isNaN(userId)) {
      error.value = 'ID utilizator invalid.'
      return
    }

    const data: UserAccountDto = await getUserById(userId)

    form.name = data.name
    form.email = data.email
    form.phone = data.phone
    form.role = data.role
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-au putut încărca datele utilizatorului.'
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  try {
    error.value = ''
    successMessage.value = ''

    if (!form.name.trim()) {
      error.value = 'Numele este obligatoriu.'
      return
    }

    if (!form.email.trim()) {
      error.value = 'Emailul este obligatoriu.'
      return
    }

    saving.value = true

    await updateUser(userId, {
      name: form.name,
      email: form.email,
      phone: form.phone,
      role: form.role
    })

    successMessage.value = 'Utilizatorul a fost actualizat cu succes.'
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-au putut salva modificările.'
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push('/admin/accounts')
}

onMounted(() => {
  loadUser()
})
</script>

<style scoped>
.edit-page {
  max-width: 900px;
  margin: 0 auto;
  padding: 2rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1.5rem;
}

.page-header h1 {
  font-size: 2rem;
  color: #1f2937;
  margin-bottom: 0.4rem;
}

.page-header p {
  color: #6b7280;
}

.form-card {
  background: white;
  border-radius: 22px;
  padding: 1.75rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 1rem;
}

.form-group {
  display: grid;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 700;
  color: #374151;
}

.form-group input,
.form-group select {
  border: 1px solid #d1d5db;
  border-radius: 14px;
  padding: 0.9rem 1rem;
  font-size: 1rem;
  color: #111827;
  background: #fff;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #760f5c;
  box-shadow: 0 0 0 4px rgba(118, 15, 92, 0.12);
}

.form-actions {
  margin-top: 1.5rem;
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.primary-btn,
.secondary-btn,
.back-btn {
  border: none;
  border-radius: 12px;
  padding: 0.85rem 1.2rem;
  font-weight: 700;
  cursor: pointer;
}

.primary-btn {
  background: #760f5c;
  color: white;
}

.secondary-btn,
.back-btn {
  background: #f3f4f6;
  color: #374151;
}

.state-box {
  margin-bottom: 1rem;
  padding: 1rem 1.2rem;
  border-radius: 14px;
  background: #f9fafb;
  color: #374151;
}

.state-box.error {
  background: #fee2e2;
  color: #b91c1c;
}

.state-box.success {
  background: #dcfce7;
  color: #166534;
}

@media (max-width: 768px) {
  .edit-page {
    padding: 1rem;
  }

  .form-card {
    padding: 1.2rem;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>