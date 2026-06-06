using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VeterinaryClinic.API.DTOs.Auth;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Models.Entities.Enums;
using VeterinaryClinic.API.Repositories.Users;
using VeterinaryClinic.API.Services.Email;

namespace VeterinaryClinic.API.Services
{
    public class AuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, UserRepository userRepository, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing is not null) return null;

            // Adminii se creează direct aprobați
            var isAdmin = dto.Role == UserRole.Admin;

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow,
                IsApproved = isAdmin,
                IsRejected = false
            };

            await _unitOfWork.Users.AddAsync(user);

            if (!isAdmin)
            {
                // Trimite email utilizatorului — cont în așteptare
                await _emailService.SendAccountPendingAsync(
                    toEmail: dto.Email,
                    userName: dto.Name,
                    role: dto.Role == UserRole.Doctor ? "doctor" : "client"
                );

                // Trimite email adminului cu cererea
                var adminEmail = _configuration["Email:AdminEmail"] ?? _configuration["Email:FromEmail"] ?? "";
                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    await _emailService.SendNewAccountRequestToAdminAsync(
                        adminEmail: adminEmail,
                        userName: dto.Name,
                        userEmail: dto.Email,
                        role: dto.Role == UserRole.Doctor ? "Doctor" : "Client",
                        userId: user.Id
                    );
                }

                // Returnăm răspuns special — cont în așteptare
                return new AuthResponseDto
                {
                    Token = string.Empty,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    IsPending = true
                };
            }

            // Admin — creat direct
            return new AuthResponseDto
            {
                Token = GenerateToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsPending = false
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user is null) return null;

            var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!isValid) return null;

            // Verificăm dacă contul e aprobat
            if (!user.IsApproved)
            {
                if (user.IsRejected)
                    return new AuthResponseDto { Token = string.Empty, IsPending = false, IsRejected = true };

                return new AuthResponseDto { Token = string.Empty, IsPending = true };
            }

            return new AuthResponseDto
            {
                Token = GenerateToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsPending = false
            };
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}