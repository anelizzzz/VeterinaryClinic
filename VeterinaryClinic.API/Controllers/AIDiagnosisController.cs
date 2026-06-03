using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeterinaryClinic.API.DTOs.AiDiagnosis;
using VeterinaryClinic.API.Services.AiDiagnosis;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIDiagnosisController : ControllerBase

    {
        private readonly IAIDiagnosisService _aiDiagnosisService;

        public AIDiagnosisController(IAIDiagnosisService aiDiagnosisService)
        {
            _aiDiagnosisService = aiDiagnosisService;
        }

        [HttpPost]
        public async Task<IActionResult> Generate([FromBody] AIDiagnosisRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await _aiDiagnosisService.GenerateAsync(dto, cancellationToken);
            return Ok(result);
        }
    }
}
