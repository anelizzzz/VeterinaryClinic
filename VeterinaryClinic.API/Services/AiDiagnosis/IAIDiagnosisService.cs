using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.API.DTOs.AiDiagnosis;

namespace VeterinaryClinic.API.Services.AiDiagnosis
{
    public interface IAIDiagnosisService
    {
        Task<AIDiagnosisResponseDto> GenerateAsync(AIDiagnosisRequestDto dto, CancellationToken cancellationToken = default);
    }
}
