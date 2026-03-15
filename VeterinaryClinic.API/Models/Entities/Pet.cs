using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Models.Entities;

public class Pet
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public PetStatus Status { get; set; }
    public string Vaccines { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
