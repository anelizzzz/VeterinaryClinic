using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Models.Entities;

public class AdoptionAnimal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public int Age { get; set; } // in luni
    public string Description { get; set; } = string.Empty;
    public string Vaccines { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public AdoptionStatus Status { get; set; }
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    public ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();
}
