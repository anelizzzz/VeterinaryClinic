using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.DTOs.AdoptionAnimal
{
    public class AdoptionAnimalResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Vaccines { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public AdoptionStatus Status { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
