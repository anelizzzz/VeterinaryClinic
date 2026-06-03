using VeterinaryClinic.API.DTOs.AdoptionAnimal;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Services
{
    public class AdoptionAnimalService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdoptionAnimalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AdoptionAnimalResponseDto>> GetAllAsync()
        {
            var animals = await _unitOfWork.AdoptionAnimals.GetAllAsync();
            return animals.Select(a => new AdoptionAnimalResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Species = a.Species,
                Breed = a.Breed,
                Age = a.Age,
                Description = a.Description,
                Vaccines = a.Vaccines,
                ImageUrl = a.ImageUrl,
                Status = a.Status,
                AddedDate = a.AddedDate
            });
        }

        public async Task<AdoptionAnimalResponseDto?> GetByIdAsync(int id)
        {
            var animal = await _unitOfWork.AdoptionAnimals.GetByIdAsync(id);
            if (animal is null) return null;

            return new AdoptionAnimalResponseDto
            {
                Id = animal.Id,
                Name = animal.Name,
                Species = animal.Species,
                Breed = animal.Breed,
                Age = animal.Age,
                Description = animal.Description,
                Vaccines = animal.Vaccines,
                ImageUrl = animal.ImageUrl,
                Status = animal.Status,
                AddedDate = animal.AddedDate
            };
        }

        public async Task<AdoptionAnimalResponseDto> CreateAsync(AdoptionAnimalCreateDto dto)
        {
            var animal = new AdoptionAnimal
            {
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                Age = dto.Age,
                Description = dto.Description,
                Vaccines = dto.Vaccines,
                ImageUrl = dto.ImageUrl,
                Status = AdoptionStatus.Available,
                AddedDate = DateTime.UtcNow
            };

            await _unitOfWork.AdoptionAnimals.AddAsync(animal);

            return new AdoptionAnimalResponseDto
            {
                Id = animal.Id,
                Name = animal.Name,
                Species = animal.Species,
                Breed = animal.Breed,
                Age = animal.Age,
                Description = animal.Description,
                Vaccines = animal.Vaccines,
                ImageUrl = animal.ImageUrl,
                Status = animal.Status,
                AddedDate = animal.AddedDate
            };
        }

        public async Task<AdoptionAnimalResponseDto?> UpdateAsync(int id, AdoptionAnimalCreateDto dto)
        {
            var animal = await _unitOfWork.AdoptionAnimals.GetByIdAsync(id);
            if (animal is null) return null;

            animal.Name = dto.Name;
            animal.Species = dto.Species;
            animal.Breed = dto.Breed;
            animal.Age = dto.Age;
            animal.Description = dto.Description;
            animal.Vaccines = dto.Vaccines;
            animal.ImageUrl = dto.ImageUrl;

            await _unitOfWork.AdoptionAnimals.UpdateAsync(animal);

            return new AdoptionAnimalResponseDto
            {
                Id = animal.Id,
                Name = animal.Name,
                Species = animal.Species,
                Breed = animal.Breed,
                Age = animal.Age,
                Description = animal.Description,
                Vaccines = animal.Vaccines,
                ImageUrl = animal.ImageUrl,
                Status = animal.Status,
                AddedDate = animal.AddedDate
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var animal = await _unitOfWork.AdoptionAnimals.GetByIdAsync(id);
            if (animal is null) return false;

            await _unitOfWork.AdoptionAnimals.DeleteAsync(id);
            return true;
        }
    }
}