<script setup lang="ts">
import { onMounted, reactive, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { getDoctorProfile, updateDoctorProfile, type DoctorUpdateDto } from '../../api/services/doctorService'

const router = useRouter()
const loading = ref(false)
const saving = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const email = ref('')
const avatarUrl = ref<string | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)
const form = reactive<DoctorUpdateDto>({ name: '', phone: '', specialization: '', bio: '', schedule: '' })

const userInitial = computed(() => form.name?.charAt(0).toUpperCase() || 'D')
const storageKey = computed(() => `avatar_${email.value}`)

function loadAvatar() {
  const saved = localStorage.getItem(storageKey.value)
  if (saved) avatarUrl.value = saved
}

function triggerFileInput() { fileInput.value?.click() }

function onFileChange(e: Event) {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return
  if (!file.type.startsWith('image/')) { errorMessage.value = 'Selectează o imagine (JPG, PNG).'; return }
  if (file.size > 2 * 1024 * 1024) { errorMessage.value = 'Imaginea trebuie să fie mai mică de 2MB.'; return }
  const reader = new FileReader()
  reader.onload = (ev) => {
    const result = ev.target?.result as string
    avatarUrl.value = result
    localStorage.setItem(storageKey.value, result)
    errorMessage.value = ''
  }
  reader.readAsDataURL(file)
}

function removeAvatar() {
  avatarUrl.value = null
  localStorage.removeItem(storageKey.value)
  if (fileInput.value) fileInput.value.value = ''
}

async function loadProfile() {
  try {
    loading.value = true
    const data = await getDoctorProfile()
    form.name = data.name
    form.phone = data.phone
    form.specialization = data.specialization
    form.bio = data.bio
    form.schedule = data.schedule
    email.value = data.email
    loadAvatar()
  } catch {
    errorMessage.value = 'Nu am putut încărca profilul.'
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  try {
    errorMessage.value = ''
    successMessage.value = ''
    if (!form.name.trim()) { errorMessage.value = 'Numele este obligatoriu.'; return }
    if (!form.phone.trim()) { errorMessage.value = 'Telefonul este obligatoriu.'; return }
    if (!form.specialization.trim()) { errorMessage.value = 'Specializarea este obligatorie.'; return }
    saving.value = true
    await updateDoctorProfile({ name: form.name.trim(), phone: form.phone.trim(), specialization: form.specialization.trim(), bio: form.bio.trim(), schedule: form.schedule.trim() })
    successMessage.value = 'Profilul a fost actualizat cu succes.'
    setTimeout(() => router.push({ name: 'doctor-profile' }), 1200)
  } catch {
    errorMessage.value = 'A apărut o eroare la salvarea profilului.'
  } finally {
    saving.value = false
  }
}

onMounted(() => { loadProfile() })
</script>

<template>
  <div class="page-wrapper">
    <div class="page-nav">
      <button class="back-btn" @click="router.push({ name: 'doctor-profile' })">← Înapoi la profil</button>
    </div>

    <div class="edit-card">
      <div class="avatar-section">
        <div class="avatar-wrap">
          <img v-if="avatarUrl" :src="avatarUrl" alt="Avatar" class="avatar-img" />
          <div v-else class="avatar-placeholder"><span>{{ userInitial }}</span></div>
          <button class="avatar-edit-btn" type="button" @click="triggerFileInput">✏️</button>
        </div>
        <input ref="fileInput" type="file" accept="image/*" class="hidden-input" @change="onFileChange" />
        <div class="avatar-btns">
          <button type="button" class="upload-btn" @click="triggerFileInput">
            📷 {{ avatarUrl ? 'Schimbă poza' : 'Adaugă poza' }}
          </button>
          <button v-if="avatarUrl" type="button" class="remove-btn" @click="removeAvatar">Elimină</button>
        </div>
        <p class="avatar-hint">JPG sau PNG, maxim 2MB</p>
      </div>

      <div class="divider" />

      <h2>Editează profilul</h2>

      <div v-if="loading" class="info-box">Se încarcă datele...</div>

      <form v-else class="edit-form" @submit.prevent="handleSubmit">
        <div v-if="errorMessage" class="info-box error">{{ errorMessage }}</div>
        <div v-if="successMessage" class="info-box success">{{ successMessage }}</div>

        <div class="form-group">
          <label for="name">Nume complet *</label>
          <input id="name" v-model="form.name" type="text" placeholder="Introdu numele complet" />
        </div>
        <div class="form-group">
          <label>Email</label>
          <input :value="email" type="email" disabled />
        </div>
        <div class="form-group">
          <label for="phone">Telefon *</label>
          <input id="phone" v-model="form.phone" type="text" placeholder="Introdu numărul de telefon" />
        </div>
        <div class="form-group">
          <label for="specialization">Specializare *</label>
          <input id="specialization" v-model="form.specialization" type="text" placeholder="Ex: Chirurgie veterinară" />
        </div>
        <div class="form-group">
          <label for="schedule">Program</label>
          <textarea id="schedule" v-model="form.schedule" rows="2" placeholder="Ex: Luni-Vineri 08:00-16:00" />
        </div>
        <div class="form-group">
          <label for="bio">Bio</label>
          <textarea id="bio" v-model="form.bio" rows="4" placeholder="Scurtă descriere profesională" />
        </div>

        <div class="form-actions">
          <button type="button" class="cancel-btn" @click="router.push({ name: 'doctor-profile' })">Anulează</button>
          <button type="submit" class="save-btn" :disabled="saving">
            {{ saving ? 'Se salvează...' : '✓ Salvează modificările' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper { max-width: 640px; margin: 0 auto; padding: 2rem 2rem 3rem; display: grid; gap: 16px; }
.back-btn { background: none; border: none; font-size: 0.95rem; color: #6b7280; cursor: pointer; padding: 0; font-weight: 600; }
.back-btn:hover { color: #760f5c; }

.edit-card { background: white; border-radius: 24px; padding: 2rem; box-shadow: 0 14px 32px rgba(0,0,0,0.08); }

.avatar-section { display: flex; flex-direction: column; align-items: center; gap: 10px; }
.avatar-wrap { position: relative; width: 100px; height: 100px; }
.avatar-img { width: 100px; height: 100px; border-radius: 50%; object-fit: cover; border: 4px solid #ede9fe; box-shadow: 0 6px 20px rgba(118,15,92,0.15); }
.avatar-placeholder { width: 100px; height: 100px; border-radius: 50%; background: linear-gradient(135deg, #ede9fe, #ddd6fe); border: 4px solid #ede9fe; display: flex; align-items: center; justify-content: center; }
.avatar-placeholder span { font-size: 2.6rem; font-weight: 800; color: #760f5c; }
.avatar-edit-btn { position: absolute; bottom: 4px; right: 4px; width: 28px; height: 28px; border-radius: 50%; border: 2px solid white; background: #760f5c; color: white; font-size: 0.7rem; cursor: pointer; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 6px rgba(0,0,0,0.2); }
.avatar-edit-btn:hover { background: #5f0c49; }
.hidden-input { display: none; }
.avatar-btns { display: flex; gap: 8px; }
.upload-btn { border: 1px solid #ddd6fe; border-radius: 10px; padding: 6px 14px; font-size: 0.83rem; font-weight: 600; cursor: pointer; background: #faf5ff; color: #760f5c; }
.upload-btn:hover { background: #ede9fe; }
.remove-btn { border: 1px solid #fecaca; border-radius: 10px; padding: 6px 14px; font-size: 0.83rem; font-weight: 600; cursor: pointer; background: #fef2f2; color: #b91c1c; }
.remove-btn:hover { background: #fee2e2; }
.avatar-hint { font-size: 0.78rem; color: #9ca3af; margin: 0; }

.divider { border: none; border-top: 1px solid #f3f4f6; margin: 1.25rem 0; }
.edit-card h2 { font-size: 1.25rem; font-weight: 800; color: #1f2937; margin: 0 0 1.25rem; }

.edit-form { display: grid; gap: 1.1rem; }
.form-group { display: grid; gap: 0.5rem; }
.form-group label { font-size: 0.9rem; font-weight: 600; color: #374151; }
.form-group input, .form-group textarea { border: 1px solid #d1d5db; border-radius: 12px; padding: 0.85rem 1rem; font-size: 0.95rem; color: #1f2937; background: #fff; resize: vertical; }
.form-group input:focus, .form-group textarea:focus { outline: none; border-color: #760f5c; box-shadow: 0 0 0 3px rgba(118,15,92,0.12); }
.form-group input:disabled { background: #f3f4f6; color: #9ca3af; cursor: not-allowed; }

.form-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 0.5rem; }
.cancel-btn { border: 1px solid #e5e7eb; border-radius: 12px; padding: 10px 18px; background: white; color: #374151; font-weight: 600; cursor: pointer; font-size: 0.95rem; }
.cancel-btn:hover { background: #f9fafb; }
.save-btn { border: none; border-radius: 12px; padding: 10px 22px; background: linear-gradient(135deg, #760f5c, #5f0c49); color: white; font-weight: 700; cursor: pointer; font-size: 0.95rem; box-shadow: 0 4px 14px rgba(118,15,92,0.25); }
.save-btn:hover { transform: translateY(-1px); box-shadow: 0 8px 20px rgba(118,15,92,0.3); }
.save-btn:disabled { opacity: 0.7; cursor: not-allowed; transform: none; }

.info-box { border-radius: 14px; padding: 0.9rem 1.1rem; background: #f9fafb; color: #374151; }
.info-box.error { background: #fee2e2; color: #b91c1c; }
.info-box.success { background: #dcfce7; color: #166534; }
</style>