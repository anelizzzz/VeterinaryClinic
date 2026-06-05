using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.Services.Email;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Trimite un email de test pentru a verifica configurația SMTP.
        /// ȘTERGE acest controller după testare!
        /// </summary>
        [HttpPost("send-test")]
        public async Task<IActionResult> SendTest([FromQuery] string toEmail)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest("Adresa de email este obligatorie.");

            await _emailService.SendAppointmentConfirmationAsync(
                toEmail: toEmail,
                clientName: "Test Client",
                petName: "Mimi",
                doctorName: "Dr. Ionescu",
                date: DateTime.Now.AddDays(2),
                appointmentType: "Control",
                notes: "Acesta este un email de test din VetCare."
            );

            return Ok(new { message = $"Email de test trimis la {toEmail}. Verifică inbox-ul!" });
        }
    }
}