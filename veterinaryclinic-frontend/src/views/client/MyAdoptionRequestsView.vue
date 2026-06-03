<template>
  <div class="page-wrapper">
    <div class="page-header">
      <h1>Cererile mele de adopție</h1>
      <p>Vezi toate cererile trimise și statusul lor actual.</p>
    </div>

    <div v-if="loading" class="info-box">
      Se încarcă cererile...
    </div>

    <div v-else-if="errorMessage" class="info-box error">
      {{ errorMessage }}
    </div>

    <div v-else-if="requests.length === 0" class="info-box">
      Nu ai trimis încă nicio cerere de adopție.
    </div>

    <div v-else class="requests-list">
      <div
        v-for="request in requests"
        :key="request.id"
        class="request-card"
      >
        <div class="request-top">
          <div>
            <h2>{{ request.animalName }}</h2>
            <p class="meta"><strong>Data cererii:</strong> {{ formatDate(request.requestDate) }}</p>
          </div>

          <span class="request-status" :class="getStatusClass(request.status)">
            {{ formatRequestStatus(request.status) }}
          </span>
        </div>

        <div class="motivation-box">
          <h3>Motivația trimisă</h3>
          <p>{{ request.motivation }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import {
  getMyAdoptionRequests,
  type AdoptionRequestResponseDto,
  AdoptionStatus as AdoptionStatusEnum
} from '../../api/services/adoptionService'

const loading = ref(false)
const errorMessage = ref('')
const requests = ref<AdoptionRequestResponseDto[]>([])

async function loadRequests() {
  try {
    loading.value = true
    errorMessage.value = ''
    requests.value = await getMyAdoptionRequests()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Nu am putut încărca cererile tale de adopție.'
  } finally {
    loading.value = false
  }
}

function formatDate(value: string): string {
  return new Date(value).toLocaleString('ro-RO')
}

function formatRequestStatus(status: number): string {
  if (status === AdoptionStatusEnum.Available) return 'Disponibilă'
  if (status === AdoptionStatusEnum.Pending) return 'În așteptare'
  if (status === AdoptionStatusEnum.Reserved) return 'Rezervată'
  if (status === AdoptionStatusEnum.Adopted) return 'Adoptată'
  if (status === AdoptionStatusEnum.Rejected) return 'Respinsă'
  if (status === AdoptionStatusEnum.Approved) return 'Aprobată'
  return `Status ${status}`
}

function getStatusClass(status: number): string {
  if (status === AdoptionStatusEnum.Available) return 'available'
  if (status === AdoptionStatusEnum.Pending) return 'pending'
  if (status === AdoptionStatusEnum.Reserved) return 'reserved'
  if (status === AdoptionStatusEnum.Adopted) return 'adopted'
  if (status === AdoptionStatusEnum.Rejected) return 'rejected'
  if (status === AdoptionStatusEnum.Approved) return 'approved'
  return ''
}

onMounted(() => {
  loadRequests()
})
</script>

<style scoped>
.page-wrapper {
  max-width: 1100px;
  margin: 0 auto;
  padding: 3rem 2rem;
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.4rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.page-header p {
  color: #6b7280;
  max-width: 720px;
  margin: 0 auto;
}

.requests-list {
  display: grid;
  gap: 1.5rem;
}

.request-card {
  background: white;
  border-radius: 24px;
  padding: 1.5rem;
  box-shadow: 0 14px 32px rgba(0, 0, 0, 0.08);
}

.request-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1rem;
}

.request-top h2 {
  font-size: 1.35rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.meta {
  color: #6b7280;
  margin: 0.25rem 0;
}

.request-status {
  border-radius: 999px;
  padding: 0.35rem 0.8rem;
  font-size: 0.9rem;
  font-weight: 700;
  white-space: nowrap;
}

.request-status.available {
  background: #dbeafe;
  color: #1d4ed8;
}

.request-status.pending {
  background: #fef3c7;
  color: #92400e;
}

.request-status.reserved {
  background: #ede9fe;
  color: #6d28d9;
}

.request-status.adopted {
  background: #dcfce7;
  color: #166534;
}

.request-status.rejected {
  background: #fee2e2;
  color: #b91c1c;
}

.request-status.approved {
  background: #d1fae5;
  color: #065f46;
}

.motivation-box {
  background: #f9fafb;
  border-radius: 16px;
  padding: 1rem 1.1rem;
}

.motivation-box h3 {
  margin-bottom: 0.5rem;
  font-size: 1rem;
  color: #1f2937;
}

.motivation-box p {
  color: #4b5563;
  line-height: 1.6;
}

.info-box {
  margin-bottom: 1rem;
  border-radius: 16px;
  padding: 1rem 1.2rem;
  background: #f9fafb;
  color: #374151;
}

.info-box.error {
  background: #fee2e2;
  color: #b91c1c;
}

@media (max-width: 768px) {
  .page-wrapper {
    padding: 2rem 1rem;
  }

  .request-top {
    flex-direction: column;
  }
}
</style>