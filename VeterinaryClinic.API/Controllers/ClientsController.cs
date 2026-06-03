using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.Dtos.Client;
using VeterinaryClinic.API.Services;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly ClientService _clientService;

        public ClientsController(ClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client is null) return NotFound();
            return Ok(client);
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return BadRequest("User ID not found in token.");

            var userId = int.Parse(userIdClaim);

            var client = await _clientService.GetByUserIdAsync(userId);
            if (client is null) return NotFound();

            return Ok(client);
        }

        [HttpPut("profile")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdateProfile(ClientUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return BadRequest("User ID not found in token.");

            var userId = int.Parse(userIdClaim);

            var result = await _clientService.UpdateAsync(userId, dto);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Client")]
        public async Task<IActionResult> Update(int id, ClientUpdateDto dto)
        {
            var result = await _clientService.UpdateAsync(id, dto);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _clientService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}