using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.LabResult;
using VeterinaryClinic.API.Services;
using VeterinaryClinic.API.Services.Pdf;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/LabResults")]
    public class LabResultController : ControllerBase
    {
        private readonly LabResultService _labResultService;
        private readonly IPdfService _pdfService;

        public LabResultController(LabResultService labResultService, IPdfService pdfService)
        {
            _labResultService = labResultService;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _labResultService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _labResultService.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetByPetId(int petId) =>
            Ok(await _labResultService.GetByPetIdAsync(petId));

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var result = await _labResultService.GetByIdAsync(id);
            if (result is null) return NotFound();

            var pdfBytes = _pdfService.GenerateLabResultPdf(result);
            var fileName = $"rezultat-laborator-{result.PetName}-{result.Date:yyyy-MM-dd}.pdf"
                .Replace(" ", "-").ToLower();

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabResultCreateDto dto)
        {
            var result = await _labResultService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LabResultCreateDto dto)
        {
            var result = await _labResultService.UpdateAsync(id, dto);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _labResultService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}