using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Models.Entities;

public class AdoptionRequest
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int AdoptionAnimalId { get; set; }
    public AdoptionAnimal AdoptionAnimal { get; set; } = null!;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public AdoptionStatus Status { get; set; }
    public string Motivation { get; set; } = string.Empty;  
}
