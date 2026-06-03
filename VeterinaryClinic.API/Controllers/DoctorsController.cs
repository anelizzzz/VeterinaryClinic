using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.Dtos.Doctor;
using VeterinaryClinic.API.Services.Doctor;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor is null) return NotFound();
            return Ok(doctor);
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return BadRequest("User ID not found in token.");

            var userId = int.Parse(userIdClaim);

            var doctor = await _doctorService.GetByUserIdAsync(userId);
            if (doctor is null) return NotFound();

            return Ok(doctor);
        }

        [HttpGet("my-patients")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyPatients()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return BadRequest("User ID not found in token.");

            var userId = int.Parse(userIdClaim);

            var patients = await _doctorService.GetMyPatientsAsync(userId);
            return Ok(patients);
        }
        [HttpGet("patients/{petId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetPatientDetails(int petId)
        {
            var patient = await _doctorService.GetPatientDetailsAsync(petId);
            if (patient is null)
                return NotFound();

            return Ok(patient);
        }
        [HttpPut("profile")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateProfile(DoctorUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return BadRequest("User ID not found in token.");

            var userId = int.Parse(userIdClaim);

            var result = await _doctorService.UpdateByUserIdAsync(userId, dto);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, DoctorUpdateDto dto)
        {
            var result = await _doctorService.UpdateAsync(id, dto);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _doctorService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}