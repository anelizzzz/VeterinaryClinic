
<template>
    <section class="login-page">
        <div class="login-wrapper">
            <div class="login-left">
                <span class="badge">Vetcare Platform</span>
        <h1>Bine ati revenit</h1>
        <p>
            Autentifica-te pentru a accesa programarile, pacientii si fisele medicale din cadrul cabinetului veterinar.
            </p>
            </div>
            <div class="login-right">
                <div class="login-care">
                    <p class="subtitle">Introdu datele contului tau.</p>
                <form @submit.prevent="handleLogin" class="auth-form">
                    <div class="form-group">
                        <label for="email">Email</label>
                        <input
                        id="email"
                        v-model.trim="email"
                        type="email"
                        placeholder="email@exempliu.com"
                        autocomplete="email"
                        />
                    </div>
                    <div class="form-group">
                        <label for="password">Parola</label>
                        <input
                        id="password"
                        v-model="password"
                        type="password"
                        placeholder="Introdu parola"
                        autocomplete="current-password"
                        />
                    </div> 
                    <button type="submit" :disabled="loading">
                        {{ loading ? 'Se autentifica...' : 'Autentificare' }}
                    </button>
                    <p v-if="error" class="error">{{ error }}</p>
                    </form>
                    <p class="register-link">
                        Nu ai cont? 
                        <router-link to="/register">Creeaza unul</router-link>
                    </p>
                
                </div>
            </div>

    </div>
    </section>
    
</template>

<script setup lang="ts">

import {ref} from 'vue'
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';

const email = ref<string>('')
const password = ref<string>('')
const error = ref<string> ('')
const loading = ref<boolean>(false)
const router = useRouter()
const authStore = useAuthStore()

const handleLogin = async (): Promise<void> => {
    try {
        error.value = ''
        if(!email.value || !password.value)
    {
        error.value = 'Completeaza email-ul si parola'
        return
    }
        loading.value = true
        await authStore.login({
            email : email.value,
            password : password.value
        })
        router.push('/dashboard')
    }catch{
        error.value = 'Email sau parola incorecta'
    } finally{
        loading.value = false
    }
}
</script>

<style scoped>
.login-page
{
    min-height: calc(100vh - 72px);
    background: linear-gradient(180deg, #f8fafc 0%, #eef6f5 100%);
    display: flex;
    align-items: center;
    padding: 32px 16px;
}
.login-wrapper{
    width: 100%;
    max-width: 1120px;
    margin: 0 auto;
    display: grid;
    grid-template-columns: 1.05fr 0.95fr;
    background: #ffffff;
    border-radius: 28px;
    overflow: hidden;
    box-shadow: 0 24px 60px rgba(15, 23, 42, 0.10);
}
.login-left{
    background : linear-gradient(135deg, #760f5c 0%, #134e4a 100%);
    color: white;
    padding: 56px;
    display: flex;
    flex-direction: column;
    justify-content: center;
}
.badge {
    display: inline-block;
    width: fit-content;
    margin-bottom: 18px;
    padding: 8px 14px;
    border-radius: 999px;
    background: rgba(255, 255, 255, 0.14);
    font-size: 14px;
    font-weight: 600;

}
.login-left h1{
    font-size: 44px;
    line-height: 1.1;
    margin-bottom: 16px;
}
.login-left p {
    font-size : 17px;
    line-height: 17px;
    color: #d1fae5;
    max-width: 420px;
}
.login-right{
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 48px;
    background: #fcfefd;
}
.login-card{
    width: 100%;
    max-width: 420px;
}
.login-card h2{
    margin: 0 0 8px;
    font-size: 32px;
    color: #111827;
}
.subtitle{
    margin-bottom: 28px;
    color: #760f5c;
}
.auth-form{
    display: flex;
    flex-direction: column;
    gap: 18px;
}
.form-group{
    display: flex;
    flex-direction: column;
    gap: 8px;
}
label{
    font-size: 14px;
    font-weight: 600;
    color: #760f5c;

}
input{
    width: 100%;
    border: 1px solid #d1d5db;
    border-radius: 14px;
    padding: 14px 16px;
    font-size: 15px;
    outline: none;
    transition: 0.2s ease;
    background: #fff;
}
input:focus{
    border-color: #760f5c;
    box-shadow: 0 0 0 4px rgba(15, 118, 110, 0.12);
}
button{
    margin-top: 6px;
    border: none;
    border-radius: 14px;
    background: #760f5c;
    color: white;
    padding: 14px 18px;
    font-size: 15px;
    font-weight: 700;
    cursor: pointer;
    transition: 0.2 ease;

}
button:hover {
  background: #115e59;
}

button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.error {
  margin-top: 6px;
  padding: 12px 14px;
  border-radius: 12px;
  background: #fef2f2;
  color: #b91c1c;
  font-size: 14px;
  border: 1px solid #fecaca;
}

.register-link {
  margin-top: 22px;
  font-size: 14px;
  color: #760f5c;
}

.register-link a {
  color: #760f5c;
  font-weight: 600;
  text-decoration: none;
}

.register-link a:hover {
  text-decoration: underline;
}

@media (max-width: 900px) {
  .login-wrapper {
    grid-template-columns: 1fr;
  }

  .login-left,
  .login-right {
    padding: 32px 24px;
  }

  .login-left h1 {
    font-size: 34px;
  }
}
</style>