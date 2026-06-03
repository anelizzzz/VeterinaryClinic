namespace VeterinaryClinic.API.Dtos.Pet;

public class PetCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public string Vaccines { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int ClientId { get; set; }
}
