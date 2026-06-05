using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.Diagnosis;
using VeterinaryClinic.API.Services;
using VeterinaryClinic.API.Services.Diagnosis;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosisController : ControllerBase
    {
        private readonly DiagnosisService _diagnosisService;

        public DiagnosisController(DiagnosisService diagnosisService)
        {
            _diagnosisService = diagnosisService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _diagnosisService.GetAllAsync());

        [HttpGet("by-species/{species}")]
        public async Task<IActionResult> GetBySpecies(string species) =>
            Ok(await _diagnosisService.GetBySpeciesAsync(species));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var d = await _diagnosisService.GetByIdAsync(id);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiagnosisCreateDto dto)
        {
            var d = await _diagnosisService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = d.Id }, d);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DiagnosisUpdateDto dto)
        {
            var d = await _diagnosisService.UpdateAsync(id, dto);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _diagnosisService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}