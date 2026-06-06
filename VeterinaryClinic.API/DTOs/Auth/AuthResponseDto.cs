using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsPending { get; set; } = false;
        public bool IsRejected { get; set; } = false;
    }
}