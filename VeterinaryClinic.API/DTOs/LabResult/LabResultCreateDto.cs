using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.DTOs.LabResult
{
    public class LabResultCreateDto
    {
        public TestType TestType { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string KeyValues { get; set; } = "{}";
        public string Interpretation { get; set; } = string.Empty;
        public int PetId { get; set; }
    }
}
