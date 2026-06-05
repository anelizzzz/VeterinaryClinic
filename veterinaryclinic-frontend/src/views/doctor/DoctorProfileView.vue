<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { getDoctorProfile, type DoctorResponseDto } from '../../api/services/doctorService'

const router = useRouter()
const loading = ref(false)
const error = ref('')
const profile = ref<DoctorResponseDto | null>(null)

const avatarUrl = computed(() => {
  if (!profile.value?.email) return null
  return localStorage.getItem(`avatar_${profile.value.email}`)
})
const userInitial = computed(() => profile.value?.name?.charAt(0).toUpperCase() || 'D')

async function loadProfile() {
  try {
    loading.value = true
    profile.value = await getDoctorProfile()
  } catch {
    error.value = 'Nu am putut încărca profilul.'
  } finally {
    loading.value = false
  }
}

onMounted(() => { loadProfile() })
</script>

<template>
  <div class="page-wrapper">
    <div v-if="loading" class="info-box">Se încarcă profilul...</div>
    <div v-else-if="error" class="info-box error">{{ error }}</div>

    <template v-else-if="profile">
      <div class="profile-hero">
        <div class="avatar-wrap">
          <img v-if="avatarUrl" :src="avatarUrl" alt="Avatar" class="avatar-img" />
          <div v-else class="avatar-placeholder"><span>{{ userInitial }}</span></div>
        </div>
        <div class="hero-text">
          <h1>{{ profile.name }}</h1>
          <p class="hero-spec">🩺 {{ profile.specialization || 'Medic veterinar' }}</p>
          <p class="hero-email">{{ profile.email }}</p>
          <span class="role-badge">Doctor</span>
        </div>
        <button class="edit-btn" @click="router.push({ name: 'doctor-profile-edit' })">
          ✏️ Editează profilul
        </button>
      </div>

      <div class="info-grid">
        <div class="info-card">
          <div class="info-icon">📞</div>
          <div>
            <div class="info-label">Telefon</div>
            <div class="info-value">{{ profile.phone || '—' }}</div>
          </div>
        </div>
        <div class="info-card">
          <div class="info-icon">📧</div>
          <div>
            <div class="info-label">Email</div>
            <div class="info-value">{{ profile.email }}</div>
          </div>
        </div>
        <div class="info-card">
          <div class="info-icon">🕐</div>
          <div>
            <div class="info-label">Program</div>
            <div class="info-value">{{ profile.schedule || '—' }}</div>
          </div>
        </div>
        <div class="info-card">
          <div class="info-icon">🏥</div>
          <div>
            <div class="info-label">Specializare</div>
            <div class="info-value">{{ profile.specialization || '—' }}</div>
          </div>
        </div>
        <div v-if="profile.bio" class="info-card full">
          <div class="info-icon">📝</div>
          <div>
            <div class="info-label">Bio</div>
            <div class="info-value">{{ profile.bio }}</div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.page-wrapper { max-width: 860px; margin: 0 auto; padding: 3rem 2rem; display: grid; gap: 20px; }

.profile-hero { background: white; border-radius: 24px; padding: 2rem; box-shadow: 0 14px 32px rgba(0,0,0,0.08); display: flex; align-items: center; gap: 24px; flex-wrap: wrap; }
.avatar-wrap { flex-shrink: 0; }
.avatar-img { width: 100px; height: 100px; border-radius: 50%; object-fit: cover; border: 4px solid #ede9fe; box-shadow: 0 8px 20px rgba(118,15,92,0.15); }
.avatar-placeholder { width: 100px; height: 100px; border-radius: 50%; background: linear-gradient(135deg, #ede9fe, #ddd6fe); border: 4px solid #ede9fe; display: flex; align-items: center; justify-content: center; }
.avatar-placeholder span { font-size: 2.6rem; font-weight: 800; color: #760f5c; }

.hero-text { flex: 1; }
.hero-text h1 { font-size: 1.7rem; font-weight: 800; color: #1f2937; margin: 0 0 4px; }
.hero-spec { color: #7c3aed; font-size: 0.92rem; font-weight: 600; margin: 0 0 4px; }
.hero-email { color: #9ca3af; font-size: 0.88rem; margin: 0 0 10px; }
.role-badge { display: inline-block; background: #faf5ff; color: #760f5c; border: 1px solid #ddd6fe; border-radius: 999px; padding: 4px 12px; font-size: 0.8rem; font-weight: 700; }

.edit-btn { margin-left: auto; border: none; border-radius: 14px; padding: 10px 20px; background: linear-gradient(135deg, #760f5c, #5f0c49); color: white; font-weight: 700; font-size: 0.95rem; cursor: pointer; transition: all 0.2s; box-shadow: 0 6px 16px rgba(118,15,92,0.25); white-space: nowrap; }
.edit-btn:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(118,15,92,0.3); }

.info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; }
.info-card { background: white; border-radius: 18px; padding: 18px 20px; display: flex; align-items: flex-start; gap: 14px; border: 1px solid #f3f4f6; box-shadow: 0 2px 8px rgba(0,0,0,0.04); }
.info-card.full { grid-column: 1 / -1; }
.info-icon { font-size: 1.4rem; flex-shrink: 0; margin-top: 2px; }
.info-label { font-size: 0.75rem; font-weight: 700; color: #9ca3af; text-transform: uppercase; letter-spacing: 0.06em; margin-bottom: 4px; }
.info-value { font-size: 0.95rem; color: #1f2937; font-weight: 500; line-height: 1.5; }

.info-box { border-radius: 16px; padding: 1rem 1.2rem; background: #f9fafb; color: #374151; }
.info-box.error { background: #fee2e2; color: #b91c1c; }

@media (max-width: 640px) {
  .profile-hero { flex-direction: column; align-items: flex-start; }
  .edit-btn { margin-left: 0; width: 100%; }
  .info-grid { grid-template-columns: 1fr; }
  .info-card.full { grid-column: 1; }
}
</style>