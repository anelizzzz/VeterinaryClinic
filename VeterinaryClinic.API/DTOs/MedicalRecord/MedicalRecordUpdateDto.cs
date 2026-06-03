namespace VeterinaryClinic.API.DTOs.MedicalRecord
{
    public class MedicalRecordUpdateDto
    {
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
    }
}
