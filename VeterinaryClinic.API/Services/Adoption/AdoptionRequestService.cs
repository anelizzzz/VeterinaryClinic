using VeterinaryClinic.API.DTOs.AdoptionRequest;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Services
{
    public class AdoptionRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdoptionRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AdoptionRequestResponseDto>> GetAllAsync()
        {
            var requests = await _unitOfWork.AdoptionRequests.GetAllAsync();
            return requests.Select(r => new AdoptionRequestResponseDto
            {
                Id = r.Id,
                ClientName = r.Client?.User?.Name ?? string.Empty,
                AnimalName = r.AdoptionAnimal?.Name ?? string.Empty,
                RequestDate = r.RequestDate,
                Status = r.Status,
                Motivation = r.Motivation
            });
        }

        public async Task<AdoptionRequestResponseDto?> GetByIdAsync(int id)
        {
            var request = await _unitOfWork.AdoptionRequests.GetByIdAsync(id);
            if (request is null) return null;

            return new AdoptionRequestResponseDto
            {
                Id = request.Id,
                ClientName = request.Client?.User?.Name ?? string.Empty,
                AnimalName = request.AdoptionAnimal?.Name ?? string.Empty,
                RequestDate = request.RequestDate,
                Status = request.Status,
                Motivation = request.Motivation
            };
        }

        public async Task<IEnumerable<AdoptionRequestResponseDto>> GetByClientIdAsync(int clientId)
        {
            var requests = await _unitOfWork.AdoptionRequests.GetAllAsync();
            return requests
                .Where(r => r.ClientId == clientId)
                .Select(r => new AdoptionRequestResponseDto
                {
                    Id = r.Id,
                    ClientName = r.Client?.User?.Name ?? string.Empty,
                    AnimalName = r.AdoptionAnimal?.Name ?? string.Empty,
                    RequestDate = r.RequestDate,
                    Status = r.Status,
                    Motivation = r.Motivation
                });
        }

        public async Task<AdoptionRequestResponseDto> CreateAsync(int userId, AdoptionRequestCreateDto dto)
        {
            var client = await _unitOfWork.Clients
                .GetByUserIdAsync(userId);

            if (client == null)
                throw new Exception($"Nu există client asociat userId = {userId}");

            var animal = await _unitOfWork.AdoptionAnimals
                .GetByIdAsync(dto.AdoptionAnimalId);

            if (animal == null)
                throw new Exception($"Nu există animal cu id = {dto.AdoptionAnimalId}");

            var request = new AdoptionRequest
            {
                ClientId = client.Id,
                AdoptionAnimalId = dto.AdoptionAnimalId,
                Motivation = dto.Motivation,
                Status = AdoptionStatus.Pending,
                RequestDate = DateTime.UtcNow
            };

            await _unitOfWork.AdoptionRequests.AddAsync(request);

            return new AdoptionRequestResponseDto
            {
                Id = request.Id,
                RequestDate = request.RequestDate,
                Status = request.Status,
                Motivation = request.Motivation
            };
        }
        public async Task<IEnumerable<AdoptionRequestResponseDto>> GetByUserIdAsync(int userId)
        {
            var client = await _unitOfWork.Clients
                .GetByUserIdAsync( userId);

            if (client == null)
                throw new Exception($"Nu există client asociat userId = {userId}");

            return await GetByClientIdAsync(client.Id);
        }
        public async Task<AdoptionRequestResponseDto?> UpdateStatusAsync(int id, AdoptionStatus status)
        {
            var request = await _unitOfWork.AdoptionRequests.GetByIdAsync(id);
            if (request is null) return null;

            request.Status = status;
            await _unitOfWork.AdoptionRequests.UpdateAsync(request);

            return new AdoptionRequestResponseDto
            {
                Id = request.Id,
                ClientName = request.Client?.User?.Name ?? string.Empty,
                AnimalName = request.AdoptionAnimal?.Name ?? string.Empty,
                RequestDate = request.RequestDate,
                Status = request.Status,
                Motivation = request.Motivation
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _unitOfWork.AdoptionRequests.GetByIdAsync(id);
            if (request is null) return false;

            await _unitOfWork.AdoptionRequests.DeleteAsync(id);
            return true;
        }
    }
}