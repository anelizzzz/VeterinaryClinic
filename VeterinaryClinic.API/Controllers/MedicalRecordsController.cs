using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.MedicalRecord;
using VeterinaryClinic.API.Services;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly MedicalRecordService _medicalRecordService;

        public MedicalRecordsController(MedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAll()
        {
            var records = await _medicalRecordService.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _medicalRecordService.GetByIdAsync(id);
            if (record is null) return NotFound();
            return Ok(record);
        }

        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetByPetId(int petId)
        {
            var records = await _medicalRecordService.GetByPetIdAsync(petId);
            return Ok(records);
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByDoctorId(int doctorId)
        {
            var records = await _medicalRecordService.GetByDoctorIdAsync(doctorId);
            return Ok(records);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create(MedicalRecordCreateDto dto)
        {
            var result = await _medicalRecordService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, MedicalRecordUpdateDto dto)
        {
            var result = await _medicalRecordService.UpdateAsync(id, dto);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _medicalRecordService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}