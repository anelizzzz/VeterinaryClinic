using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.API.DTOs.Auth;
using VeterinaryClinic.API.Services;

namespace VeterinaryClinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result is null)
                return BadRequest("Email-ul este deja folosit.");

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result is null)
                return Unauthorized("Email sau parolă incorectă.");

            return Ok(result);
        }
    }
}