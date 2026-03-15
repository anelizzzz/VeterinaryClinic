using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Models.Entities;

public class Appointment
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public AppointmentType Type { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;
}
