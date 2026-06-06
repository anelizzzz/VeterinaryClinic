using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Aprobare cont
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
    }
}