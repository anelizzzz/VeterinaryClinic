<template>
  <section class="hero">
    <div class="container hero-grid">
      <div>
        <span class="badge">Platformă pentru cabinet veterinar</span>
        <h1 class="title">
          Gestionare modernă pentru programări, pacienți și fișe medicale
        </h1>
        <p class="text">
          O aplicație web completă pentru clinici veterinare, cu autentificare,
          programări, adopții și integrare AI.
        </p>

        <div class="actions">
          <router-link to="/login" class="btn-primary">Login</router-link>
          <router-link to="/register" class="btn-secondary">Register</router-link>
        </div>
      </div>

      <div class="card">
        <h3>Funcționalități</h3>
        <ul>
          <li>Programări online</li>
          <li>Fișe medicale</li>
          <li>Rezultate analize</li>
          <li>Listă medici</li>
          <li>Adopții și cereri</li>
        </ul>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
.hero {
  padding: 80px 0;
}

.hero-grid {
  display: grid;
  grid-template-columns: 1.2fr 0.8fr;
  gap: 40px;
  align-items: center;
}

.badge {
  display: inline-block;
  background: #ccfbf1;
  color: #115e59;
  padding: 8px 14px;
  border-radius: 999px;
  font-weight: 600;
  margin-bottom: 20px;
}

.title {
  font-size: 48px;
  line-height: 1.1;
  margin-bottom: 20px;
  color: #111827;
}

.text {
  font-size: 18px;
  color: #4b5563;
  line-height: 1.8;
  margin-bottom: 30px;
}

.actions {
  display: flex;
  gap: 16px;
}

.btn-primary {
  text-decoration: none;
  background: #0f766e;
  color: white;
  padding: 14px 22px;
  border-radius: 12px;
  font-weight: 600;
}

.btn-secondary {
  text-decoration: none;
  background: white;
  color: #111827;
  padding: 14px 22px;
  border-radius: 12px;
  border: 1px solid #d1d5db;
  font-weight: 600;
}

.card {
  background: white;
  padding: 28px;
  border-radius: 20px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.08);
}
</style>