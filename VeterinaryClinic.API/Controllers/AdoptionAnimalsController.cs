using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.AdoptionAnimal;
using VeterinaryClinic.API.Services;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdoptionAnimalsController : ControllerBase
    {
        private readonly AdoptionAnimalService _adoptionAnimalService;

        public AdoptionAnimalsController(AdoptionAnimalService adoptionAnimalService)
        {
            _adoptionAnimalService = adoptionAnimalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animals = await _adoptionAnimalService.GetAllAsync();
            return Ok(animals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var animal = await _adoptionAnimalService.GetByIdAsync(id);
            if (animal is null) return NotFound();
            return Ok(animal);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AdoptionAnimalCreateDto dto)
        {
            var result = await _adoptionAnimalService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, AdoptionAnimalCreateDto dto)
        {
            var result = await _adoptionAnimalService.UpdateAsync(id, dto);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adoptionAnimalService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}