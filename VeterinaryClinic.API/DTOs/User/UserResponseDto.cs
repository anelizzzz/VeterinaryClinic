using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.DTOs.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
    }
}