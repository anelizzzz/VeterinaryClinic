namespace VeterinaryClinic.API.DTOs.AdoptionAnimal
{
    public class AdoptionAnimalCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Vaccines { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
