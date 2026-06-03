using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Dtos.Appointment;

public class AppointmentUpdateDto
{
    public DateTime Date { get; set; }
    public AppointmentStatus Status { get; set; }
    public AppointmentType Type { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int DoctorId { get; set; }
}
