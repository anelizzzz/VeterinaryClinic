using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.DTOs.AdoptionRequest
{
    public class AdoptionRequestResponseDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string AnimalName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public AdoptionStatus Status { get; set; }
        public string Motivation { get; set; } = string.Empty;
    }
}
