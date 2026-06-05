namespace VeterinaryClinic.API.Models.Entities;

public class Diagnosis
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Observations { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty; // ex: "Câine", "Pisică", "" = toate
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}