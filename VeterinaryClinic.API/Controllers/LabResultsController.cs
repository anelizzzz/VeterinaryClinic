using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.LabResult;
using VeterinaryClinic.API.Services;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LabResultsController : ControllerBase
    {
        private readonly LabResultService _labResultService;

        public LabResultsController(LabResultService labResultService)
        {
            _labResultService = labResultService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _labResultService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _labResultService.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetByPetId(int petId)
        {
            var results = await _labResultService.GetByPetIdAsync(petId);
            return Ok(results);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create(LabResultCreateDto dto)
        {
            var result = await _labResultService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, LabResultCreateDto dto)
        {
            var result = await _labResultService.UpdateAsync(id, dto);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _labResultService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}