<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios'
import { UserRole, type RegisterDto } from '../types/auth'

const router = useRouter()
const errorMessage = ref('')
const loading = ref(false)
const isPending = ref(false)
const pendingEmail = ref('')

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
    loading.value = true

    const response = await api.post('/Auth/register', form)
    const data = response.data

    if (data.isPending) {
      pendingEmail.value = form.email
      isPending.value = true
    } else {
      router.push('/login')
    }
  } catch (error: any) {
    errorMessage.value =
      error?.response?.data ||
      error?.response?.data?.message ||
      'A apărut o eroare la înregistrare.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <section class="register-wrapper">

    <!-- ECRAN AȘTEPTARE -->
    <div v-if="isPending" class="register-card pending-card">
      <div class="pending-icon">⏳</div>
      <h1>Cerere trimisă!</h1>
      <p class="pending-title">Contul tău urmează să fie aprobat</p>

      <div class="pending-box">
        <p>
          Cererea ta de creare cont a fost înregistrată cu succes.
          Administratorul clinicii va analiza cererea și vei primi un
          <strong>email de confirmare</strong> la adresa:
        </p>
        <div class="pending-email">📧 {{ pendingEmail }}</div>
        <p class="pending-hint">
          De obicei procesul durează câteva ore. Verifică și folderul <strong>Spam</strong>
          dacă nu primești emailul.
        </p>
      </div>

      <button class="btn-login" @click="router.push('/login')">
        Mergi la pagina de login
      </button>
    </div>

    <!-- FORMULAR ÎNREGISTRARE -->
    <div v-else class="register-card">
      <h1>Creează cont</h1>
      <p>Înregistrează-te pe platforma clinicii veterinare VetCare.</p>

      <form class="form" @submit.prevent="handleRegister">
        <div class="form-group">
          <label>Nume complet</label>
          <input v-model="form.name" type="text" placeholder="Ex: Maria Popescu" required />
        </div>

        <div class="form-group">
          <label>Email</label>
          <input v-model="form.email" type="email" placeholder="exemplu@email.com" required />
        </div>

        <div class="form-group">
          <label>Parolă</label>
          <input v-model="form.password" type="password" placeholder="Minim 6 caractere" required />
        </div>

        <div class="form-group">
          <label>Telefon</label>
          <input v-model="form.phone" type="text" placeholder="07XX XXX XXX" />
        </div>

        <div class="form-group">
          <label>Tip cont</label>
          <div class="role-group">
            <label class="role-option" :class="{ active: form.role === UserRole.Client }">
              <input v-model="form.role" type="radio" :value="UserRole.Client" />
              <span class="role-icon">👤</span>
              <span>Client</span>
            </label>
            <label class="role-option" :class="{ active: form.role === UserRole.Doctor }">
              <input v-model="form.role" type="radio" :value="UserRole.Doctor" />
              <span class="role-icon">🩺</span>
              <span>Doctor</span>
            </label>
          </div>
        </div>

        <div class="info-notice">
          ℹ️ Contul va fi activat după aprobarea administratorului clinicii. Vei primi un email de confirmare.
        </div>

        <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

        <button type="submit" :disabled="loading">
          {{ loading ? 'Se trimite cererea...' : 'Trimite cererea de cont' }}
        </button>

        <p class="login-link">
          Ai deja cont?
          <router-link to="/login">Autentifică-te</router-link>
        </p>
      </form>
    </div>

  </section>
</template>

<style scoped>
.register-wrapper {
  min-height: calc(100vh - 80px);
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 30px;
  background: linear-gradient(160deg, #fff0f8 0%, #ffffff 50%, #f0f9ff 100%);
}

.register-card {
  width: 100%;
  max-width: 520px;
  background: white;
  border-radius: 24px;
  padding: 40px;
  box-shadow: 0 20px 50px rgba(0,0,0,0.1);
}

.register-card h1 {
  font-size: 2rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 8px;
}

.register-card > p {
  color: #6b7280;
  margin-bottom: 28px;
}

/* Pending */
.pending-card { text-align: center; }
.pending-icon { font-size: 4rem; margin-bottom: 12px; animation: popIn 0.5s ease; }
@keyframes popIn { 0%{transform:scale(0.5);opacity:0} 70%{transform:scale(1.1)} 100%{transform:scale(1);opacity:1} }
.pending-title { color: #f59e0b; font-weight: 700; font-size: 1rem; margin-bottom: 20px !important; }
.pending-box { background: #fffbeb; border: 1px solid #fde68a; border-radius: 16px; padding: 20px; margin-bottom: 24px; text-align: left; }
.pending-box p { color: #4b5563; font-size: 0.95rem; line-height: 1.7; margin: 0 0 12px; }
.pending-email { background: #fef3c7; border-radius: 10px; padding: 10px 14px; font-weight: 700; color: #92400e; font-size: 0.95rem; margin-bottom: 12px; }
.pending-hint { font-size: 0.85rem !important; color: #6b7280 !important; margin: 0 !important; }
.btn-login { width: 100%; border: none; background: linear-gradient(135deg, #be185d, #9d174d); color: white; padding: 14px; border-radius: 14px; font-weight: 700; font-size: 1rem; cursor: pointer; }

/* Form */
.form { display: flex; flex-direction: column; gap: 18px; }
.form-group { display: grid; gap: 6px; }
.form-group label { font-weight: 600; color: #374151; font-size: 0.9rem; }
.form-group input[type='text'],
.form-group input[type='email'],
.form-group input[type='password'] {
  padding: 12px 16px; border-radius: 12px;
  border: 1px solid #d1d5db; font-size: 1rem; color: #1f2937;
}
.form-group input:focus { outline: none; border-color: #be185d; box-shadow: 0 0 0 3px rgba(190,24,93,0.12); }

.role-group { display: flex; gap: 12px; }
.role-option {
  flex: 1; display: flex; align-items: center; gap: 8px;
  padding: 12px 16px; border: 2px solid #e5e7eb;
  border-radius: 12px; cursor: pointer; transition: all 0.2s;
}
.role-option.active { border-color: #be185d; background: #fdf2f8; }
.role-option input { display: none; }
.role-icon { font-size: 1.2rem; }

.info-notice {
  background: #f0fdf4; border: 1px solid #bbf7d0;
  border-radius: 12px; padding: 12px 14px;
  font-size: 0.85rem; color: #166534; line-height: 1.6;
}

.error-message { color: #dc2626; font-size: 0.9rem; }

button[type='submit'] {
  border: none; background: linear-gradient(135deg, #be185d, #9d174d);
  color: white; padding: 14px; border-radius: 14px;
  font-weight: 700; font-size: 1rem; cursor: pointer;
  transition: all 0.2s; box-shadow: 0 6px 16px rgba(190,24,93,0.25);
}
button[type='submit']:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(190,24,93,0.3); }
button[type='submit']:disabled { opacity: 0.7; cursor: not-allowed; transform: none; }

.login-link { text-align: center; font-size: 0.9rem; color: #6b7280; }
.login-link a { color: #be185d; font-weight: 700; text-decoration: none; }
</style>