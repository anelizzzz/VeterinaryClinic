<template>
  <section class="register-wrapper">
    <div class="register-card">
      <h1>Register</h1>
      <p>Creează un cont nou pentru platforma clinicii veterinare.</p>

      <form class="form" @submit.prevent="handleRegister">
        <input v-model="form.name" type="text" placeholder="Nume complet" />
        <input v-model="form.email" type="email" placeholder="Email" />
        <input v-model="form.password" type="password" placeholder="Parolă" />
        <input v-model="form.phone" type="text" placeholder="Telefon" />

        <div class="role-group">
          <label class="role-option">
            <input v-model="form.role" type="radio" :value="UserRole.Client" />
            <span>Client</span>
          </label>

          <label class="role-option">
            <input v-model="form.role" type="radio" :value="UserRole.Doctor" />
            <span>Doctor</span>
          </label>
        </div>

        <button type="submit">Register</button>

        <p v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </p>
      </form>
    </div>
  </section>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios'
import { UserRole, type RegisterDto } from '../types/auth'

const router = useRouter()
const errorMessage = ref('')

const form = reactive<RegisterDto>({
  name: '',
  email: '',
  password: '',
  phone: '',
  role: UserRole.Client
})

const handleRegister = async (): Promise<void> => {
  try {
    errorMessage.value = ''

    await api.post('/Auth/register', form)

    router.push('/login')
  } catch (error: any) {
    errorMessage.value =
      error?.response?.data?.message ||
      'A apărut o eroare la înregistrare.'
  }
}
</script>

<style scoped>
.register-wrapper {
  min-height: calc(100vh - 80px);
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 30px;
}

.register-card {
  width: 100%;
  max-width: 520px;
  background: white;
  border-radius: 20px;
  padding: 36px;
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.08);
}

.register-card h1 {
  font-size: 32px;
  margin-bottom: 10px;
}

.register-card p {
  color: #760f5c;
  margin-bottom: 24px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

input[type='text'],
input[type='email'],
input[type='password'] {
  padding: 14px 16px;
  border-radius: 12px;
  border: 1px solid #d1d5db;
  font-size: 16px;
}

.role-group {
  display: flex;
  gap: 16px;
  margin-top: 4px;
}

.role-option {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 14px;
  border: 1px solid #d1d5db;
  border-radius: 12px;
  cursor: pointer;
}

.role-option input {
  margin: 0;
}

button {
  border: none;
  background: #760f5c;
  color: white;
  padding: 14px;
  border-radius: 12px;
  font-weight: 700;
  cursor: pointer;
}

.error-message {
  color: #dc2626;
  margin-top: 8px;
}
</style>