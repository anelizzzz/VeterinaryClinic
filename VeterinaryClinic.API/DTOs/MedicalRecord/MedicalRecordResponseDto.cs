namespace VeterinaryClinic.API.DTOs.MedicalRecord
{
    public class MedicalRecordResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
    }
}
