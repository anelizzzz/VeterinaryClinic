<template>
  <div class="role-dashboard">
    <!-- ADMIN -->
    <section v-if="authStore.isAdmin" class="dashboard-section">
      <header class="dashboard-header">
        <h1>Panou Admin</h1>
        <p>Bun venit, {{ authStore.user?.name }}! De aici poți administra platforma.</p>
      </header>

      <div class="card-grid">
        <router-link to="/admin/pets" class="dashboard-card">
          <span class="card-icon">🐾</span>
          <h3>Pacienți</h3>
          <p>Vezi lista animalelor și informațiile lor.</p>
        </router-link>

        <router-link to="/admin/accounts" class="dashboard-card">
          <span class="card-icon">👤</span>
          <h3>Conturi</h3>
          <p>Vezi conturile create de clienți și doctori.</p>
        </router-link>

        <router-link to="/admin/appointments" class="dashboard-card">
          <span class="card-icon">📅</span>
          <h3>Programări</h3>
          <p>Vezi programările pentru fiecare doctor.</p>
        </router-link>
        
        <router-link to="/admin/adoptions" class="dashboard-card">
        <span class="card-icon">🐾</span>
        <h3>Adopții</h3>
        <p>Vezi animalele disponibile pentru adopție.</p>
        </router-link>

        <router-link to="/admin/adoption-requests" class="dashboard-card">
        <span class="card-icon">📩</span>
        <h3>Cereri adopție</h3>
        <p>Gestionează cererile trimise de clienți.</p>
        </router-link>

        <router-link to="/admin/profile" class="dashboard-card">
          <span class="card-icon">⚙️</span>
          <h3>Profil</h3>
          <p>Gestionează informațiile contului tău.</p>
        </router-link>
      </div>
    </section>

    <!-- DOCTOR -->
    <section v-else-if="authStore.isDoctor" class="dashboard-section">
      <header class="dashboard-header">
        <h1>Panou Doctor</h1>
        <p>Bun venit, Dr. {{ authStore.user?.name }}! Aici ai acces rapid la activitatea ta.</p>
      </header>

      <div class="card-grid">
        <router-link to="/doctor/appointments" class="dashboard-card">
          <span class="card-icon">📋</span>
          <h3>Programările mele</h3>
          <p>Vezi consultațiile programate și istoricul lor.</p>
        </router-link>

        <router-link to="/doctor/patients" class="dashboard-card">
          <span class="card-icon">🐶</span>
          <h3>Pacienții mei</h3>
          <p>Accesează fișele animalelor pe care le tratezi.</p>
        </router-link>

        <router-link to="/doctor/profile" class="dashboard-card">
          <span class="card-icon">👨‍⚕️</span>
          <h3>Profil doctor</h3>
          <p>Vezi și modifică datele tale profesionale.</p>
        </router-link>
      </div>
    </section>

    <!-- CLIENT -->
    <section v-else class="dashboard-section">
      <header class="dashboard-header">
        <h1>Bun venit, {{ authStore.user?.name }}!</h1>
        <p>De aici poți gestiona animalele tale și programările făcute.</p>
      </header>

      <div class="card-grid">
        <router-link to="/my-pets" class="dashboard-card">
          <span class="card-icon">🐕</span>
          <h3>Animalele mele</h3>
          <p>Vezi animalele înregistrate în contul tău.</p>
        </router-link>

        <router-link to="/my-appointments" class="dashboard-card">
          <span class="card-icon">🗓️</span>
          <h3>Programările mele</h3>
          <p>Verifică programările active și istoricul vizitelor.</p>
        </router-link>

        <router-link to="/appointments/create" class="dashboard-card">
          <span class="card-icon">➕</span>
          <h3>Programează o vizită</h3>
          <p>Fă rapid o nouă programare la un doctor.</p>
        </router-link>

        <router-link to="/client/profile" class="dashboard-card">
          <span class="card-icon">👤</span>
          <h3>Profilul meu</h3>
          <p>Actualizează datele personale ale contului.</p>
        </router-link>

        <router-link to="/client/adoption-requests" class="dashboard-card">
          <span class="card-icon">📨</span>
          <h3>Cererile mele de adopție</h3>
          <p>Vezi cererile trimise și statusul lor actual.</p>
        </router-link>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
</script>

<style scoped>
.role-dashboard {
  padding: 3rem 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.dashboard-section {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.dashboard-header {
  text-align: center;
}

.dashboard-header h1 {
  font-size: 2.4rem;
  font-weight: 800;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.dashboard-header p {
  font-size: 1.05rem;
  color: #6b7280;
  max-width: 720px;
  margin: 0 auto;
  line-height: 1.7;
}

.card-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 1.5rem;
}

.dashboard-card {
  background: white;
  border-radius: 22px;
  padding: 2rem;
  text-decoration: none;
  color: #1f2937;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  transition: transform 0.25s ease, box-shadow 0.25s ease;
  display: block;
}

.dashboard-card:hover {
  transform: translateY(-6px);
  box-shadow: 0 18px 40px rgba(233, 30, 99, 0.16);
}

.card-icon {
  font-size: 2.2rem;
  display: block;
  margin-bottom: 1rem;
}

.dashboard-card h3 {
  font-size: 1.2rem;
  margin-bottom: 0.6rem;
  font-weight: 700;
}

.dashboard-card p {
  font-size: 0.98rem;
  color: #6b7280;
  line-height: 1.6;
}

@media (max-width: 768px) {
  .role-dashboard {
    padding: 2rem 1rem;
  }

  .dashboard-header h1 {
    font-size: 2rem;
  }

  .dashboard-card {
    padding: 1.5rem;
  }
}
</style>