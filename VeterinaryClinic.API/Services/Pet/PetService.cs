using VeterinaryClinic.API.Dtos.Pet;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Services
{
    public class PetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public PetService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        public async Task<IEnumerable<PetResponseDto>> GetAllAsync()
        {
            var pets = await _unitOfWork.Pets.GetAllAsync();
            return pets.Select(p => new PetResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Species = p.Species,
                Breed = p.Breed,
                Birthdate = p.Birthdate,
                Status = p.Status,
                Vaccines = p.Vaccines,
                ImageUrl = p.ImageUrl,
                ClientName = p.Client?.User?.Name ?? string.Empty
            });
        }

        public async Task<PetResponseDto?> GetByIdAsync(int id)
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(id);
            if (pet is null) return null;

            return new PetResponseDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                Birthdate = pet.Birthdate,
                Status = pet.Status,
                Vaccines = pet.Vaccines,
                ImageUrl = pet.ImageUrl,
                ClientName = pet.Client?.User?.Name ?? string.Empty
            };
        }

        public async Task<IEnumerable<PetResponseDto>> GetByClientIdAsync(int clientId)
        {
            var pets = await _unitOfWork.Pets.GetAllAsync();
            return pets
                .Where(p => p.ClientId == clientId)
                .Select(p => new PetResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Species = p.Species,
                    Breed = p.Breed,
                    Birthdate = p.Birthdate,
                    Status = p.Status,
                    Vaccines = p.Vaccines,
                    ImageUrl = p.ImageUrl,
                    ClientName = p.Client?.User?.Name ?? string.Empty
                });
        }

        public async Task<PetResponseDto> CreateAsync(PetCreateDto dto)
        {
            var pet = new Pet
            {
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                Birthdate = dto.Birthdate,
                Vaccines = dto.Vaccines,
                ImageUrl = dto.ImageUrl,
                ClientId = dto.ClientId,
                Status = PetStatus.Healthy
            };

            await _unitOfWork.Pets.AddAsync(pet);
            await _unitOfWork.SaveChangesAsync();

            return new PetResponseDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                Birthdate = DateTime.SpecifyKind(dto.Birthdate, DateTimeKind.Utc),
                Status = pet.Status,
                Vaccines = pet.Vaccines,
                ImageUrl = pet.ImageUrl,
                ClientName = string.Empty
            };
        }

        public async Task<PetResponseDto?> UpdateAsync(int id, PetUpdateDto dto)
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(id);
            if (pet is null) return null;

            pet.Name = dto.Name;
            pet.Species = dto.Species;
            pet.Breed = dto.Breed;
            pet.Birthdate = DateTime.SpecifyKind(dto.Birthdate, DateTimeKind.Utc);
            pet.Vaccines = dto.Vaccines;
            pet.ImageUrl = dto.ImageUrl;

            await _unitOfWork.Pets.UpdateAsync(pet);

            return new PetResponseDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                Birthdate = pet.Birthdate,
                Status = pet.Status,
                Vaccines = pet.Vaccines,
                ImageUrl = pet.ImageUrl,
                ClientName = pet.Client?.User?.Name ?? string.Empty
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(id);
            if (pet is null) return false;

            await _unitOfWork.Pets.DeleteAsync(id);
            return true;
        }
        public async Task<bool> UpdatePetImageAsync(int petId, IFormFile file)
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null)
                return false;

            if (file == null || file.Length == 0)
                throw new ArgumentException("Fișierul este invalid.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Formatul fișierului nu este permis.");

            var webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "pets");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (!string.IsNullOrWhiteSpace(pet.ImageUrl))
            {
                var oldRelativePath = pet.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var oldFullPath = Path.Combine(_environment.WebRootPath, oldRelativePath);

                if (File.Exists(oldFullPath))
                {
                    File.Delete(oldFullPath);
                }
            }

            pet.ImageUrl = $"/uploads/pets/{uniqueFileName}";

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}