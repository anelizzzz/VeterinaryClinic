using VeterinaryClinic.API.DTOs.LabResult;
using VeterinaryClinic.API.DTOs.MedicalRecord;

namespace VeterinaryClinic.API.Services.Pdf
{
    public interface IPdfService
    {
        byte[] GenerateMedicalRecordPdf(MedicalRecordResponseDto record);
        byte[] GenerateLabResultPdf(LabResultResponseDto result);
    }
}