namespace VeterinaryClinic.API.DTOs.MedicalRecord
{
    public class MedicalRecordCreateDto
    {
        public int PetId { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
    }
}
