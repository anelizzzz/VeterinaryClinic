using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VeterinaryClinic.API.DTOs.AdoptionRequest;
using VeterinaryClinic.API.Models.Entities.Enums;
using VeterinaryClinic.API.Services;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdoptionRequestsController : ControllerBase
    {
        private readonly AdoptionRequestService _adoptionRequestService;

        public AdoptionRequestsController(AdoptionRequestService adoptionRequestService)
        {
            _adoptionRequestService = adoptionRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _adoptionRequestService.GetAllAsync();
            return Ok(requests);
        }
        [HttpGet("my")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Nu s-a găsit user id în token.");

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User id invalid în token.");

            var requests = await _adoptionRequestService.GetByUserIdAsync(userId);
            return Ok(requests);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _adoptionRequestService.GetByIdAsync(id);
            if (request is null) return NotFound();
            return Ok(request);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClientId(int clientId)
        {
            var requests = await _adoptionRequestService.GetByClientIdAsync(clientId);
            return Ok(requests);
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdoptionRequestCreateDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Nu s-a găsit user id în token.");

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("User id invalid în token.");

            var result = await _adoptionRequestService.CreateAsync(userId, dto);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAdoptionRequestStatusDto dto)
        {
            var result = await _adoptionRequestService.UpdateStatusAsync(id, dto.Status);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adoptionRequestService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}