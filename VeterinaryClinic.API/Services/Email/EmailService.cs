using System.Text;
using System.Text.Json;

namespace VeterinaryClinic.API.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task SendAppointmentConfirmationAsync(
            string toEmail, string clientName, string petName, string doctorName,
            DateTime date, string appointmentType, string? notes,
            CancellationToken cancellationToken = default)
        {
            var subject = $"✅ Confirmare programare – {petName} | VetCare";
            var body = BuildAppointmentHtmlBody(clientName, petName, doctorName, date, appointmentType, notes);
            await SendEmailAsync(toEmail, clientName, subject, body, cancellationToken);
        }

        public async Task SendAccountPendingAsync(
            string toEmail, string userName, string role,
            CancellationToken cancellationToken = default)
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
        <tr><td style='background:white;padding:40px;'>
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

        public async Task SendNewAccountRequestToAdminAsync(
            string adminEmail, string userName, string userEmail,
            string role, int userId,
            CancellationToken cancellationToken = default)
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
        <tr><td style='background:white;padding:40px;'>
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
        public async Task SendAccountApprovedAsync(
            string toEmail, string userName, string role,
            CancellationToken cancellationToken = default)
        {
            var subject = "✅ Contul tău VetCare a fost aprobat!";
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "https://veterinary-clinic-red.vercel.app";
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
        <tr><td style='background:white;padding:40px;'>
          <p style='color:#1f2937;font-size:16px;'>Bună ziua, <strong>{userName}</strong>!</p>
          <p style='color:#4b5563;line-height:1.7;'>Cererea ta de cont ca <strong>{role}</strong> la VetCare a fost <strong style='color:#10b981;'>aprobată</strong>!</p>
          <div style='background:#d1fae5;border-radius:14px;padding:20px;margin:24px 0;border-left:4px solid #10b981;'>
            <p style='margin:0;color:#065f46;font-size:14px;'>🎉 Poți să te autentifici acum folosind emailul și parola setate la înregistrare.</p>
          </div>
          <div style='text-align:center;margin:28px 0;'>
            <a href='{frontendUrl}/login' style='background:linear-gradient(135deg,#be185d,#9d174d);color:white;padding:14px 32px;border-radius:14px;text-decoration:none;font-weight:700;font-size:16px;'>Intră în cont →</a>
          </div>
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

        public async Task SendAccountRejectedAsync(
            string toEmail, string userName,
            CancellationToken cancellationToken = default)
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
        <tr><td style='background:white;padding:40px;'>
          <p style='color:#1f2937;font-size:16px;'>Bună ziua, <strong>{userName}</strong>!</p>
          <p style='color:#4b5563;line-height:1.7;'>Ne pare rău să te informăm că cererea ta de creare cont la VetCare nu a putut fi aprobată la acest moment.</p>
          <p style='color:#4b5563;line-height:1.7;'>Pentru mai multe detalii, ne poți contacta la <a href='mailto:contact@vetcare.ro' style='color:#be185d;'>contact@vetcare.ro</a>.</p>
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

        private async Task SendEmailAsync(
            string toEmail, string toName, string subject, string htmlBody,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiKey = _configuration["Email:BrevoApiKey"] ?? "";
                var fromEmail = _configuration["Email:FromEmail"] ?? "";
                var fromName = _configuration["Email:FromName"] ?? "VetCare Clinic";

                var payload = new
                {
                    sender = new { name = fromName, email = fromEmail },
                    to = new[] { new { email = toEmail, name = toName } },
                    subject = subject,
                    htmlContent = htmlBody
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
                request.Headers.Add("api-key", apiKey);
                request.Content = content;

                var response = await _httpClient.SendAsync(request, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Email trimis cu succes la {Email}", toEmail);
                else
                    _logger.LogError("Brevo API error la {Email}: {Status} - {Body}", toEmail, response.StatusCode, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EMAIL ERROR la {Email}: {Message}", toEmail, ex.Message);
                throw;
            }
        }

        private static string BuildAppointmentHtmlBody(
            string clientName, string petName, string doctorName,
            DateTime date, string appointmentType, string? notes)
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
<!DOCTYPE html><html lang='ro'><head><meta charset='UTF-8'></head>
<body style='margin:0;padding:0;background:#f9fafb;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f9fafb;padding:40px 20px;'>
    <tr><td align='center'>
      <table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;'>
        <tr><td style='background:linear-gradient(135deg,#be185d,#9d174d);border-radius:20px 20px 0 0;padding:36px 40px;text-align:center;'>
          <div style='font-size:36px;margin-bottom:8px;'>🐾</div>
          <h1 style='margin:0;color:white;font-size:26px;font-weight:800;'>VetCare</h1>
          <p style='margin:4px 0 0;color:rgba(255,255,255,0.8);font-size:13px;text-transform:uppercase;letter-spacing:2px;'>Veterinary Clinic</p>
        </td></tr>
        <tr><td style='background:white;padding:40px;'>
          <h2 style='margin:0 0 8px;color:#1f2937;font-size:22px;'>Programare confirmată ✅</h2>
          <p style='margin:0 0 28px;color:#6b7280;font-size:15px;line-height:1.6;'>
            Bună ziua, <strong style='color:#1f2937;'>{clientName}</strong>!<br>
            Programarea pentru <strong style='color:#be185d;'>{petName}</strong> a fost înregistrată cu succes.
          </p>
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
          <p style='margin:0;color:#6b7280;font-size:14px;line-height:1.6;'>
            Ne vedem curând!<br>
            <strong style='color:#1f2937;'>Echipa VetCare 🐾</strong>
          </p>
        </td></tr>
        <tr><td style='background:#f3f4f6;border-radius:0 0 20px 20px;padding:24px 40px;text-align:center;'>
          <p style='margin:0;color:#9ca3af;font-size:12px;'>📍 Strada Veterinarilor nr. 12, Sibiu | 📧 contact@vetcare.ro</p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body></html>";
        }
    }
}