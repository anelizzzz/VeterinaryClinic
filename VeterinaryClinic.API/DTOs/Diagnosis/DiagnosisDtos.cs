namespace VeterinaryClinic.API.DTOs.Diagnosis
{
    public class DiagnosisCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
    }

    public class DiagnosisUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
    }

    public class DiagnosisResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
    }
}