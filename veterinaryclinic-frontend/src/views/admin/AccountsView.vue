<template>
  <section class="admin-page">
    <div class="page-header">
      <h1>Conturi</h1>
      <p>Gestionează toate conturile, doctorii și clienții din platformă.</p>
    </div>

    <div class="tabs">
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'all' }"
        @click="activeTab = 'all'"
      >
        Toate conturile
      </button>

      <button
        class="tab-btn"
        :class="{ active: activeTab === 'doctors' }"
        @click="activeTab = 'doctors'"
      >
        Doctori
      </button>

      <button
        class="tab-btn"
        :class="{ active: activeTab === 'clients' }"
        @click="activeTab = 'clients'"
      >
        Clienți
      </button>
    </div>

    <p v-if="loading" class="state-message">Se încarcă datele...</p>
    <p v-else-if="error" class="error-message">{{ error }}</p>

    <div v-else class="table-card">
      <!-- TOATE USER-URILE -->
      <table v-if="activeTab === 'all'">
        <thead>
          <tr>
            <th>Id</th>
            <th>Nume</th>
            <th>Email</th>
            <th>Telefon</th>
            <th>Rol</th>
            <th>Acțiuni</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.name }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.phone }}</td>
            <td>{{ roleLabel(user.role) }}</td>
            <td>
              <div class="row-actions">
                <button class="action-btn edit" @click="editUser(user.id)">
                  Modifică
                </button>
                <button
                  class="action-btn delete"
                  @click="removeUser(user.id, user.role)"
                >
                  Șterge
                </button>
              </div>
            </td>
          </tr>

          <tr v-if="users.length === 0">
            <td colspan="6" class="empty-row">Nu există conturi disponibile.</td>
          </tr>
        </tbody>
      </table>

      <!-- DOCTORI -->
      <table v-else-if="activeTab === 'doctors'">
        <thead>
          <tr>
            <th>Id</th>
            <th>Nume</th>
            <th>Email</th>
            <th>Telefon</th>
            <th>Specializare</th>
            <th>Bio</th>
            <th>Acțiuni</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="doctor in doctors" :key="doctor.id">
            <td>{{ doctor.id }}</td>
            <td>{{ doctor.name }}</td>
            <td>{{ doctor.email }}</td>
            <td>{{ doctor.phone }}</td>
            <td>{{ doctor.specialization }}</td>
            <td class="bio-cell">{{ doctor.bio || '-' }}</td>
            <td>
              <div class="row-actions">
                <button class="action-btn edit" @click="editDoctor(doctor.id)">
                  Modifică
                </button>
                <button class="action-btn delete" @click="removeDoctor(doctor.id)">
                  Șterge
                </button>
              </div>
            </td>
          </tr>

          <tr v-if="doctors.length === 0">
            <td colspan="7" class="empty-row">Nu există doctori disponibili.</td>
          </tr>
        </tbody>
      </table>

      <!-- CLIENȚI -->
      <table v-else>
        <thead>
          <tr>
            <th>Id</th>
            <th>Nume</th>
            <th>Email</th>
            <th>Telefon</th>
            <th>Adresă</th>
            <th>Acțiuni</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="client in clients" :key="client.id">
            <td>{{ client.id }}</td>
            <td>{{ client.name }}</td>
            <td>{{ client.email }}</td>
            <td>{{ client.phone }}</td>
            <td>{{ client.address || '-' }}</td>
            <td>
              <div class="row-actions">
                <button class="action-btn delete" @click="removeClient(client.id)">
                  Șterge
                </button>
              </div>
            </td>
          </tr>

          <tr v-if="clients.length === 0">
            <td colspan="6" class="empty-row">Nu există clienți disponibili.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { UserRole } from '../../types/auth'
import {
  getAllUsers,
  deleteUser,
  type UserAccountDto
} from '../../api/services/accountService'
import {
  getAllDoctors,
  deleteDoctor,
  type DoctorResponseDto
} from '../../api/services/doctorService'
import {
  getAllClients,
  deleteClient,
  type ClientResponseDto
} from '../../api/services/clientService'

const router = useRouter()

const users = ref<UserAccountDto[]>([])
const doctors = ref<DoctorResponseDto[]>([])
const clients = ref<ClientResponseDto[]>([])

const loading = ref(false)
const error = ref('')
const activeTab = ref<'all' | 'doctors' | 'clients'>('all')

const roleLabel = (role: number): string => {
  if (role === UserRole.Admin) return 'Admin'
  if (role === UserRole.Doctor) return 'Doctor'
  return 'Client'
}

const loadData = async (): Promise<void> => {
  try {
    loading.value = true
    error.value = ''

    const [usersData, doctorsData, clientsData] = await Promise.all([
      getAllUsers(),
      getAllDoctors(),
      getAllClients()
    ])

    const approvedUsers = usersData.filter((u: UserAccountDto) => u.isApproved === true)
    const approvedUserIds = new Set(approvedUsers.map((u: UserAccountDto) => u.id))

    users.value = approvedUsers
    doctors.value = doctorsData.filter((d: DoctorResponseDto) => d.isApproved === true)
    clients.value = clientsData.filter((c: ClientResponseDto) => c.isApproved === true)
  } catch (err: any) {
    console.error(err)
    error.value = 'Nu s-au putut încărca datele.'
  } finally {
    loading.value = false
  }
}

function editUser(id: number) {
  router.push(`/admin/accounts/users/edit/${id}`)
}

function editDoctor(id: number) {
  router.push(`/admin/accounts/doctors/edit/${id}`)
}

function editClient(id: number) {
  router.push(`/admin/accounts/clients/edit/${id}`)
}

async function removeUser(id: number, role: number) {
  if (role === UserRole.Admin) {
    error.value = 'Conturile de admin nu pot fi șterse din această listă.'
    return
  }

  const confirmed = window.confirm('Sigur vrei să ștergi acest cont?')
  if (!confirmed) return

  try {
    error.value = ''
    await deleteUser(id)

    users.value = users.value.filter(user => user.id !== id)

    if (role === UserRole.Doctor) {
      doctors.value = doctors.value.filter(doctor => doctor.id !== id)
    }

    if (role === UserRole.Client) {
      clients.value = clients.value.filter(client => client.id !== id)
    }
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-a putut șterge contul.'
  }
}

async function removeDoctor(id: number) {
  const confirmed = window.confirm('Sigur vrei să ștergi acest doctor?')
  if (!confirmed) return

  try {
    error.value = ''
    await deleteDoctor(id)

    doctors.value = doctors.value.filter(doctor => doctor.id !== id)
    users.value = users.value.filter(
      user => !(user.id === id && user.role === UserRole.Doctor)
    )
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-a putut șterge doctorul.'
  }
}

async function removeClient(id: number) {
  const confirmed = window.confirm('Sigur vrei să ștergi acest client?')
  if (!confirmed) return

  try {
    error.value = ''
    await deleteClient(id)

    clients.value = clients.value.filter(client => client.id !== id)
    users.value = users.value.filter(
      user => !(user.id === id && user.role === UserRole.Client)
    )
  } catch (err) {
    console.error(err)
    error.value = 'Nu s-a putut șterge clientul.'
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.admin-page {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 1.5rem;
}

.page-header h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  color: #1f2937;
}

.page-header p {
  color: #6b7280;
}

.tabs {
  display: flex;
  gap: 12px;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.tab-btn {
  border: none;
  background: #f3f4f6;
  color: #374151;
  padding: 12px 18px;
  border-radius: 12px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s ease;
}

.tab-btn:hover {
  background: #e5e7eb;
}

.tab-btn.active {
  background: #760f5c;
  color: white;
}

.table-card {
  background: white;
  border-radius: 20px;
  padding: 1.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  text-align: left;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
  vertical-align: top;
}

th {
  color: #374151;
  font-weight: 700;
}

td {
  color: #4b5563;
}

.bio-cell {
  max-width: 280px;
  white-space: normal;
}

.row-actions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.action-btn {
  border: none;
  border-radius: 10px;
  padding: 8px 12px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s ease;
}

.action-btn.edit {
  background: #ede9fe;
  color: #6d28d9;
}

.action-btn.edit:hover {
  background: #ddd6fe;
}

.action-btn.delete {
  background: #fee2e2;
  color: #b91c1c;
}

.action-btn.delete:hover {
  background: #fecaca;
}

.state-message {
  color: #374151;
  font-weight: 500;
}

.error-message {
  color: #dc2626;
  font-weight: 600;
  margin-bottom: 1rem;
}

.empty-row {
  text-align: center;
  color: #6b7280;
  padding: 1.5rem;
}

@media (max-width: 768px) {
  .admin-page {
    padding: 1rem;
  }

  .table-card {
    padding: 1rem;
  }

  th,
  td {
    padding: 0.8rem;
  }
}
</style>