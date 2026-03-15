namespace VeterinaryClinic.API.Models.Entities;

public class Doctor
{
    public int Id { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Bio { get; set; } = string.Empty;
    public string Schedule { get; set; } = "[]";
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
