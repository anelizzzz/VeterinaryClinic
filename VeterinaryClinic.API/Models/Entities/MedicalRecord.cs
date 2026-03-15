namespace VeterinaryClinic.API.Models.Entities;

public class MedicalRecord
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public string Diagnosis { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Observations { get; set; } = string.Empty;
}
