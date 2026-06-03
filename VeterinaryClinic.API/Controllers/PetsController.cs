using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.Dtos.Pet;
using VeterinaryClinic.API.Services;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly PetService _petService;

        public PetsController(PetService petService)
        {
            _petService = petService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet is null) return NotFound();
            return Ok(pet);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClientId(int clientId)
        {
            var pets = await _petService.GetByClientIdAsync(clientId);
            return Ok(pets);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PetCreateDto dto)
        {
            var result = await _petService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PetUpdateDto dto)
        {
            var result = await _petService.UpdateAsync(id, dto);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _petService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/image")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdatePetImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nu a fost selectat niciun fișier.");

            var result = await _petService.UpdatePetImageAsync(id, file);
            if (!result) return NotFound();

            return Ok(new { message = "Poza a fost actualizată." });
        }
    }
}