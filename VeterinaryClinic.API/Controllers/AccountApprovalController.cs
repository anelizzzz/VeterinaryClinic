using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Models.Entities.Enums;
using VeterinaryClinic.API.Services;
using VeterinaryClinic.API.Services.Email;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApprovalController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AccountApprovalController(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var pending = users
                .Where(u => !u.IsApproved && !u.IsRejected && u.Role != UserRole.Admin)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    Role = u.Role.ToString(),
                    u.CreatedAt
                });
            return Ok(pending);
        }

        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> Approve(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) return NotFound("Utilizatorul nu a fost găsit.");
            if (user.IsApproved) return BadRequest("Contul este deja aprobat.");

            user.IsApproved = true;
            user.IsRejected = false;
            await _unitOfWork.Users.UpdateAsync(user);

            if (user.Role == UserRole.Client)
            {
                var existingClient = await _unitOfWork.Clients.GetByUserIdAsync(user.Id);
                if (existingClient is null)
                {
                    await _unitOfWork.Clients.AddAsync(new Client
                    {
                        UserId = user.Id,
                        Address = string.Empty
                    });
                }
            }
            else if (user.Role == UserRole.Doctor)
            {
                var doctors = await _unitOfWork.Doctors.GetAllAsync();
                var existingDoctor = doctors.FirstOrDefault(d => d.UserId == user.Id);
                if (existingDoctor is null)
                {
                    await _unitOfWork.Doctors.AddAsync(new Doctor
                    {
                        UserId = user.Id,
                        Specialization = string.Empty,
                        Bio = string.Empty,
                        Schedule = "[]"
                    });
                }
            }

            await _emailService.SendAccountApprovedAsync(
                toEmail: user.Email,
                userName: user.Name,
                role: user.Role == UserRole.Doctor ? "doctor" : "client"
            );

            return Ok(new { message = $"Contul lui {user.Name} a fost aprobat." });
        }

        [HttpPost("reject/{userId}")]
        public async Task<IActionResult> Reject(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) return NotFound("Utilizatorul nu a fost găsit.");

            user.IsApproved = false;
            user.IsRejected = true;
            await _unitOfWork.Users.UpdateAsync(user);

            await _emailService.SendAccountRejectedAsync(
                toEmail: user.Email,
                userName: user.Name
            );

            return Ok(new { message = $"Contul lui {user.Name} a fost respins." });
        }
    }
}