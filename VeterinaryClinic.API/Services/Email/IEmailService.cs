namespace VeterinaryClinic.API.Services.Email
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationAsync(
            string toEmail, string clientName, string petName, string doctorName,
            DateTime date, string appointmentType, string? notes,
            CancellationToken cancellationToken = default);

        Task SendAccountPendingAsync(
            string toEmail, string userName, string role,
            CancellationToken cancellationToken = default);

        Task SendNewAccountRequestToAdminAsync(
            string adminEmail, string userName, string userEmail,
            string role, int userId,
            CancellationToken cancellationToken = default);

        Task SendAccountApprovedAsync(
            string toEmail, string userName, string role,
            CancellationToken cancellationToken = default);

        Task SendAccountRejectedAsync(
            string toEmail, string userName,
            CancellationToken cancellationToken = default);
    }
}