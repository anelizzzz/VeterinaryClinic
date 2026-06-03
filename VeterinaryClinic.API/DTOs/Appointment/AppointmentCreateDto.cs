using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Dtos.Appointment;

public class AppointmentCreateDto
{
    public DateTime Date { get; set; }
    public AppointmentType Type { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int DoctorId { get; set; }
    public int PetId { get; set; }
}
