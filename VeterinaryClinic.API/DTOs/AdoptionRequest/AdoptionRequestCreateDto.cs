namespace VeterinaryClinic.API.DTOs.AdoptionRequest
{
    public class AdoptionRequestCreateDto
    {
        public int AdoptionAnimalId { get; set; }
        public string Motivation { get; set; } = string.Empty;
    }
}
