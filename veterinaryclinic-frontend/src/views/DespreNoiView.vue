<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getAllDoctors, type DoctorResponseDto } from '../api/services/doctorService'

const doctors = ref<DoctorResponseDto[]>([])

function getDoctorAvatar(doctor: DoctorResponseDto): string | null {
  return localStorage.getItem(`avatar_${doctor.email}`) ?? null
}

function getInitial(name: string): string {
  return name?.charAt(0).toUpperCase() || 'D'
}

onMounted(async () => {
  try {
    doctors.value = await getAllDoctors()
  } catch {
    // fallback silentios
  }
})
</script>

<template>
  <div class="home">

    <!-- HERO -->
    <section class="hero">
      <div class="hero-bg">
        <div class="blob blob-1" />
        <div class="blob blob-2" />
        <div class="blob blob-3" />
      </div>
      <div class="container">
        <div class="hero-inner">
          <div class="hero-text">
            <span class="pill">🐾 Clinică Veterinară</span>
            <h1>
              Îngrijire cu drag<br />
              <span class="accent">pentru animalul</span><br />
              tău de companie
            </h1>
            <p>
              La VetCare îți oferim servicii complete de sănătate pentru animale —
              de la controale de rutină și vaccinări, până la analize de laborator
              și fișe medicale digitale. Tot ce are nevoie prietenul tău blănos,
              într-un singur loc.
            </p>
            <div class="hero-actions">
              <router-link to="/register" class="btn-primary">Creează cont gratuit</router-link>
              <router-link to="/login" class="btn-ghost">Intră în cont →</router-link>
            </div>
          </div>
          <div class="hero-visual">
            <div class="visual-card main-card">
              <div class="vc-header">
                <div class="vc-avatar">🐶</div>
                <div>
                  <div class="vc-name">Max</div>
                  <div class="vc-breed">Labrador · 3 ani</div>
                </div>
                <span class="vc-status healthy">Sănătos</span>
              </div>
              <div class="vc-row"><span>Ultima vizită</span><strong>02 iun 2026</strong></div>
              <div class="vc-row"><span>Vaccin următor</span><strong>Aug 2026</strong></div>
              <div class="vc-row"><span>Doctor</span><strong>Dr. Ionescu</strong></div>
            </div>
            <div class="visual-card mini-card top">
              <span class="mini-icon">📅</span>
              <div>
                <div class="mini-title">Programare</div>
                <div class="mini-sub">Mâine · 10:00</div>
              </div>
            </div>
            <div class="visual-card mini-card bottom">
              <span class="mini-icon">🔬</span>
              <div>
                <div class="mini-title">Analize gata</div>
                <div class="mini-sub">Rezultate noi</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- STATS -->
    <section class="stats-section">
      <div class="container">
        <div class="stats-grid">
          <div class="stat-item">
            <strong>1,200+</strong>
            <span>Pacienți tratați</span>
          </div>
          <div class="stat-divider" />
          <div class="stat-item">
            <strong>8</strong>
            <span>Medici specialiști</span>
          </div>
          <div class="stat-divider" />
          <div class="stat-item">
            <strong>15 ani</strong>
            <span>Experiență</span>
          </div>
          <div class="stat-divider" />
          <div class="stat-item">
            <strong>98%</strong>
            <span>Clienți mulțumiți</span>
          </div>
        </div>
      </div>
    </section>

    <!-- SERVICII -->
    <section class="services-section">
      <div class="container">
        <div class="section-header">
          <span class="pill">Serviciile noastre</span>
          <h2>Tot ce are nevoie animalul tău</h2>
          <p>De la consultații de rutină la proceduri complexe, suntem alături de tine la fiecare pas.</p>
        </div>
        <div class="services-grid">
          <div class="service-card">
            <div class="service-icon" style="background:#fff0f9;">🩺</div>
            <h3>Consultații</h3>
            <p>Controale de rutină, diagnostic și monitorizare periodică a sănătății animalului tău.</p>
          </div>
          <div class="service-card">
            <div class="service-icon" style="background:#f0f9ff;">💉</div>
            <h3>Vaccinări</h3>
            <p>Program complet de vaccinare cu reminder automat pentru fiecare doză importantă.</p>
          </div>
          <div class="service-card">
            <div class="service-icon" style="background:#f0fdf4;">🔬</div>
            <h3>Analize laborator</h3>
            <p>Analize de sânge, urină și alte teste cu rezultate digitale accesibile oricând.</p>
          </div>
          <div class="service-card">
            <div class="service-icon" style="background:#fefce8;">🏥</div>
            <h3>Intervenții chirurgicale</h3>
            <p>Proceduri chirurgicale efectuate de medici cu experiență, în condiții sterile.</p>
          </div>
          <div class="service-card">
            <div class="service-icon" style="background:#fff7ed;">🧬</div>
            <h3>Diagnostic AI</h3>
            <p>Sistem AI integrat care analizează rezultatele de laborator și oferă estimări orientative.</p>
          </div>
          <div class="service-card">
            <div class="service-icon" style="background:#fdf4ff;">🐾</div>
            <h3>Adopții</h3>
            <p>Platformă de adopție pentru animale fără stăpân, cu verificare medicală completă.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- CUM FUNCTIONEAZA -->
    <section class="how-section">
      <div class="container">
        <div class="section-header">
          <span class="pill">Cum funcționează</span>
          <h2>Simplu, rapid, digital</h2>
        </div>
        <div class="steps-grid">
          <div class="step-card">
            <div class="step-num">01</div>
            <h3>Creează cont</h3>
            <p>Înregistrează-te gratuit și adaugă profilul animalului tău de companie.</p>
          </div>
          <div class="step-arrow">→</div>
          <div class="step-card">
            <div class="step-num">02</div>
            <h3>Programează vizita</h3>
            <p>Alege data, ora și medicul dorit direct din aplicație, în câteva secunde.</p>
          </div>
          <div class="step-arrow">→</div>
          <div class="step-card">
            <div class="step-num">03</div>
            <h3>Urmărește istoricul</h3>
            <p>Accesează fișa medicală, analizele și recomandările doctorului oricând, de oriunde.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- TESTIMONIALE -->
    <section class="testimonials-section">
      <div class="container">
        <div class="section-header">
          <span class="pill">Ce spun clienții</span>
          <h2>Iubiți de proprietari</h2>
        </div>
        <div class="testimonials-grid">
          <div class="testimonial-card">
            <div class="stars">★★★★★</div>
            <p>„Aplicația mi-a schimbat complet modul în care îngrijesc pisica mea. Totul e la un click distanță!"</p>
            <div class="testimonial-author">
              <div class="author-avatar">M</div>
              <div>
                <div class="author-name">Maria Popescu</div>
                <div class="author-pet">Proprietara lui Whiskers 🐱</div>
              </div>
            </div>
          </div>
          <div class="testimonial-card featured">
            <div class="stars">★★★★★</div>
            <p>„Diagnosticul AI m-a ajutat enorm. Am văzut rapid că analizele lui Rex indicau o problemă și am venit la timp la consult."</p>
            <div class="testimonial-author">
              <div class="author-avatar">A</div>
              <div>
                <div class="author-name">Alexandru Marin</div>
                <div class="author-pet">Proprietarul lui Rex 🐶</div>
              </div>
            </div>
          </div>
          <div class="testimonial-card">
            <div class="stars">★★★★★</div>
            <p>„Recordurile medicale digitale sunt extraordinare. Nu mai caut prin hârtii — totul e salvat și organizat perfect."</p>
            <div class="testimonial-author">
              <div class="author-avatar">I</div>
              <div>
                <div class="author-name">Ioana Constantin</div>
                <div class="author-pet">Proprietara lui Biscuit 🐰</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- DOCTORI -->
    <section v-if="doctors.length > 0" class="doctors-section">
      <div class="container">
        <div class="section-header">
          <span class="pill">Echipa noastră</span>
          <h2>Medicii noștri veterinari</h2>
          <p>Profesioniști dedicați, cu experiență și pasiune pentru sănătatea animalelor tale.</p>
        </div>
        <div class="doctors-grid">
          <div v-for="doctor in doctors" :key="doctor.id" class="doctor-card">
            <div class="doctor-avatar-wrap">
              <img
                v-if="getDoctorAvatar(doctor)"
                :src="getDoctorAvatar(doctor)!"
                :alt="doctor.name"
                class="doctor-avatar-img"
              />
              <div v-else class="doctor-avatar-placeholder">
                <span>{{ getInitial(doctor.name) }}</span>
              </div>
            </div>
            <div class="doctor-info">
              <h3>{{ doctor.name }}</h3>
              <span class="doctor-spec">{{ doctor.specialization || 'Medic veterinar' }}</span>
              <p v-if="doctor.bio" class="doctor-bio">{{ doctor.bio }}</p>
              <div class="doctor-details">
                <div v-if="doctor.email" class="doctor-detail">
                  <span class="detail-icon">📧</span>
                  <a :href="`mailto:${doctor.email}`">{{ doctor.email }}</a>
                </div>
                <div v-if="doctor.phone" class="doctor-detail">
                  <span class="detail-icon">📞</span>
                  <span>{{ doctor.phone }}</span>
                </div>
                <div v-if="doctor.schedule" class="doctor-detail">
                  <span class="detail-icon">🕐</span>
                  <span>{{ doctor.schedule }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA FINAL -->
    <section class="cta-section">
      <div class="container">
        <div class="cta-box">
          <div class="cta-blob" />
          <span class="pill light">Începe acum</span>
          <h2>Gata să oferi cea mai bună îngrijire?</h2>
          <p>Înregistrează-te gratuit și programează prima consultație pentru animalul tău.</p>
          <div class="cta-actions">
            <router-link to="/register" class="btn-primary large">Creează cont gratuit</router-link>
            <router-link to="/adoptii" class="btn-ghost-light">Explorează adopții 🐾</router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- FOOTER -->
    <footer class="footer">
      <div class="container">
        <div class="footer-inner">
          <div class="footer-brand">
            <span class="footer-logo">🐾 VetCare</span>
            <p>Îngrijire veterinară modernă și digitală pentru animalele tale de companie.</p>
          </div>
          <div class="footer-links">
            <div class="footer-col">
              <div class="footer-col-title">Servicii</div>
              <a href="#">Consultații</a>
              <a href="#">Vaccinări</a>
              <a href="#">Analize</a>
              <a href="#">Adopții</a>
            </div>
            <div class="footer-col">
              <div class="footer-col-title">Cont</div>
              <router-link to="/register">Înregistrare</router-link>
              <router-link to="/login">Autentificare</router-link>
            </div>
          </div>
        </div>
        <div class="footer-bottom">
          <span>© 2026 VetCare. Toate drepturile rezervate.</span>
        </div>
      </div>
    </footer>

  </div>
</template>

<style scoped>
/* ===== BASE ===== */
.home { overflow-x: hidden; }

/* ===== HERO ===== */
.hero {
  position: relative;
  padding: 100px 0 80px;
  overflow: hidden;
  background: linear-gradient(160deg, #fff0f8 0%, #ffffff 50%, #f0f9ff 100%);
}

.hero-bg { position: absolute; inset: 0; pointer-events: none; }
.blob { position: absolute; border-radius: 50%; filter: blur(80px); opacity: 0.35; }
.blob-1 { width: 500px; height: 500px; background: #f9a8d4; top: -100px; right: -100px; animation: float 8s ease-in-out infinite; }
.blob-2 { width: 350px; height: 350px; background: #bfdbfe; bottom: -80px; left: -80px; animation: float 10s ease-in-out infinite reverse; }
.blob-3 { width: 250px; height: 250px; background: #fde68a; top: 50%; left: 40%; animation: float 6s ease-in-out infinite 2s; }

@keyframes float {
  0%, 100% { transform: translateY(0) scale(1); }
  50% { transform: translateY(-30px) scale(1.05); }
}

.hero-inner {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 60px;
  align-items: center;
  position: relative;
  z-index: 1;
}

.hero-text .pill { margin-bottom: 20px; }
.hero-text h1 {
  font-size: clamp(2.4rem, 3vw + 1rem, 3.6rem);
  font-weight: 900;
  line-height: 1.1;
  color: #1f2937;
  margin-bottom: 20px;
  font-family: Georgia, 'Times New Roman', serif;
}
.accent { color: #be185d; }
.hero-text p { font-size: 1.05rem; color: #4b5563; line-height: 1.8; margin-bottom: 32px; max-width: 48ch; }
.hero-actions { display: flex; gap: 14px; flex-wrap: wrap; align-items: center; }

/* Butoane */
.btn-primary {
  display: inline-block; text-decoration: none;
  background: linear-gradient(135deg, #be185d, #9d174d);
  color: white; padding: 14px 26px; border-radius: 14px;
  font-weight: 700; font-size: 1rem; transition: all 0.2s;
  box-shadow: 0 8px 20px rgba(190,24,93,0.3);
}
.btn-primary:hover { transform: translateY(-2px); box-shadow: 0 12px 28px rgba(190,24,93,0.4); }
.btn-primary.large { padding: 16px 32px; font-size: 1.05rem; }

.btn-ghost {
  display: inline-block; text-decoration: none;
  color: #be185d; font-weight: 700; font-size: 1rem;
  transition: all 0.2s; padding: 14px 4px;
}
.btn-ghost:hover { color: #9d174d; letter-spacing: 0.02em; }

.btn-ghost-light {
  display: inline-block; text-decoration: none;
  color: rgba(255,255,255,0.85); font-weight: 600; font-size: 1rem;
  transition: all 0.2s; padding: 16px 4px;
}
.btn-ghost-light:hover { color: white; }

/* Visual cards */
.hero-visual { position: relative; height: 380px; }
.visual-card {
  position: absolute; background: white; border-radius: 20px;
  padding: 20px; box-shadow: 0 20px 50px rgba(0,0,0,0.12);
  border: 1px solid rgba(255,255,255,0.8);
}
.main-card { width: 280px; top: 40px; left: 50%; transform: translateX(-50%); animation: cardFloat 6s ease-in-out infinite; }
@keyframes cardFloat { 0%,100%{transform:translateX(-50%) translateY(0)} 50%{transform:translateX(-50%) translateY(-12px)} }

.vc-header { display: flex; align-items: center; gap: 10px; margin-bottom: 16px; padding-bottom: 14px; border-bottom: 1px solid #f3f4f6; }
.vc-avatar { font-size: 2rem; }
.vc-name { font-weight: 800; color: #1f2937; font-size: 1rem; }
.vc-breed { font-size: 0.8rem; color: #9ca3af; }
.vc-status { margin-left: auto; font-size: 0.72rem; font-weight: 700; padding: 4px 10px; border-radius: 999px; }
.vc-status.healthy { background: #dcfce7; color: #166534; }
.vc-row { display: flex; justify-content: space-between; font-size: 0.85rem; padding: 6px 0; color: #6b7280; }
.vc-row strong { color: #1f2937; }

.mini-card { display: flex; align-items: center; gap: 12px; padding: 14px 18px; width: 200px; }
.mini-card.top { top: 10px; right: -20px; animation: cardFloat 7s ease-in-out infinite 1s; }
.mini-card.bottom { bottom: 40px; right: 0; animation: cardFloat 5s ease-in-out infinite 2s; }
.mini-icon { font-size: 1.6rem; }
.mini-title { font-weight: 700; color: #1f2937; font-size: 0.9rem; }
.mini-sub { font-size: 0.78rem; color: #9ca3af; }

/* ===== STATS ===== */
.stats-section { padding: 40px 0; background: white; border-top: 1px solid #f3f4f6; border-bottom: 1px solid #f3f4f6; }
.stats-grid { display: flex; align-items: center; justify-content: center; gap: 0; flex-wrap: wrap; }
.stat-item { text-align: center; padding: 20px 50px; }
.stat-item strong { display: block; font-size: 2.2rem; font-weight: 900; color: #be185d; font-family: Georgia, serif; }
.stat-item span { font-size: 0.88rem; color: #6b7280; font-weight: 600; text-transform: uppercase; letter-spacing: 0.06em; }
.stat-divider { width: 1px; height: 50px; background: #e5e7eb; }

/* ===== PILL ===== */
.pill {
  display: inline-block; background: #fdf2f8; color: #be185d;
  padding: 6px 16px; border-radius: 999px; font-size: 0.82rem;
  font-weight: 700; text-transform: uppercase; letter-spacing: 0.06em;
  border: 1px solid #fbcfe8; margin-bottom: 16px;
}
.pill.light { background: rgba(255,255,255,0.2); color: white; border-color: rgba(255,255,255,0.3); }

/* ===== SECTIONS ===== */
.section-header { text-align: center; margin-bottom: 52px; }
.section-header h2 { font-size: clamp(1.8rem, 2vw + 0.5rem, 2.6rem); font-weight: 900; color: #1f2937; margin-bottom: 12px; font-family: Georgia, serif; }
.section-header p { color: #6b7280; max-width: 50ch; margin: 0 auto; font-size: 1rem; line-height: 1.7; }

/* ===== SERVICES ===== */
.services-section { padding: 90px 0; background: #fafafa; }
.services-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 24px; }
.service-card {
  background: white; border-radius: 22px; padding: 28px;
  border: 1px solid #f3f4f6; transition: all 0.25s;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
.service-card:hover { transform: translateY(-6px); box-shadow: 0 16px 40px rgba(190,24,93,0.1); border-color: #fbcfe8; }
.service-icon { width: 56px; height: 56px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 1.6rem; margin-bottom: 18px; }
.service-card h3 { font-size: 1.05rem; font-weight: 800; color: #1f2937; margin-bottom: 8px; }
.service-card p { font-size: 0.9rem; color: #6b7280; line-height: 1.6; }

/* ===== HOW ===== */
.how-section { padding: 90px 0; background: white; }
.steps-grid { display: flex; align-items: center; gap: 20px; justify-content: center; flex-wrap: wrap; }
.step-card {
  background: linear-gradient(160deg, #fff0f8, #ffffff);
  border: 1px solid #fbcfe8; border-radius: 22px; padding: 32px 28px;
  flex: 1; min-width: 220px; max-width: 300px;
  transition: all 0.25s;
}
.step-card:hover { transform: translateY(-4px); box-shadow: 0 12px 32px rgba(190,24,93,0.1); }
.step-num { font-size: 2.5rem; font-weight: 900; color: #fbcfe8; font-family: Georgia, serif; line-height: 1; margin-bottom: 16px; }
.step-card h3 { font-size: 1.05rem; font-weight: 800; color: #1f2937; margin-bottom: 8px; }
.step-card p { font-size: 0.88rem; color: #6b7280; line-height: 1.6; }
.step-arrow { font-size: 1.5rem; color: #fbcfe8; font-weight: 900; flex-shrink: 0; }

/* ===== TESTIMONIALS ===== */
.testimonials-section { padding: 90px 0; background: #fafafa; }
.testimonials-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 24px; align-items: start; }
.testimonial-card {
  background: white; border-radius: 22px; padding: 28px;
  border: 1px solid #f3f4f6; box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
.testimonial-card.featured {
  background: linear-gradient(160deg, #be185d, #9d174d);
  border-color: transparent;
  transform: scale(1.04);
  box-shadow: 0 20px 50px rgba(190,24,93,0.25);
}
.testimonial-card.featured p, .testimonial-card.featured .stars { color: white; }
.testimonial-card.featured .author-name { color: white; }
.testimonial-card.featured .author-pet { color: rgba(255,255,255,0.7); }
.stars { color: #f59e0b; font-size: 1rem; margin-bottom: 14px; letter-spacing: 2px; }
.testimonial-card p { font-size: 0.95rem; color: #374151; line-height: 1.7; margin-bottom: 20px; font-style: italic; }
.testimonial-author { display: flex; align-items: center; gap: 12px; }
.author-avatar { width: 40px; height: 40px; background: #fdf2f8; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 800; color: #be185d; font-size: 1rem; flex-shrink: 0; }
.testimonial-card.featured .author-avatar { background: rgba(255,255,255,0.2); color: white; }
.author-name { font-weight: 700; color: #1f2937; font-size: 0.9rem; }
.author-pet { font-size: 0.78rem; color: #9ca3af; }

/* ===== CTA ===== */
.cta-section { padding: 80px 0; }
.cta-box {
  position: relative; overflow: hidden;
  background: linear-gradient(135deg, #be185d 0%, #9d174d 50%, #7c1237 100%);
  border-radius: 32px; padding: 70px 60px; text-align: center;
  box-shadow: 0 30px 80px rgba(190,24,93,0.3);
}
.cta-blob { position: absolute; width: 400px; height: 400px; border-radius: 50%; background: rgba(255,255,255,0.05); top: -100px; right: -100px; pointer-events: none; }
.cta-box h2 { font-size: clamp(1.8rem, 2.5vw + 0.5rem, 2.8rem); font-weight: 900; color: white; margin-bottom: 14px; font-family: Georgia, serif; position: relative; }
.cta-box p { color: rgba(255,255,255,0.8); font-size: 1rem; max-width: 50ch; margin: 0 auto 32px; line-height: 1.7; position: relative; }
.cta-actions { display: flex; gap: 20px; justify-content: center; flex-wrap: wrap; position: relative; }
.cta-actions .btn-primary { background: white; color: #be185d; box-shadow: 0 8px 24px rgba(0,0,0,0.15); }
.cta-actions .btn-primary:hover { box-shadow: 0 14px 32px rgba(0,0,0,0.2); }

/* ===== FOOTER ===== */
.footer { background: #1f2937; padding: 60px 0 30px; }
.footer-inner { display: flex; gap: 60px; margin-bottom: 40px; flex-wrap: wrap; }
.footer-brand { flex: 1; min-width: 220px; }
.footer-logo { font-size: 1.3rem; font-weight: 800; color: white; display: block; margin-bottom: 12px; }
.footer-brand p { color: #9ca3af; font-size: 0.9rem; line-height: 1.6; max-width: 30ch; }
.footer-links { display: flex; gap: 50px; flex-wrap: wrap; }
.footer-col { display: flex; flex-direction: column; gap: 10px; }
.footer-col-title { font-size: 0.78rem; font-weight: 700; color: #6b7280; text-transform: uppercase; letter-spacing: 0.1em; margin-bottom: 4px; }
.footer-col a { color: #9ca3af; text-decoration: none; font-size: 0.9rem; transition: color 0.2s; }
.footer-col a:hover { color: #f9a8d4; }
.footer-bottom { border-top: 1px solid #374151; padding-top: 24px; text-align: center; color: #6b7280; font-size: 0.85rem; }

/* ===== DOCTORS ===== */
.doctors-section { padding: 90px 0; background: white; }
.doctors-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 24px; }

.doctor-card {
  background: white; border-radius: 24px; padding: 28px;
  border: 1px solid #f3f4f6; box-shadow: 0 4px 16px rgba(0,0,0,0.06);
  display: flex; flex-direction: column; align-items: center; text-align: center;
  transition: all 0.25s;
}
.doctor-card:hover { transform: translateY(-6px); box-shadow: 0 16px 40px rgba(190,24,93,0.1); border-color: #fbcfe8; }

.doctor-avatar-wrap { margin-bottom: 18px; }
.doctor-avatar-img {
  width: 100px; height: 100px; border-radius: 50%; object-fit: cover;
  border: 4px solid #ede9fe; box-shadow: 0 6px 20px rgba(118,15,92,0.15);
}
.doctor-avatar-placeholder {
  width: 100px; height: 100px; border-radius: 50%;
  background: linear-gradient(135deg, #ede9fe, #ddd6fe);
  border: 4px solid #ede9fe; box-shadow: 0 6px 20px rgba(118,15,92,0.1);
  display: flex; align-items: center; justify-content: center;
}
.doctor-avatar-placeholder span { font-size: 2.4rem; font-weight: 800; color: #760f5c; }

.doctor-info { width: 100%; }
.doctor-info h3 { font-size: 1.1rem; font-weight: 800; color: #1f2937; margin: 0 0 6px; }
.doctor-spec {
  display: inline-block; background: #faf5ff; color: #760f5c;
  border: 1px solid #ddd6fe; border-radius: 999px;
  padding: 4px 12px; font-size: 0.78rem; font-weight: 700;
  margin-bottom: 12px;
}
.doctor-bio {
  font-size: 0.88rem; color: #6b7280; line-height: 1.6;
  margin: 0 0 14px; display: -webkit-box;
  -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;
}
.doctor-details { display: grid; gap: 8px; text-align: left; }
.doctor-detail { display: flex; align-items: flex-start; gap: 8px; font-size: 0.84rem; color: #4b5563; }
.detail-icon { font-size: 0.9rem; flex-shrink: 0; margin-top: 1px; }
.doctor-detail a { color: #be185d; text-decoration: none; font-weight: 500; word-break: break-all; }
.doctor-detail a:hover { text-decoration: underline; }

/* ===== RESPONSIVE ===== */
@media (max-width: 1024px) {
  .services-grid { grid-template-columns: repeat(2, 1fr); }
  .testimonials-grid { grid-template-columns: 1fr; }
  .testimonial-card.featured { transform: scale(1); }
  .doctors-grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 768px) {
  .hero { padding: 60px 0 50px; }
  .hero-inner { grid-template-columns: 1fr; gap: 40px; }
  .hero-visual { height: 300px; }
  .main-card { width: 240px; }
  .mini-card.top { right: 0; }
  .services-grid { grid-template-columns: 1fr; }
  .doctors-grid { grid-template-columns: 1fr; }
  .steps-grid { flex-direction: column; }
  .step-arrow { transform: rotate(90deg); }
  .step-card { max-width: 100%; }
  .stats-grid { gap: 0; }
  .stat-item { padding: 16px 24px; }
  .stat-divider { display: none; }
  .cta-box { padding: 48px 24px; }
  .footer-inner { flex-direction: column; gap: 32px; }
}
</style>