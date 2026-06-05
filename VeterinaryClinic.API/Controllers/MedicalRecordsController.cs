using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.MedicalRecord;
using VeterinaryClinic.API.Services;
using VeterinaryClinic.API.Services.Pdf;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/MedicalRecords")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalRecordService _medicalRecordService;
        private readonly IPdfService _pdfService;

        public MedicalRecordController(MedicalRecordService medicalRecordService, IPdfService pdfService)
        {
            _medicalRecordService = medicalRecordService;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _medicalRecordService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _medicalRecordService.GetByIdAsync(id);
            return record is null ? NotFound() : Ok(record);
        }

        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetByPetId(int petId) =>
            Ok(await _medicalRecordService.GetByPetIdAsync(petId));

        [HttpGet("by-doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId(int doctorId) =>
            Ok(await _medicalRecordService.GetByDoctorIdAsync(doctorId));

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var record = await _medicalRecordService.GetByIdAsync(id);
            if (record is null) return NotFound();

            var pdfBytes = _pdfService.GenerateMedicalRecordPdf(record);
            var fileName = $"fisa-medicala-{record.PetName}-{record.Date:yyyy-MM-dd}.pdf"
                .Replace(" ", "-").ToLower();

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicalRecordCreateDto dto)
        {
            var record = await _medicalRecordService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicalRecordUpdateDto dto)
        {
            var record = await _medicalRecordService.UpdateAsync(id, dto);
            return record is null ? NotFound() : Ok(record);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _medicalRecordService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}