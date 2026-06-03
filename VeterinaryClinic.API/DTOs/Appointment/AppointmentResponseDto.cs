using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Dtos.Appointment;

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public AppointmentStatus Status { get; set; }
    public AppointmentType Type { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string PetName { get; set; } = string.Empty;
}
