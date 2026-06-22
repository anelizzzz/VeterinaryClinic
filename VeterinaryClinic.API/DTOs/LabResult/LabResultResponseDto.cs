using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.DTOs.LabResult
{
    public class LabResultResponseDto
    {
        public int Id { get; set; }
        public TestType TestType { get; set; }
        public DateTime Date { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string KeyValues { get; set; } = "{}";
        public string Interpretation { get; set; } = string.Empty;

        public string PetName { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int AgeYears { get; set; }
        public string Vaccines { get; set; } = string.Empty;
    }
}