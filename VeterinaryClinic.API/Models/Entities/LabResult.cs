using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Models.Entities;

public class LabResult
{
    public int Id { get; set; }
    public TestType TestType { get; set; } 
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string FilePath { get; set; } = string.Empty;
    public string KeyValues { get; set; } = "{}";
    public string Interpretation { get; set; } = string.Empty;
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

}
