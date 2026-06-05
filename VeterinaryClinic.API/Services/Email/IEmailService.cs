namespace VeterinaryClinic.API.Services.Email
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationAsync(
            string toEmail,
            string clientName,
            string petName,
            string doctorName,
            DateTime date,
            string appointmentType,
            string? notes,
            CancellationToken cancellationToken = default);
    }
}