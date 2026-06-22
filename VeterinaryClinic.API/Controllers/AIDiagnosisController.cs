using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.AiDiagnosis;
using VeterinaryClinic.API.Services.AiDiagnosis;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIDiagnosisController : ControllerBase
    {
        private readonly IAIDiagnosisService _aiDiagnosisService;
        private readonly ILogger<AIDiagnosisController> _logger;

        public AIDiagnosisController(IAIDiagnosisService aiDiagnosisService, ILogger<AIDiagnosisController> logger)
        {
            _aiDiagnosisService = aiDiagnosisService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Generate([FromBody] AIDiagnosisRequestDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _aiDiagnosisService.GenerateAsync(dto, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Diagnosis failed");
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}