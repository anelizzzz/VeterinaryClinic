<script setup lang="ts">
import { onMounted, ref } from 'vue'
import api from '../../api/axios'

interface PendingUser {
  id: number
  name: string
  email: string
  phone: string
  role: string
  createdAt: string
}

const loading = ref(false)
const pendingUsers = ref<PendingUser[]>([])
const message = ref('')
const messageType = ref<'success' | 'error'>('success')

async function loadPending() {
  try {
    loading.value = true
    const res = await api.get('/AccountApproval/pending')
    pendingUsers.value = res.data
  } catch {
    message.value = 'Nu am putut încărca cererile.'
    messageType.value = 'error'
  } finally {
    loading.value = false
  }
}

async function approve(user: PendingUser) {
  try {
    await api.post(`/AccountApproval/approve/${user.id}`)
    message.value = `✅ Contul lui ${user.name} a fost aprobat. Email trimis!`
    messageType.value = 'success'
    pendingUsers.value = pendingUsers.value.filter(u => u.id !== user.id)
  } catch {
    message.value = 'Eroare la aprobare.'
    messageType.value = 'error'
  }
}

async function reject(user: PendingUser) {
  if (!confirm(`Ești sigur că vrei să respingi cererea lui ${user.name}?`)) return
  try {
    await api.post(`/AccountApproval/reject/${user.id}`)
    message.value = `❌ Cererea lui ${user.name} a fost respinsă.`
    messageType.value = 'success'
    pendingUsers.value = pendingUsers.value.filter(u => u.id !== user.id)
  } catch {
    message.value = 'Eroare la respingere.'
    messageType.value = 'error'
  }
}

function formatDate(d: string) {
  return new Date(d).toLocaleString('ro-RO', { dateStyle: 'medium', timeStyle: 'short' })
}

onMounted(() => { loadPending() })
</script>

<template>
  <div class="page-wrapper">
    <div class="page-card">
      <div class="page-header">
        <h1>Cereri de cont noi</h1>
        <p>Aprobă sau respinge cererile de înregistrare ale utilizatorilor.</p>
      </div>

      <div v-if="message" :class="['alert', messageType === 'success' ? 'alert-success' : 'alert-error']">
        {{ message }}
      </div>

      <div v-if="loading" class="info-box">Se încarcă cererile...</div>

      <div v-else-if="pendingUsers.length === 0" class="empty-box">
        <div class="empty-icon">✅</div>
        <p>Nu există cereri de aprobare în așteptare.</p>
      </div>

      <div v-else class="requests-list">
        <div v-for="user in pendingUsers" :key="user.id" class="request-card">
          <div class="request-avatar">{{ user.name.charAt(0).toUpperCase() }}</div>

          <div class="request-info">
            <div class="request-name">{{ user.name }}</div>
            <div class="request-meta">
              <span>📧 {{ user.email }}</span>
              <span v-if="user.phone">📞 {{ user.phone }}</span>
              <span>🕐 {{ formatDate(user.createdAt) }}</span>
            </div>
          </div>

          <span :class="['role-badge', user.role.toLowerCase()]">
            {{ user.role === 'Doctor' ? '🩺 Doctor' : '👤 Client' }}
          </span>

          <div class="request-actions">
            <button class="btn-approve" @click="approve(user)">✅ Aprobă</button>
            <button class="btn-reject" @click="reject(user)">❌ Respinge</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper { max-width: 1000px; margin: 0 auto; padding: 3rem 2rem; }
.page-card { background: white; border-radius: 24px; padding: 2rem; box-shadow: 0 14px 32px rgba(0,0,0,0.08); }
.page-header { text-align: center; margin-bottom: 1.5rem; }
.page-header h1 { font-size: 2rem; font-weight: 800; color: #1f2937; margin-bottom: 0.5rem; }
.page-header p { color: #6b7280; }

.alert { border-radius: 14px; padding: 12px 16px; margin-bottom: 16px; font-weight: 600; }
.alert-success { background: #dcfce7; color: #166534; }
.alert-error { background: #fee2e2; color: #b91c1c; }

.info-box { padding: 1.5rem; text-align: center; color: #6b7280; }

.empty-box { text-align: center; padding: 3rem; }
.empty-icon { font-size: 3rem; margin-bottom: 12px; }
.empty-box p { color: #6b7280; font-size: 1rem; }

.requests-list { display: grid; gap: 12px; }

.request-card {
  display: flex; align-items: center; gap: 16px; flex-wrap: wrap;
  border: 1px solid #f3f4f6; border-radius: 18px; padding: 16px 20px;
  background: #fafafa; transition: all 0.2s;
}
.request-card:hover { border-color: #fbcfe8; box-shadow: 0 4px 16px rgba(190,24,93,0.08); }

.request-avatar {
  width: 48px; height: 48px; border-radius: 50%; flex-shrink: 0;
  background: linear-gradient(135deg, #fce7f3, #fbcfe8);
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; font-weight: 800; color: #be185d;
}

.request-info { flex: 1; min-width: 200px; }
.request-name { font-weight: 700; color: #1f2937; font-size: 1rem; margin-bottom: 4px; }
.request-meta { display: flex; gap: 14px; flex-wrap: wrap; font-size: 0.82rem; color: #6b7280; }

.role-badge { padding: 4px 12px; border-radius: 999px; font-size: 0.82rem; font-weight: 700; white-space: nowrap; }
.role-badge.doctor { background: #ede9fe; color: #5b21b6; }
.role-badge.client { background: #e0f2fe; color: #0369a1; }

.request-actions { display: flex; gap: 8px; }
.btn-approve, .btn-reject {
  border: none; border-radius: 10px; padding: 8px 16px;
  font-weight: 700; font-size: 0.88rem; cursor: pointer; transition: all 0.2s;
}
.btn-approve { background: #dcfce7; color: #166534; }
.btn-approve:hover { background: #bbf7d0; }
.btn-reject { background: #fee2e2; color: #b91c1c; }
.btn-reject:hover { background: #fecaca; }

@media (max-width: 640px) {
  .request-card { flex-direction: column; align-items: flex-start; }
  .request-actions { width: 100%; }
  .btn-approve, .btn-reject { flex: 1; }
}
</style>