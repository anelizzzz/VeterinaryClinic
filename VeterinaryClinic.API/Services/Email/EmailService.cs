using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace VeterinaryClinic.API.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAppointmentConfirmationAsync(
            string toEmail,
            string clientName,
            string petName,
            string doctorName,
            DateTime date,
            string appointmentType,
            string? notes,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:Username"] ?? "";
                var smtpPass = _configuration["Email:Password"] ?? "";
                var fromName = _configuration["Email:FromName"] ?? "VetCare Clinic";
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress(clientName, toEmail));
                message.Subject = $"✅ Confirmare programare – {petName} | VetCare";

                var body = BuildHtmlBody(clientName, petName, doctorName, date, appointmentType, notes);

                message.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(smtpUser, smtpPass, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation("Email confirmare trimis la {Email} pentru {Pet}", toEmail, petName);
            }
            catch (Exception ex)
            {
                // Nu blocăm crearea programării dacă emailul eșuează
                _logger.LogError(ex, "Eroare la trimiterea emailului de confirmare la {Email}", toEmail);
            }
        }

        private static string BuildHtmlBody(
            string clientName,
            string petName,
            string doctorName,
            DateTime date,
            string appointmentType,
            string? notes)
        {
            var dateStr = date.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ro-RO"));
            var timeStr = date.ToString("HH:mm");
            var notesSection = !string.IsNullOrWhiteSpace(notes)
                ? $@"<tr>
                       <td style='padding:8px 0;color:#6b7280;font-size:14px;'>Note</td>
                       <td style='padding:8px 0;font-weight:600;color:#1f2937;font-size:14px;'>{notes}</td>
                     </tr>"
                : "";

            return $@"
<!DOCTYPE html>
<html lang='ro'>
<head><meta charset='UTF-8'><meta name='viewport' content='width=device-width,initial-scale=1'></head>
<body style='margin:0;padding:0;background:#f9fafb;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f9fafb;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;'>

        <!-- HEADER -->
        <tr>
          <td style='background:linear-gradient(135deg,#be185d,#9d174d);border-radius:20px 20px 0 0;padding:36px 40px;text-align:center;'>
            <div style='font-size:36px;margin-bottom:8px;'>🐾</div>
            <h1 style='margin:0;color:white;font-size:26px;font-weight:800;letter-spacing:-0.5px;'>VetCare</h1>
            <p style='margin:4px 0 0;color:rgba(255,255,255,0.8);font-size:13px;text-transform:uppercase;letter-spacing:2px;'>Veterinary Clinic</p>
          </td>
        </tr>

        <!-- BODY -->
        <tr>
          <td style='background:white;padding:40px;'>
            <h2 style='margin:0 0 8px;color:#1f2937;font-size:22px;'>Programare confirmată ✅</h2>
            <p style='margin:0 0 28px;color:#6b7280;font-size:15px;line-height:1.6;'>
              Bună ziua, <strong style='color:#1f2937;'>{clientName}</strong>!<br>
              Programarea pentru <strong style='color:#be185d;'>{petName}</strong> a fost înregistrată cu succes.
              Mai jos găsești detaliile consultației.
            </p>

            <!-- DETAILS BOX -->
            <table width='100%' cellpadding='0' cellspacing='0'
                   style='background:#fdf2f8;border-radius:16px;padding:24px;border:1px solid #fbcfe8;margin-bottom:28px;'>
              <tr>
                <td style='padding:8px 0;color:#6b7280;font-size:14px;width:40%;'>📅 Data</td>
                <td style='padding:8px 0;font-weight:700;color:#1f2937;font-size:14px;'>{dateStr}</td>
              </tr>
              <tr>
                <td style='padding:8px 0;color:#6b7280;font-size:14px;border-top:1px solid #fbcfe8;'>🕐 Ora</td>
                <td style='padding:8px 0;font-weight:700;color:#1f2937;font-size:14px;border-top:1px solid #fbcfe8;'>{timeStr}</td>
              </tr>
              <tr>
                <td style='padding:8px 0;color:#6b7280;font-size:14px;border-top:1px solid #fbcfe8;'>🐾 Pacient</td>
                <td style='padding:8px 0;font-weight:700;color:#1f2937;font-size:14px;border-top:1px solid #fbcfe8;'>{petName}</td>
              </tr>
              <tr>
                <td style='padding:8px 0;color:#6b7280;font-size:14px;border-top:1px solid #fbcfe8;'>🩺 Doctor</td>
                <td style='padding:8px 0;font-weight:700;color:#1f2937;font-size:14px;border-top:1px solid #fbcfe8;'>{doctorName}</td>
              </tr>
              <tr>
                <td style='padding:8px 0;color:#6b7280;font-size:14px;border-top:1px solid #fbcfe8;'>📋 Tip</td>
                <td style='padding:8px 0;font-weight:700;color:#1f2937;font-size:14px;border-top:1px solid #fbcfe8;'>{appointmentType}</td>
              </tr>
              {notesSection}
            </table>

            <p style='margin:0 0 24px;color:#6b7280;font-size:14px;line-height:1.7;
                      background:#fef3c7;border-radius:12px;padding:16px;border-left:4px solid #f59e0b;'>
              ⚠️ Dacă nu poți ajunge la programare, te rugăm să ne anunți cu cel puțin 24 de ore înainte
              la <a href='tel:+40269123456' style='color:#be185d;font-weight:600;'>+40 269 123 456</a>
              sau prin email.
            </p>

            <p style='margin:0;color:#6b7280;font-size:14px;line-height:1.6;'>
              Ne vedem curând!<br>
              <strong style='color:#1f2937;'>Echipa VetCare 🐾</strong>
            </p>
          </td>
        </tr>

        <!-- FOOTER -->
        <tr>
          <td style='background:#f3f4f6;border-radius:0 0 20px 20px;padding:24px 40px;text-align:center;'>
            <p style='margin:0 0 8px;color:#9ca3af;font-size:12px;'>
              📍 Strada Veterinarilor nr. 12, Sibiu &nbsp;|&nbsp;
              📞 +40 269 123 456 &nbsp;|&nbsp;
              📧 contact@vetcare.ro
            </p>
            <p style='margin:0;color:#d1d5db;font-size:11px;'>
              © 2026 VetCare Veterinary Clinic. Toate drepturile rezervate.
            </p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";
        }

        // ── HELPER ──────────────────────────────────────────────────────────
        private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:Username"] ?? "";
                var smtpPass = _configuration["Email:Password"] ?? "";
                var fromName = _configuration["Email:FromName"] ?? "VetCare Clinic";
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(smtpUser, smtpPass, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la trimiterea emailului la {Email}", toEmail);
            }
        }

        // ── CONT ÎN AȘTEPTARE ────────────────────────────────────────────────
        public async Task SendAccountPendingAsync(string toEmail, string userName, string role, CancellationToken cancellationToken = default)
        {
            var subject = "⏳ Cererea ta de cont VetCare a fost primită";
            var body = $@"
<!DOCTYPE html><html lang='ro'><head><meta charset='UTF-8'></head>
<body style='margin:0;padding:0;background:#f9fafb;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f9fafb;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;'>
        <tr><td style='background:linear-gradient(135deg,#f59e0b,#d97706);border-radius:20px 20px 0 0;padding:36px 40px;text-align:center;'>
          <div style='font-size:36px;margin-bottom:8px;'>⏳</div>
          <h1 style='margin:0;color:white;font-size:24px;font-weight:800;'>Cerere primită!</h1>
        </td></tr>
        <tr><td style='background:white;padding:40px;border-radius:0 0 20px 20px;'>
          <p style='color:#1f2937;font-size:16px;'>Bună ziua, <strong>{userName}</strong>!</p>
          <p style='color:#4b5563;line-height:1.7;'>Cererea ta de creare cont ca <strong>{role}</strong> la VetCare a fost primită și urmează să fie analizată de administratorul clinicii.</p>
          <div style='background:#fef3c7;border-radius:14px;padding:20px;margin:24px 0;border-left:4px solid #f59e0b;'>
            <p style='margin:0;color:#92400e;font-size:14px;'>🕐 Vei primi un email de confirmare imediat după ce contul tău a fost aprobat. De obicei procesul durează câteva ore.</p>
          </div>
          <p style='color:#6b7280;font-size:14px;'>Dacă ai întrebări, ne poți contacta la <a href='mailto:contact@vetcare.ro' style='color:#be185d;'>contact@vetcare.ro</a>.</p>
          <p style='color:#1f2937;'>Cu drag,<br><strong>Echipa VetCare 🐾</strong></p>
        </td></tr>
        <tr><td style='background:#f3f4f6;border-radius:0 0 20px 20px;padding:20px 40px;text-align:center;'>
          <p style='margin:0;color:#9ca3af;font-size:12px;'>© 2026 VetCare Veterinary Clinic</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";
            await SendEmailAsync(toEmail, userName, subject, body, cancellationToken);
        }

        // ── CERERE NOUĂ PENTRU ADMIN ─────────────────────────────────────────
        public async Task SendNewAccountRequestToAdminAsync(string adminEmail, string userName, string userEmail, string role, int userId, CancellationToken cancellationToken = default)
        {
            var subject = $"🔔 Cerere cont nou: {userName} ({role})";
            var body = $@"
<!DOCTYPE html><html lang='ro'><head><meta charset='UTF-8'></head>
<body style='margin:0;padding:0;background:#f9fafb;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f9fafb;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;'>
        <tr><td style='background:linear-gradient(135deg,#7c3aed,#5b21b6);border-radius:20px 20px 0 0;padding:36px 40px;text-align:center;'>
          <div style='font-size:36px;margin-bottom:8px;'>🔔</div>
          <h1 style='margin:0;color:white;font-size:24px;font-weight:800;'>Cerere cont nou</h1>
        </td></tr>
        <tr><td style='background:white;padding:40px;border-radius:0 0 20px 20px;'>
          <p style='color:#1f2937;font-size:16px;'>Un utilizator nou așteaptă aprobare:</p>
          <table width='100%' style='background:#f9fafb;border-radius:14px;padding:20px;border:1px solid #e5e7eb;'>
            <tr><td style='padding:8px 0;color:#6b7280;font-size:14px;width:40%;'>👤 Nume</td><td style='padding:8px 0;font-weight:700;color:#1f2937;'>{userName}</td></tr>
            <tr><td style='padding:8px 0;color:#6b7280;font-size:14px;border-top:1px solid #e5e7eb;'>📧 Email</td><td style='padding:8px 0;font-weight:700;color:#1f2937;border-top:1px solid #e5e7eb;'>{userEmail}</td></tr>
            <tr><td style='padding:8px 0;color:#6b7280;font-size:14px;border-top:1px solid #e5e7eb;'>🏷️ Rol</td><td style='padding:8px 0;font-weight:700;color:#1f2937;border-top:1px solid #e5e7eb;'>{role}</td></tr>
            <tr><td style='padding:8px 0;color:#6b7280;font-size:14px;border-top:1px solid #e5e7eb;'>🔑 ID</td><td style='padding:8px 0;font-weight:700;color:#1f2937;border-top:1px solid #e5e7eb;'>#{userId}</td></tr>
          </table>
          <p style='color:#4b5563;margin-top:24px;'>Intră în panoul de administrare pentru a aproba sau respinge cererea.</p>
          <p style='color:#1f2937;'>Echipa VetCare 🐾</p>
        </td></tr>
        <tr><td style='background:#f3f4f6;padding:20px 40px;text-align:center;'>
          <p style='margin:0;color:#9ca3af;font-size:12px;'>© 2026 VetCare Veterinary Clinic</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";
            await SendEmailAsync(adminEmail, "Administrator", subject, body, cancellationToken);
        }

        // ── CONT APROBAT ─────────────────────────────────────────────────────
        public async Task SendAccountApprovedAsync(string toEmail, string userName, string role, CancellationToken cancellationToken = default)
        {
            var subject = "✅ Contul tău VetCare a fost aprobat!";
            var body = $@"
<!DOCTYPE html><html lang='ro'><head><meta charset='UTF-8'></head>
<body style='margin:0;padding:0;background:#f9fafb;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f9fafb;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;'>
        <tr><td style='background:linear-gradient(135deg,#10b981,#059669);border-radius:20px 20px 0 0;padding:36px 40px;text-align:center;'>
          <div style='font-size:36px;margin-bottom:8px;'>✅</div>
          <h1 style='margin:0;color:white;font-size:24px;font-weight:800;'>Cont aprobat!</h1>
        </td></tr>
        <tr><td style='background:white;padding:40px;border-radius:0 0 20px 20px;'>
          <p style='color:#1f2937;font-size:16px;'>Bună ziua, <strong>{userName}</strong>!</p>
          <p style='color:#4b5563;line-height:1.7;'>Îți facem plăcere să te informăm că cererea ta de cont ca <strong>{role}</strong> la VetCare a fost <strong style='color:#10b981;'>aprobată</strong>!</p>
          <div style='background:#d1fae5;border-radius:14px;padding:20px;margin:24px 0;border-left:4px solid #10b981;'>
            <p style='margin:0;color:#065f46;font-size:14px;'>🎉 Poți să te autentifici acum folosind emailul și parola setate la înregistrare.</p>
          </div>
          <div style='text-align:center;margin:28px 0;'>
            <a href='http://localhost:5173/login' style='background:linear-gradient(135deg,#be185d,#9d174d);color:white;padding:14px 32px;border-radius:14px;text-decoration:none;font-weight:700;font-size:16px;'>Intră în cont →</a>
          </div>
          <p style='color:#6b7280;font-size:14px;'>Bine ai venit în familia VetCare! 🐾</p>
          <p style='color:#1f2937;'>Cu drag,<br><strong>Echipa VetCare</strong></p>
        </td></tr>
        <tr><td style='background:#f3f4f6;padding:20px 40px;text-align:center;'>
          <p style='margin:0;color:#9ca3af;font-size:12px;'>© 2026 VetCare Veterinary Clinic</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";
            await SendEmailAsync(toEmail, userName, subject, body, cancellationToken);
        }

        // ── CONT RESPINS ─────────────────────────────────────────────────────
        public async Task SendAccountRejectedAsync(string toEmail, string userName, CancellationToken cancellationToken = default)
        {
            var subject = "❌ Cererea ta de cont VetCare nu a fost aprobată";
            var body = $@"
<!DOCTYPE html><html lang='ro'><head><meta charset='UTF-8'></head>
<body style='margin:0;padding:0;background:#f9fafb;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f9fafb;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;'>
        <tr><td style='background:linear-gradient(135deg,#ef4444,#dc2626);border-radius:20px 20px 0 0;padding:36px 40px;text-align:center;'>
          <div style='font-size:36px;margin-bottom:8px;'>❌</div>
          <h1 style='margin:0;color:white;font-size:24px;font-weight:800;'>Cerere respinsă</h1>
        </td></tr>
        <tr><td style='background:white;padding:40px;border-radius:0 0 20px 20px;'>
          <p style='color:#1f2937;font-size:16px;'>Bună ziua, <strong>{userName}</strong>!</p>
          <p style='color:#4b5563;line-height:1.7;'>Ne pare rău să te informăm că cererea ta de creare cont la VetCare nu a putut fi aprobată la acest moment.</p>
          <p style='color:#4b5563;line-height:1.7;'>Pentru mai multe detalii sau pentru a face o nouă cerere, te rugăm să ne contactezi la <a href='mailto:contact@vetcare.ro' style='color:#be185d;'>contact@vetcare.ro</a>.</p>
          <p style='color:#1f2937;'>Cu respect,<br><strong>Echipa VetCare 🐾</strong></p>
        </td></tr>
        <tr><td style='background:#f3f4f6;padding:20px 40px;text-align:center;'>
          <p style='margin:0;color:#9ca3af;font-size:12px;'>© 2026 VetCare Veterinary Clinic</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";
            await SendEmailAsync(toEmail, userName, subject, body, cancellationToken);
        }
    }
}