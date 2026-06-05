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
    }
}