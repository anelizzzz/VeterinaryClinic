<script setup lang="ts">
</script>

<template>
  <div class="contact-page">

    <!-- HERO -->
    <section class="hero">
      <div class="hero-bg">
        <div class="blob blob-1" />
        <div class="blob blob-2" />
      </div>
      <div class="container">
        <span class="pill">📍 Găsește-ne</span>
        <h1>Contactează-ne</h1>
        <p>Suntem aici pentru tine și animalul tău. Vino să ne vizitezi sau scrie-ne oricând.</p>
      </div>
    </section>

    <!-- CONTENT -->
    <section class="content-section">
      <div class="container">
        <div class="content-grid">

          <!-- INFO CONTACT -->
          <div class="info-col">

            <div class="info-card">
              <div class="info-icon">📍</div>
              <div>
                <div class="info-label">Adresă</div>
                <div class="info-value">Strada Veterinarilor nr. 12<br />Sibiu, județul Sibiu</div>
              </div>
            </div>

            <div class="info-card">
              <div class="info-icon">📞</div>
              <div>
                <div class="info-label">Telefon</div>
                <div class="info-value">
                  <a href="tel:+40269123456">+40 269 123 456</a>
                </div>
              </div>
            </div>

            <div class="info-card">
              <div class="info-icon">📧</div>
              <div>
                <div class="info-label">Email</div>
                <div class="info-value">
                  <a href="mailto:contact@vetcare.ro">contact@vetcare.ro</a>
                </div>
              </div>
            </div>

            <div class="info-card">
              <div class="info-icon">🕐</div>
              <div>
                <div class="info-label">Program</div>
                <div class="info-value">
                  <div class="schedule-row"><span>Luni – Vineri</span><strong>08:00 – 20:00</strong></div>
                  <div class="schedule-row"><span>Sâmbătă</span><strong>09:00 – 16:00</strong></div>
                  <div class="schedule-row"><span>Duminică</span><strong>Urgențe doar</strong></div>
                </div>
              </div>
            </div>

            <div class="info-card">
              <div class="info-icon">🚨</div>
              <div>
                <div class="info-label">Urgențe non-stop</div>
                <div class="info-value">
                  <a href="tel:+40269999000" class="emergency">+40 269 999 000</a>
                </div>
              </div>
            </div>

            <!-- Social -->
            <div class="social-row">
              <a href="#" class="social-btn">📘 Facebook</a>
              <a href="#" class="social-btn">📸 Instagram</a>
            </div>

          </div>

          <!-- HARTA -->
          <div class="map-col">
            <div class="map-label">
              <span class="map-pin">📍</span>
              Strada Veterinarilor nr. 12, Sibiu
            </div>
            <div class="map-wrapper">
              <iframe
                title="Locație VetCare Sibiu"
                src="https://www.openstreetmap.org/export/embed.html?bbox=24.12,45.78,24.18,45.82&layer=mapnik&marker=45.80,24.15"
                width="100%"
                height="100%"
                style="border:0;"
                loading="lazy"
                allowfullscreen
              />
            </div>
            <a
              href="https://www.openstreetmap.org/?mlat=45.80&mlon=24.15#map=15/45.80/24.15"
              target="_blank"
              rel="noopener noreferrer"
              class="map-open-btn"
            >
              Deschide în Maps →
            </a>
          </div>

        </div>
      </div>
    </section>

    <!-- CTA -->
    <section class="cta-section">
      <div class="container">
        <div class="cta-box">
          <div class="cta-blob" />
          <h2>Programează o consultație</h2>
          <p>Creează un cont și programează vizita pentru animalul tău în câteva secunde.</p>
          <div class="cta-actions">
            <router-link to="/register" class="btn-primary">Creează cont gratuit</router-link>
            <router-link to="/login" class="btn-ghost-light">Intră în cont →</router-link>
          </div>
        </div>
      </div>
    </section>

  </div>
</template>

<style scoped>
.contact-page { overflow-x: hidden; }

/* ===== HERO ===== */
.hero {
  position: relative; overflow: hidden;
  padding: 80px 0 60px;
  background: linear-gradient(160deg, #fff0f8 0%, #ffffff 50%, #f0f9ff 100%);
  text-align: center;
}
.hero-bg { position: absolute; inset: 0; pointer-events: none; }
.blob { position: absolute; border-radius: 50%; filter: blur(80px); opacity: 0.3; }
.blob-1 { width: 400px; height: 400px; background: #f9a8d4; top: -120px; right: -100px; }
.blob-2 { width: 300px; height: 300px; background: #bfdbfe; bottom: -80px; left: -80px; }

.hero .container { position: relative; z-index: 1; }
.hero h1 {
  font-size: clamp(2.2rem, 3vw + 1rem, 3.4rem);
  font-weight: 900; color: #1f2937; margin-bottom: 14px;
  font-family: Georgia, serif;
}
.hero p { font-size: 1.05rem; color: #6b7280; max-width: 50ch; margin: 0 auto; line-height: 1.7; }

.pill {
  display: inline-block; background: #fdf2f8; color: #be185d;
  padding: 6px 16px; border-radius: 999px; font-size: 0.82rem;
  font-weight: 700; text-transform: uppercase; letter-spacing: 0.06em;
  border: 1px solid #fbcfe8; margin-bottom: 16px;
}

/* ===== CONTENT ===== */
.content-section { padding: 70px 0 80px; }

.content-grid {
  display: grid;
  grid-template-columns: 380px 1fr;
  gap: 36px;
  align-items: start;
}

/* Info col */
.info-col { display: grid; gap: 14px; }

.info-card {
  background: white; border-radius: 18px; padding: 18px 20px;
  display: flex; align-items: flex-start; gap: 16px;
  border: 1px solid #f3f4f6;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  transition: all 0.2s;
}
.info-card:hover { border-color: #fbcfe8; box-shadow: 0 6px 20px rgba(190,24,93,0.08); transform: translateY(-2px); }

.info-icon { font-size: 1.5rem; flex-shrink: 0; margin-top: 2px; }
.info-label { font-size: 0.78rem; font-weight: 700; color: #9ca3af; text-transform: uppercase; letter-spacing: 0.06em; margin-bottom: 4px; }
.info-value { font-size: 0.95rem; color: #1f2937; line-height: 1.6; }
.info-value a { color: #be185d; text-decoration: none; font-weight: 600; }
.info-value a:hover { text-decoration: underline; }
.emergency { color: #dc2626 !important; font-weight: 800 !important; font-size: 1.05rem !important; }

.schedule-row { display: flex; justify-content: space-between; gap: 12px; font-size: 0.88rem; padding: 3px 0; color: #4b5563; }
.schedule-row strong { color: #1f2937; }

.social-row { display: flex; gap: 10px; margin-top: 4px; }
.social-btn {
  flex: 1; text-align: center; text-decoration: none;
  background: #f9fafb; border: 1px solid #e5e7eb;
  border-radius: 12px; padding: 10px 14px;
  font-size: 0.88rem; font-weight: 600; color: #374151;
  transition: all 0.2s;
}
.social-btn:hover { background: #fdf2f8; border-color: #fbcfe8; color: #be185d; }

/* Map col */
.map-col { display: flex; flex-direction: column; gap: 12px; }

.map-label {
  display: flex; align-items: center; gap: 8px;
  font-size: 0.9rem; font-weight: 600; color: #374151;
}
.map-pin { font-size: 1.1rem; }

.map-wrapper {
  width: 100%; height: 420px; border-radius: 22px; overflow: hidden;
  border: 1px solid #e5e7eb;
  box-shadow: 0 8px 32px rgba(0,0,0,0.1);
}

.map-open-btn {
  display: inline-block; text-decoration: none; align-self: flex-end;
  font-size: 0.88rem; font-weight: 700; color: #be185d;
  transition: letter-spacing 0.2s;
}
.map-open-btn:hover { letter-spacing: 0.02em; }

/* ===== CTA ===== */
.cta-section { padding: 0 0 80px; }
.cta-box {
  position: relative; overflow: hidden;
  background: linear-gradient(135deg, #be185d 0%, #9d174d 50%, #7c1237 100%);
  border-radius: 32px; padding: 60px 48px; text-align: center;
  box-shadow: 0 30px 80px rgba(190,24,93,0.25);
}
.cta-blob { position: absolute; width: 350px; height: 350px; border-radius: 50%; background: rgba(255,255,255,0.05); top: -80px; right: -80px; pointer-events: none; }
.cta-box h2 { font-size: clamp(1.6rem, 2vw + 0.5rem, 2.4rem); font-weight: 900; color: white; margin-bottom: 12px; font-family: Georgia, serif; position: relative; }
.cta-box p { color: rgba(255,255,255,0.8); font-size: 1rem; max-width: 48ch; margin: 0 auto 28px; line-height: 1.7; position: relative; }
.cta-actions { display: flex; gap: 16px; justify-content: center; flex-wrap: wrap; position: relative; }

.btn-primary {
  display: inline-block; text-decoration: none;
  background: white; color: #be185d;
  padding: 14px 28px; border-radius: 14px;
  font-weight: 700; font-size: 1rem;
  box-shadow: 0 8px 20px rgba(0,0,0,0.15);
  transition: all 0.2s;
}
.btn-primary:hover { transform: translateY(-2px); box-shadow: 0 12px 28px rgba(0,0,0,0.2); }

.btn-ghost-light {
  display: inline-block; text-decoration: none;
  color: rgba(255,255,255,0.85); font-weight: 600; font-size: 1rem;
  padding: 14px 4px; transition: all 0.2s;
}
.btn-ghost-light:hover { color: white; }

/* ===== RESPONSIVE ===== */
@media (max-width: 900px) {
  .content-grid { grid-template-columns: 1fr; }
  .map-wrapper { height: 320px; }
  .map-open-btn { align-self: flex-start; }
}

@media (max-width: 600px) {
  .hero { padding: 60px 0 40px; }
  .cta-box { padding: 40px 24px; }
  .map-wrapper { height: 260px; }
}
</style>